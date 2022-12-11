namespace LittleBlocks.AspNetCore.Health;

public static class HealthExtensions
{
    public static IServiceCollection AddDefaultHealthChecks(this IServiceCollection services,
        Action<IHealthChecksBuilder> configure)
    {
        var healthCheckBuilder = services
            .AddHealthChecks();

        configure(healthCheckBuilder);

        return services;
    }
}
