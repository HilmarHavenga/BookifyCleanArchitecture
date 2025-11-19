namespace Bookify.Api.Endpoints.Health;

public class HealthCheckEndpoints : IEndpoints
{
    public static string Tag => $"{nameof(HealthCheckEndpoints)}";

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder versioned = app.MapGroup("/api/health");

        versioned.MapGet("/", CheckHealth)
            .WithName("CheckHealth")
            .Produces<HealthCheckResult>(200)
            .Produces(503)
            .WithTags(Tag);
    }

    internal static async Task<IResult> CheckHealth(HealthCheckService healthCheckService, CancellationToken cancellationToken)
    {
        var healthReport = await healthCheckService.CheckHealthAsync(cancellationToken);

        var response = new
        {
            status = healthReport.Status.ToString(),
            totalDuration = healthReport.TotalDuration,
            entries = healthReport.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration,
                exception = entry.Value.Exception?.Message,
                data = entry.Value.Data.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToString())
            })
        };

        var statusCode = healthReport.Status == HealthStatus.Healthy
            ? StatusCodes.Status200OK
            : StatusCodes.Status503ServiceUnavailable;

        return Results.Json(response, statusCode: statusCode);
    }
}