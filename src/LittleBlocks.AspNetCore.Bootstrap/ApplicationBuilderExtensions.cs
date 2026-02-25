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

using Serilog;

namespace LittleBlocks.AspNetCore.Bootstrap;

public static class ApplicationBuilderExtensions
{
    public static void UseDefaultApiPipeline(this IApplicationBuilder app,
        IConfiguration configuration,
        IWebHostEnvironment env,
        IHostApplicationLifetime lifetime,
        ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(lifetime);
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(env);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        InitiateFlushOutstandingOperations(lifetime);
        var options = new ApiPipelineOptions(configuration, env, loggerFactory);
        app.UseDefaultApiPipeline(options);
    }

    private static void InitiateFlushOutstandingOperations(IHostApplicationLifetime lifetime)
    {
        lifetime.ApplicationStopped.Register(Log.CloseAndFlush);
    }

    private static void UseDefaultApiPipeline(this IApplicationBuilder app, ApiPipelineOptions options)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(options);

        var appInfo = options.Configuration.GetApplicationInfo();
        var authOptions = options.Configuration.GetAuthOptions();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.ExceptionHandling))
        {
            if (options.Environment.IsDevelopment())
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

        options.PostAuthenticationConfigure?.Invoke();

        app.UseUserIdentityLogging();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.Diagnostics))
            app.UseDiagnostics();

        if (options.Features.HasFlag(ApiBootstrapperFeatures.OpenApi))
            app.UseOpenApiDocumentation(appInfo, u => u.ConfigureAuth(appInfo, authOptions.Authentication));

        app.UseEndpoints(endpoints =>
        {
            options.PreEndPointsConfigure?.Invoke(endpoints);

            if (options.Features.HasFlag(ApiBootstrapperFeatures.HealthChecks))
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                });
            }

            if (options.Features.HasFlag(ApiBootstrapperFeatures.Controllers))
                endpoints.MapControllers();

            if (options.Features.HasFlag(ApiBootstrapperFeatures.StartPage) && options.EnableStartPage)
                options.StartPageConfigure?.Invoke(endpoints, appInfo);

            options.PostEndPointsConfigure?.Invoke(endpoints);
        });

        LogResolvedEnvironment(options.Environment, options.LoggerFactory);
    }

    public static void UseStartPage(this IEndpointRouteBuilder endpoints, string applicationName)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        ArgumentNullException.ThrowIfNull(applicationName);

        endpoints.MapGet("/", context =>
        {
            var content = LoadStartPageFromEmbeddedResource(applicationName);
            if (string.IsNullOrEmpty(content))
                content = applicationName;

            context.Response.ContentType = "text/html";
            return context.Response.WriteAsync(content);
        });
    }

    private static string LoadStartPageFromEmbeddedResource(string applicationName)
    {
        var assembly = typeof(AppBootstrapper<>).Assembly;
        var resourceName = assembly.GetManifestResourceNames().First(s => s.EndsWith("home.html", StringComparison.CurrentCultureIgnoreCase));
        if (string.IsNullOrEmpty(resourceName))
            return null;

        try
        {
            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return null;

            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();
            content = content.Replace("{{application}}", applicationName);
            return content;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static void LogResolvedEnvironment(IHostEnvironment env, ILoggerFactory loggerFactory)
    {
        var log = loggerFactory.CreateLogger("Startup");
        log.LogInformation($"{nameof(Application)} is started in '{env.EnvironmentName.ToUpper()}' environment ...");
    }
}
