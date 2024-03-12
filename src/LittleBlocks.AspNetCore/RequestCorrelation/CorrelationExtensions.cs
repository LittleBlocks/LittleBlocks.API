using CorrelationId;
using CorrelationId.DependencyInjection;

namespace LittleBlocks.AspNetCore.RequestCorrelation;

public static class CorrelationExtensions
{
    public static IServiceCollection AddRequestCorrelation(this IServiceCollection service,
        Action<CorrelationIdOptions> configure = null)
    {
        return service.AddDefaultCorrelationId(options =>
        {
            options.CorrelationIdGenerator = () => Guid.NewGuid().ToString();
            options.AddToLoggingScope = true;
            options.EnforceHeader = false;
            options.IgnoreRequestHeader = false;
            options.IncludeInResponse = true;
            options.UpdateTraceIdentifier = true;

            configure?.Invoke(options);
        });
    }

    public static void UseRequestCorrelation(this WebApplication app)
    {
        app.UseCorrelationId();
    }
}