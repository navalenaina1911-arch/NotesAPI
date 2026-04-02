using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Notes.Middlewares;

public sealed class StructuredTelemetryMiddleware
{
    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly RequestDelegate _next;
    private readonly TelemetryOptions _options;
    private readonly ILogger<StructuredTelemetryMiddleware> _logger;

    public StructuredTelemetryMiddleware(
        RequestDelegate next,
        IOptions<TelemetryOptions> options,
        ILogger<StructuredTelemetryMiddleware> logger)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        var requestPath = ctx.Request.Path.Value ?? string.Empty;
        if (_options.ExcludedPaths.Contains(requestPath)
            || _options.ExcludedPrefixes.Any(p => requestPath.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(ctx);
            return;
        }

        var traceId = Activity.Current?.TraceId.ToString()
                        ?? ctx.TraceIdentifier
                        ?? Guid.NewGuid().ToString("N");
        ctx.Response.Headers["X-Request-ID"] = traceId;

        var sw = Stopwatch.StartNew();

        // ── Buffer request body ──────────────────────────────────────────────
        ctx.Request.EnableBuffering();
        var (rawReq, reqTruncated) = await ReadBodyAsync(
            ctx.Request.Body,
            ctx.Request.Headers.ContentEncoding.ToString());
        ctx.Request.Body.Position = 0;

        // ── Buffer response body ─────────────────────────────────────────────
        var originalBody = ctx.Response.Body;
        using var responseBuffer = new MemoryStream();
        ctx.Response.Body = responseBuffer;

        Exception? caughtEx = null;

        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            caughtEx = ex;
            throw;
        }
        finally
        {
            sw.Stop();

            responseBuffer.Position = 0;
            var (rawResp, respTruncated) = await ReadBodyAsync(
                responseBuffer,
                ctx.Response.Headers.ContentEncoding.ToString());

            responseBuffer.Position = 0;
            await responseBuffer.CopyToAsync(originalBody);
            ctx.Response.Body = originalBody;

            bool logBodies = _options.LogBodyOnStatusGte == 0
                          || ctx.Response.StatusCode >= _options.LogBodyOnStatusGte;

            var durationMs = Math.Round(sw.Elapsed.TotalMilliseconds, 3);
            var level = caughtEx is not null ? LogLevel.Error
                      : sw.Elapsed > _options.SlowRequestThreshold ? LogLevel.Warning
                                                                     : LogLevel.Information;

            // ── Log 1: human-readable summary ────────────────────────────────
            _logger.Log(
                level,
                "HTTP {Method} {Path} responded {ResponseStatus} in {DurationMs}ms",
                ctx.Request.Method,
                requestPath,
                ctx.Response.StatusCode,
                durationMs);

            // ── Log 2: structured telemetry via IReadOnlyList state ───────────
            // The JSON console formatter iterates the state list and writes each
            // entry as a named property. No blobs, no ToString() noise.
            var state = new TelemetryLogState(
                traceId: traceId,
                parentSpanId: Activity.Current?.ParentSpanId.ToString(),
                spanId: Activity.Current?.SpanId.ToString(),
                method: ctx.Request.Method,
                path: requestPath,
                queryString: ctx.Request.QueryString.HasValue ? ctx.Request.QueryString.Value : null,
                requestHeaders: RedactHeaders(ctx.Request.Headers),
                requestBody: logBodies ? ParseBody(rawReq) : null,
                requestBodyTruncated: reqTruncated,
                responseStatus: ctx.Response.StatusCode,
                responseHeaders: RedactHeaders(ctx.Response.Headers),
                responseBody: logBodies ? ParseBody(rawResp) : null,
                responseBodyTruncated: respTruncated,
                durationMs: durationMs,
               
                slowRequest: sw.Elapsed > _options.SlowRequestThreshold,
                error: caughtEx is null ? null : (object)new
                {
                    type = caughtEx.GetType().FullName,
                    message = caughtEx.Message,
                    stackTrace = caughtEx.StackTrace,
                },
                globalContext: _options.GlobalContext.Count > 0 ? _options.GlobalContext : null);

            _logger.Log(level, new EventId(0, "HttpRequestTelemetry"), state, caughtEx,
                (s, _) => s.ToString());
        }
    }

    // ── Read stream up to MaxBodyBytes, decompress if needed ──────────────────
    private async Task<(byte[] Data, bool Truncated)> ReadBodyAsync(
        Stream stream, string contentEncoding)
    {
        if (!stream.CanRead) return ([], false);

        var limit = _options.MaxBodyBytes;
        var ms = new MemoryStream();
        var buffer = new byte[Math.Min(limit + 1, 81_920)];
        bool truncated = false;
        int read;

        while ((read = await stream.ReadAsync(buffer)) > 0)
        {
            if (ms.Length + read > limit)
            {
                ms.Write(buffer, 0, (int)(limit - ms.Length));
                truncated = true;
                break;
            }
            ms.Write(buffer, 0, read);
        }

        var raw = ms.ToArray();

        raw = contentEncoding.ToLowerInvariant() switch
        {
            "gzip" => await DecompressAsync<GZipStream>(raw),
            "deflate" => await DecompressAsync<DeflateStream>(raw),
            _ => raw,
        };

        return (raw, truncated);
    }

    private static async Task<byte[]> DecompressAsync<T>(byte[] data) where T : Stream
    {
        try
        {
            await using var input = new MemoryStream(data);
            await using var decomp = (Stream)Activator.CreateInstance(
                typeof(T), input, CompressionMode.Decompress)!;
            await using var output = new MemoryStream();
            await decomp.CopyToAsync(output);
            return output.ToArray();
        }
        catch { return data; }
    }

    // ── Parse body bytes → JsonElement if JSON, string otherwise ─────────────

    private static object? ParseBody(byte[] data)
    {
        if (data is not { Length: > 0 }) return null;

        int i = 0;
        while (i < data.Length && data[i] is (byte)' ' or (byte)'\t' or (byte)'\r' or (byte)'\n') i++;

        if (i < data.Length && data[i] is (byte)'{' or (byte)'[')
        {
            try { return JsonSerializer.Deserialize<JsonElement>(data); }
            catch { /* fall through */ }
        }

        return Encoding.UTF8.GetString(data);
    }

    // ── Redact sensitive headers ──────────────────────────────────────────────

    private Dictionary<string, string> RedactHeaders(IHeaderDictionary headers)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in headers)
        {
            result[key] = _options.SensitiveHeaders.Contains(key.ToLowerInvariant())
                ? "***REDACTED***"
                : value.ToString();
        }
        return result;
    }
}