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

/// <summary>
/// Configuration options for minimal API pipeline.
/// </summary>
public sealed class MinimalApiPipelineOptions
{
    public bool EnableStartPage { get; set; } = true;
    public Action PreEndpointConfiguration { get; set; } = null;
    public Action PostEndpointConfiguration { get; set; } = null;
    public Action PostAuthenticationConfiguration { get; set; } = null;
}

/// <summary>
/// Extension methods for WebApplication to support LittleBlocks minimal API pipeline configuration.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the default LittleBlocks API pipeline for minimal APIs.
    /// </summary>
    /// <param name="app">The WebApplication instance</param>
    /// <param name="options">Optional pipeline configuration options</param>
    /// <returns>The configured WebApplication</returns>
    public static WebApplication UseLittleBlocksPipeline(
        this WebApplication app,
        MinimalApiPipelineOptions options = null)
    {
        ArgumentNullException.ThrowIfNull(app);
        
        options ??= new MinimalApiPipelineOptions();
        
        InitiateFlushOutstandingOperations(app.Lifetime);
        ConfigureDefaultPipeline(app, options);
        
        return app;
    }

    /// <summary>
    /// Configures the default LittleBlocks API pipeline for minimal APIs with custom configuration.
    /// </summary>
    /// <param name="app">The WebApplication instance</param>
    /// <param name="configureOptions">Action to configure pipeline options</param>
    /// <returns>The configured WebApplication</returns>
    public static WebApplication UseLittleBlocksPipeline(
        this WebApplication app,
        Action<MinimalApiPipelineOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configureOptions);
        
        var options = new MinimalApiPipelineOptions();
        configureOptions(options);
        
        return app.UseLittleBlocksPipeline(options);
    }

    private static void InitiateFlushOutstandingOperations(IHostApplicationLifetime lifetime)
    {
        lifetime.ApplicationStopped.Register(Log.CloseAndFlush);
    }

    private static void ConfigureDefaultPipeline(WebApplication app, MinimalApiPipelineOptions options)
    {
        var appInfo = app.Configuration.GetApplicationInfo();
        var authOptions = app.Configuration.GetAuthOptions();

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

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRequestCorrelation();
        app.UseCorrelatedLogs();
        app.UseRouting();
        app.UseCorsWithDefaultPolicy();
        
        // Only add authentication middleware if authentication is configured
        if (authOptions.AuthenticationMode != AuthenticationMode.None)
        {
            app.UseAuthentication();
        }
        app.UseAuthorization();

        options.PostAuthenticationConfiguration?.Invoke();

        app.UseUserIdentityLogging();
        app.UseDiagnostics();
        
        // Configure Swagger for minimal APIs
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Pre-endpoint configuration
        options.PreEndpointConfiguration?.Invoke();

        // Configure health checks endpoint
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });

        // Configure start page if enabled
        if (options.EnableStartPage)
        {
            app.MapGet("/", context =>
            {
                var content = LoadStartPageFromEmbeddedResource(appInfo.Name);
                if (string.IsNullOrEmpty(content))
                    content = appInfo.Name;

                context.Response.ContentType = "text/html";
                return context.Response.WriteAsync(content);
            });
        }

        // Post-endpoint configuration
        options.PostEndpointConfiguration?.Invoke();

        LogResolvedEnvironment(app.Environment, app.Services.GetRequiredService<ILoggerFactory>());
    }

    private static string LoadStartPageFromEmbeddedResource(string applicationName)
    {
        var assembly = typeof(AppBootstrapper<>).Assembly;
        var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith("home.html", StringComparison.CurrentCultureIgnoreCase));
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