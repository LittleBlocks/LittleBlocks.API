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

using System.Text.Json.Serialization;
using Asp.Versioning;
using FluentValidation.AspNetCore;
using LittleBlocks.AspNetCore.RequestCorrelation;
using LittleBlocks.Configurations;
using LittleBlocks.Logging.SeriLog;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Serilog;

namespace LittleBlocks.Sample.Minimal.WebAPI;

public class WebApp
{
    public static IWebAppBuilder CreateBuilder(string[] args, Action<WebAppOptions> webAppOptions)
    {
        
    }
}

public class WebAppOptions(AppInfo appInfo, HostInfo hostInfo, LoggingContext loggingContext)
{
    public AppInfo AppInfo { get; } = appInfo;
    public HostInfo HostInfo { get; } = hostInfo;
    public LoggingContext LoggingContext { get; }  = loggingContext;

    public Action<ConfigureHostBuilder> ConfigureLogging { get; set; } = h =>
        h.UseSerilog((context, configuration) => (context, configuration).ConfigureSerilog(loggingContext));
    
    public Action<JsonOptions> ConfigureJson { get; set; } = o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    public Action<ApiVersioningOptions> ConfigureApiVersioning { get; set; } = o =>
    {
        o.ReportApiVersions = true;
        o.ApiVersionReader = new MediaTypeApiVersionReader();
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
    };
    
    public Action<ApiVersioningOptions> ConfigureDependencyHealthCheck { get; set; } = o =>
    {
        o.ReportApiVersions = true;
        o.ApiVersionReader = new MediaTypeApiVersionReader();
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
    };  
    
    public Action<IServiceCollection, AppInfo> ConfigureApiDocumentation { get; set; } = (services, appInfo) =>
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocumentation(appInfo);
    };  
}

public interface IWebAppBuilder
{
    WebApplication Build();
}

public sealed class WebAppBuilder(string[] args, Action<WebAppOptions> configure) : IWebAppBuilder
{
    public WebApplication Build()
    {
        var builder = WebApplication.CreateBuilder(args);

        var appInfo = builder.GetApplicationInfo();
        var hostInfo = builder.GetHostInfo();
        var loggingContext = builder.GetLoggingContext();

        var options = new WebAppOptions(appInfo, hostInfo, loggingContext);

        configure(options);

        options.ConfigureLogging(builder.Host);

        builder.Services.AddControllers()
            .AddJsonOptions(o => options.ConfigureJson(o))
            .AddFluentValidationAutoValidation();
        
        builder.Services.AddValidatorsFromAssemblyContaining<>();
        
        options.ConfigureApiVersioning()
        
        options.ConfigureLogging(builder.Host);
    }
}

var appx = WebApp.CreateBuilder(args, o => { }).Build();


var builder = WebApplication.CreateBuilder(args);

var applicationInfo = builder.GetApplicationInfo();
var hostInfo = builder.GetHostInfo();
var loggingContext = builder.GetLoggingContext();

builder.Host.UseSerilog((context, configuration) => (context, configuration).ConfigureSerilog(loggingContext));
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}).AddFluentValidation();
builder.Services.AddValidatorsFromAssemblyContaining<EntityValidator>();

builder.Services.AddApiVersioning(o =>
{
    o.ReportApiVersions = true;
    o.ApiVersionReader = new MediaTypeApiVersionReader();
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
});

builder.Services.AddDependencyHealthChecks();

builder.Services
    .AddSingleton(applicationInfo)
    .AddRouting(o => o.LowercaseUrls = true)
    .AddEndpointsApiExplorer()
    .AddOpenApiDocumentation(applicationInfo)
    .AddRequestCorrelation()
    .AddCorsWithDefaultPolicy()
    .AddTypeMapping(c => c.AddProfile<MappingProfile>());

var app = builder.Build();
appx.RunAsync(o =>
{
    
});

app.UseRequestCorrelation();

app.UseOpenApiDocumentation(applicationInfo);
if (!app.Environment.IsDevelopment())
{
    app.UseGlobalExceptionAndLoggingHandler();
    app.UseHsts();
}

app.UseCors();
app.UseSerilogRequestLogging(m =>
{
    m.IncludeQueryInRequestPath = true;
    m.EnrichDiagnosticContext = (context, httpContext) => { };
});
app.UseHttpsRedirection(hostInfo);
app.UseAuthorization();
app.MapDependencyHealthChecks();
app.MapControllers();
// app.MapStartPage(applicationInfo.Name);

app.Run();

public class Program
{
    public static void Main(string[] args)
    {
        // HostAsWeb.Run<Startup>(
        //     builder => builder.CustomizeBuilder(),
        //     s =>
        //     {
        //         if (s.Environment.IsDevelopment() || s.Environment.IsEnvironment("INT"))
        //             return s.ConfigureLogger<Startup>(c => c.UseSeq(s.Configuration.GetSection("Logging:Seq")));
        //
        //         return s.ConfigureLogger<Startup>(c =>
        //             c.UseLoggly(s.Configuration.GetSection("Logging:Loggly")));
        //     }, args);
    }
}

public class MappingProfile : Profile
{
}
