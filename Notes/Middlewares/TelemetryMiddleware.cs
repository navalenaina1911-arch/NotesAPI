//using System.Diagnostics;
//using System.Text;
//using System.Text.Json;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;

//namespace Notes.Middlewares;

//public class StructuredTelemetryMiddleware
//{
//    private readonly RequestDelegate _next;
//    private readonly ILogger<StructuredTelemetryMiddleware> _logger;

//    private static readonly string[] StaticFileExtensions =
//    {
//        ".js", ".css", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".woff", ".woff2", ".ttf", ".map"
//    };

//    public StructuredTelemetryMiddleware(RequestDelegate next, ILogger<StructuredTelemetryMiddleware> logger)
//    {
//        _next = next;
//        _logger = logger;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {

//        Console.WriteLine("Middleware logger enabled: " + _logger.IsEnabled(LogLevel.Information));

//        var path = context.Request.Path.Value ?? "";

//        if (path.Contains("swagger", StringComparison.OrdinalIgnoreCase) ||
//            path.Contains("scalar", StringComparison.OrdinalIgnoreCase) ||
//            path.Contains("openapi", StringComparison.OrdinalIgnoreCase) ||
//            IsStaticFile(path))
//        {
//            await _next(context);
//            return;
//        }

//        var correlationId = context.TraceIdentifier;
//        context.Response.Headers["X-Correlation-ID"] = correlationId;

//        var activity = Activity.Current;

//        using (_logger.BeginScope(new Dictionary<string, object?>
//        {
//            ["TraceId"] = activity?.TraceId.ToString(),
//            ["SpanId"] = activity?.SpanId.ToString(),
//            ["CorrelationId"] = context.TraceIdentifier;
//        }))
//        {
//            // Capture request body
//            string requestBody = await ReadRequestBody(context);
//            if (!string.IsNullOrWhiteSpace(requestBody))
//            {
//                LogJsonSafely("RequestBody", requestBody);
//            }

//            // Capture response body
//            var originalBody = context.Response.Body;
//            using var newBody = new MemoryStream();
//            context.Response.Body = newBody;

//            var sw = Stopwatch.StartNew();
//            await _next(context);
//            sw.Stop();

//            newBody.Position = 0;
//            string responseBody = await new StreamReader(newBody).ReadToEndAsync();

//            if (!string.IsNullOrWhiteSpace(responseBody))
//            {
//                LogJsonSafely("ResponseBody", responseBody);
//            }

//            _logger.LogInformation("RequestDurationMs: {Duration}", sw.ElapsedMilliseconds);

//            newBody.Position = 0;
//            await newBody.CopyToAsync(originalBody);
//            context.Response.Body = originalBody;
//        }
//    }

//    private static bool IsStaticFile(string path)
//    {
//        return StaticFileExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
//    }

//    private async Task<string> ReadRequestBody(HttpContext context)
//    {
//        context.Request.EnableBuffering();

//        using var reader = new StreamReader(
//            context.Request.Body,
//            encoding: Encoding.UTF8,
//            detectEncodingFromByteOrderMarks: false,
//            leaveOpen: true);

//        string body = await reader.ReadToEndAsync();
//        context.Request.Body.Position = 0;

//        if (body.Length > 5000)
//            return body.Substring(0, 5000) + "...(truncated)";

//        return body;
//    }

//    private void LogJsonSafely(string label, string raw)
//    {
//        try
//        {
//            using var doc = JsonDocument.Parse(raw);
//            string pretty = JsonSerializer.Serialize(doc, new JsonSerializerOptions
//            {
//                WriteIndented = true
//            });

//            _logger.LogInformation("{Label}:\n{Pretty}", label, pretty);
//        }
//        catch
//        {
//            _logger.LogInformation("{Label}: {Raw}", label, raw);
//        }
//    }
//}

//public static class StructuredTelemetryMiddlewareExtensions
//{
//    public static IApplicationBuilder UseStructuredTelemetry(this IApplicationBuilder app)
//    {
//        return app.UseMiddleware<StructuredTelemetryMiddleware>();
//    }
//}
