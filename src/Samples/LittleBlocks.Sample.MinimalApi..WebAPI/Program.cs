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
using AutoMapper.Features;
using FluentValidation.AspNetCore;
using LittleBlocks.AspNetCore.Documentation;
using LittleBlocks.AspNetCore.RequestCorrelation;
using LittleBlocks.Sample.Minimal.WebAPI.Extensions;
using Serilog;

namespace LittleBlocks.Sample.Minimal.WebAPI;

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
builder.Services.AddFeatures<Features<>>(builder.Configuration);

builder.Services
    .AddSingleton(applicationInfo)
    .AddRouting(o => o.LowercaseUrls = true)
    .AddMediatR(typeof(Program))
    .AddEndpointsApiExplorer()
    .AddOpenApiDocumentation(applicationInfo)
    .AddRequestCorrelation()
    .AddCorsWithDefaultPolicy()
    .AddTypeMapping(c => c.AddProfile<MappingProfile>());

var app = builder.Build();
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
app.MapStartPage(applicationInfo.Name);

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
