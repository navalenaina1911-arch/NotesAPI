using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Notes.Extensions;
public static class TelemetryExtensions
{
    // Expose ActivitySource for custom spans
    public static readonly ActivitySource ActivitySource = new("CustomNotesAPI");

    public static void AddTelemetryExtensions(
        this WebApplicationBuilder builder)
    {
        // Bind settings directly from configuration
        TelemetryOptions settings = builder.Configuration
                                     .GetSection("Telemetry")
                                     .Get<TelemetryOptions>()
                                     ?? throw new InvalidOperationException("TelemetrySettings is missing from configuration.");

        builder.Services.AddSingleton(new ActivitySource(settings.ActivitySourceName));
      
        // LOGGING

        builder.Logging.AddJsonConsole(options =>
        {
            options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Indented = true // optional: pretty print
            };
        });

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;

            logging.SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService(settings.ServiceName));
        });
        // ============================
        // TRACING + METRICS
        // ============================
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(settings.ServiceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddSource(settings.ActivitySourceName); // custom spans
                    
            })
            .WithMetrics(metrics =>
            {
                metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation();
            });
    }
}


