namespace Bookify.Api.Endpoints.Health;

public class CustomSqlHealthCheck(IServiceProvider ServiceProvider) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var scope = ServiceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (await dbContext.Database.CanConnectAsync(cancellationToken))
            {
                return HealthCheckResult.Healthy("Database is Healthy");
            }

            return HealthCheckResult.Unhealthy("Database is Unhealthy");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(exception: e);
        }
    }
}