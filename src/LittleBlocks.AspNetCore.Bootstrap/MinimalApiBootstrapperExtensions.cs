// This software is part of the LittleBlocks framework
// Copyright (C) 2024 LittleBlocks
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace LittleBlocks.AspNetCore.Bootstrap;

public static class MinimalApiBootstrapperExtensions
{
    public static MinimalApiBootstrapOptions BootstrapMinimalApi(this WebApplicationBuilder builder,
        Action<MinimalApiBootstrapOptions> configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var options = new MinimalApiBootstrapOptions();
        configure?.Invoke(options);

        ApplyServiceConfiguration(builder.Services, builder.Configuration, options);
        builder.Services.AddSingleton(options);

        return options;
    }

    public static void UseMinimalApiBootstrap(this WebApplication app, MinimalApiBootstrapOptions options = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        options ??= app.Services.GetService<MinimalApiBootstrapOptions>() ?? new MinimalApiBootstrapOptions();

        options.AppInfo ??= app.Configuration.GetApplicationInfo();
        options.AuthOptions ??= app.Configuration.GetAuthOptions();

        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        ConfigurePipeline(app, options, loggerFactory);
    }

    private static void ApplyServiceConfiguration(IServiceCollection services, IConfiguration configuration,
        MinimalApiBootstrapOptions options)
    {
        var configurationBuilder = new ConfigurationOptionBuilder(services, configuration);
        if (options.Features.HasFlag(ApiBootstrapperFeatures.Configuration))
        {
            configurationBuilder.Build();
            foreach (var configureSection in options.ConfigurationSections)
                configureSection(configurationBuilder);
        }

        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.TryAddScoped<IArgumentsFormatter, ArgumentsFormatter>();
        services.TryAddSingleton(_ => new ArgumentFormatterOptions());
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddHttpRequestContext();

        ConfigureErrorHandling(services, options);

        if (options.Features.HasFlag(ApiBootstrapperFeatures.RequestCorrelation))
            services.AddRequestCorrelation(b => options.CorrelationOptions(b.ExcludeDefaultUrls()));

        if (options.Features.HasFlag(ApiBootstrapperFeatures.FeatureFlags))
            services.AddFeatureFlagging(configuration);

        if (options.Features.HasFlag(ApiBootstrapperFeatures.Controllers))
            services.AddControllers().AddNewtonsoftJson(o => o.SerializerSettings.ConfigureJsonSettings());

        if (options.Features.HasFlag(ApiBootstrapperFeatures.Cors))
            services.AddDefaultCorsPolicy();

        options.AppInfo ??= configuration.GetApplicationInfo();
        options.AuthOptions ??= configuration.GetAuthOptions();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.Authentication))
            services.AddAuthentication(options.AuthOptions);

        if (options.Features.HasFlag(ApiBootstrapperFeatures.OpenApi))
        {
            services.AddEndpointsApiExplorer();
            services.AddOpenApiDocumentation(options.AppInfo, options.AuthOptions);
        }

        if (options.Features.HasFlag(ApiBootstrapperFeatures.HealthChecks))
        {
            var healthChecksBuilder = services.AddHealthChecks();
            options.ConfigureHealthChecks?.Invoke(healthChecksBuilder);
        }

        options.ConfigureServices?.Invoke(services, configuration);
    }

    private static void ConfigureErrorHandling(IServiceCollection services, MinimalApiBootstrapOptions options)
    {
        if (options.Features.HasFlag(ApiBootstrapperFeatures.ExceptionHandling) == false)
            return;

        var errorHandlerBuilder = new GlobalErrorHandlerConfigurationBuilder(services);

        switch (options.ErrorDetails)
        {
            case LevelOfDetails.UserErrors:
                errorHandlerBuilder.UseUserErrors();
                break;
            case LevelOfDetails.DetailedErrors:
                errorHandlerBuilder.UseDetailedErrors();
                break;
            default:
                errorHandlerBuilder.UseStandardMessage();
                break;
        }

        services.AddGlobalExceptionHandler(_ => errorHandlerBuilder.UseDefault());
    }

    private static void ConfigurePipeline(WebApplication app, MinimalApiBootstrapOptions options,
        ILoggerFactory loggerFactory)
    {
        var appInfo = options.AppInfo ?? app.Configuration.GetApplicationInfo();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.ExceptionHandling))
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseGlobalExceptionHandler();
                app.UseHsts();
            }
        }

        if (options.Features.HasFlag(ApiBootstrapperFeatures.HttpsRedirection))
            app.UseHttpsRedirection();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.StaticFiles))
            app.UseStaticFiles();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.RequestCorrelation))
        {
            app.UseRequestCorrelation();
            app.UseCorrelatedLogs();
        }

        app.UseRouting();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.Cors))
            app.UseCorsWithDefaultPolicy();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.Authentication))
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        app.UseUserIdentityLogging();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.Diagnostics))
            app.UseDiagnostics();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.OpenApi))
            app.UseOpenApiDocumentation(appInfo, ui => ui.ConfigureAuth(appInfo, options.AuthOptions.Authentication));

        if (options.Features.HasFlag(ApiBootstrapperFeatures.HealthChecks))
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });
        }

        options.ConfigureEndpoints?.Invoke(app);

        if (options.Features.HasFlag(ApiBootstrapperFeatures.StartPage) && options.EnableStartPage)
            app.UseStartPage(appInfo.Name);

        options.PostConfigureEndpoints?.Invoke(app);

        LogResolvedEnvironment(app.Environment, loggerFactory);
    }

    private static void LogResolvedEnvironment(IHostEnvironment env, ILoggerFactory loggerFactory)
    {
        var log = loggerFactory.CreateLogger("Startup");
        log.LogInformation($"{nameof(Application)} is started in '{env.EnvironmentName.ToUpper()}' environment ...");
    }
}
