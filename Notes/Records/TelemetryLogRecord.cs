// ─── TelemetryLogState ────────────────────────────────────────────────────────
// Implements IReadOnlyList<KeyValuePair<string,object?>> so the JSON console
// formatter writes each field as a real property, not a stringified blob.

using OpenTelemetry.Trace;

internal sealed class TelemetryLogState : IReadOnlyList<KeyValuePair<string, object?>>
{
    private readonly List<KeyValuePair<string, object?>> _fields;

    public TelemetryLogState(
        string traceId,
        string spanId,
        string parentSpanId,
        string method,
        string? path,
        string? queryString,
        Dictionary<string, string> requestHeaders,
        object? requestBody,
        bool requestBodyTruncated,
        int responseStatus,
        Dictionary<string, string> responseHeaders,
        object? responseBody,
        bool responseBodyTruncated,
        double durationMs,
        bool slowRequest,
        object? error,
        Dictionary<string, string>? globalContext)
    {
        _fields =
        [
            new("traceId",             traceId),
            new("spanId",              spanId),
            new("parentSpanId",        parentSpanId),
            new("method",                method),
            new("path",                  path),
            new("queryString",           queryString),
            new("requestHeaders",        requestHeaders),
            new("requestBody",           requestBody),
            new("requestBodyTruncated",  requestBodyTruncated),
            new("responseStatus",        responseStatus),
            new("responseHeaders",       responseHeaders),
            new("responseBody",          responseBody),
            new("responseBodyTruncated", responseBodyTruncated),
            new("durationMs",            durationMs),
            new("slowRequest",           slowRequest),
            new("error",                 error),
            new("globalContext",         globalContext),
            // {OriginalFormat} is required by the logging infrastructure.
            new("{OriginalFormat}",      "Telemetry"),
        ];
    }
    public KeyValuePair<string, object?> this[int index] => _fields[index];
    public int Count => _fields.Count;
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _fields.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _fields.GetEnumerator();

    // ToString() is what the Message field shows — keep it minimal.
    public override string ToString() => "telemetry";
}
