using System.Net;
using GlobalExceptionHandler.WebApi;
using LittleBlocks.Exceptions;
using Newtonsoft.Json;

namespace LittleBlocks.AspNetCore.Bootstrap.Extensions;

public static class WebApplicationExtensions
{
    public static void UseGlobalExceptionAndLoggingHandler(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseGlobalExceptionHandler(x =>
        {
            x.ContentType = "application/json";
            x.ResponseBody(s => JsonConvert.SerializeObject(new
            {
                Message = "An error occurred whilst processing your request"
            }));
            x.Map<AppException>()
                .ToStatusCode(HttpStatusCode.InternalServerError)
                .WithBody((e, context) => JsonConvert.SerializeObject(new {e.Message}));

            x.OnError((e, context) =>
            {
                app.Logger.LogError((Exception) e, "Error in processing the request to {Path}", context.Request.Path);
                return Task.CompletedTask;
            });
        });
    }

    public static void UseHttpsRedirection(this WebApplication app, HostInfo hostInfo)
    {
        ArgumentNullException.ThrowIfNull(hostInfo);
        if (hostInfo.RequiredSslRedirect)
            app.UseHttpsRedirection();
    }

    public static void UseOpenApiDocumentation(this WebApplication app, AppInfo appInfo)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(appInfo);

        var name = $"{appInfo.Name} v{appInfo.Version}";

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", name);
            c.DocumentTitle = name;
        });

        app.UseReDoc(c =>
        {
            c.DocumentTitle = name;
            c.SpecUrl("/swagger/v1/swagger.json");
        });
    }

    public static void MapDependencyHealthChecks(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapHealthChecks("/health",
            new HealthCheckOptions {ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse});
    }

    public static IServiceCollection AddDependencyHealthChecks(this IServiceCollection services,
        Action<IHealthChecksBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = services.AddHealthChecks();
        configure?.Invoke(builder);

        return services;
    }
}