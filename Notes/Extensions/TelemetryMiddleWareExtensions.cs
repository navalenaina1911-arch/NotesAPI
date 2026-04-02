using Notes.Middlewares;

namespace Notes.Extensions
{
    public static class TelemetryMiddlewareExtensions
    {
        public static IApplicationBuilder UseStructuredTelemetry(this IApplicationBuilder app)
            => app.UseMiddleware<StructuredTelemetryMiddleware>();

        public static IServiceCollection AddStructuredTelemetry(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<TelemetryOptions>(configuration.GetSection(TelemetryOptions.Section));
            return services;
        }

    }
}
