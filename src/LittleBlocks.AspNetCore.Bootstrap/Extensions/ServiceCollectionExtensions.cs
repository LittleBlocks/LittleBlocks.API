using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;

namespace LittleBlocks.AspNetCore.Bootstrap.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorsWithDefaultPolicy(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddCors(c =>
            c.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
        return services;
    }

    public static IServiceCollection AddTypeMapping(this IServiceCollection services,
        Action<IMapperConfigurationExpression> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        services.AddTransient<IMapper>(sp => new MapperConfiguration(configure).CreateMapper());

        return services;
    }

    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services,
        AppInfo appInfo)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(appInfo);

        services.AddSwaggerGen(d =>
        {
            d.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"{appInfo.Name} - {appInfo.Environment}",
                Version = appInfo.Version,
                Description = appInfo.Description
            });

            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "Ardevora.Feeds.Bloomberg.*.xml",
                SearchOption.TopDirectoryOnly).ToList();
            xmlFiles.ForEach(xmlFile => d.IncludeXmlComments(xmlFile));
        });
        services.AddFluentValidationRulesToSwagger();

        return services;
    }

    public static IServiceCollection AddFeatures<T>(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var name = typeof(T).Name;
        services.AddFeatureManagement(configuration.GetSection(name));

        return services;
    }

    public static IServiceCollection Remove<T>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        services.Remove(descriptor);

        return services;
    }
}