
public sealed class TelemetryOptions
{
    public const string Section = "Telemetry";

    public int MaxBodyBytes { get; set; } = 64 * 1024;

    public HashSet<string> ExcludedPaths { get; set; } =
    [
        "/health", "/healthz", "/readyz", "/metrics", "/favicon.ico"
    ];
    public HashSet<string> ExcludedPrefixes { get; set; } =
    [
        "/scalar", "/swagger", "/_framework", "/_blazor", "/openapi"
    ];

    public HashSet<string> SensitiveHeaders { get; set; } =
    [
        "authorization", "cookie", "set-cookie",
        "x-api-key", "x-auth-token", "proxy-authorization"
    ];

    public int LogBodyOnStatusGte { get; set; } = 0;

    public TimeSpan SlowRequestThreshold { get; set; } = TimeSpan.FromSeconds(2);

    public Dictionary<string, string> GlobalContext { get; set; } = [];

    public string ActivitySourceName { get; set; } = "CustomNotesService";
    public string ServiceName { get; set; } = "NotesService";
}