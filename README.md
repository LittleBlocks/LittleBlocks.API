# LittleBlocks

![Release](https://github.com/LittleBlocks/LittleBlocks.API/workflows/Release%20build%20on%20master/main/badge.svg) ![CI](https://github.com/LittleBlocks/LittleBlocks.API/workflows/CI%20on%20Branches%20and%20PRs/badge.svg) ![](https://img.shields.io/nuget/v/LittleBlocks.svg?style=flat-square)

## Introduction

LittleBlocks is a set of libraries which facilitate different aspects of a Restful/Microservice api and takes away the boilerplate configuration/bootstrap code that needs to be written each time a new API project is setup. There are up's and down's to this approach, however the benefit of this approach is to setup consistent api projects faster and easier.

## Features

Boilerplate api provides the following features:

* Global Error Handling
* Preconfigured Logging to a rolling file and log aggregartors such as loggly and seq
* Application/Common services preregistration within IOC container
* Request Correlation
* Preconfigured MVC pipeline
* FluentValidation framework integration
* AutoMapper integration with desired IOC container
* RestEase REST client integration
* Swagger integration
* Health endpoint exposed
* Diagnostics endpoint
* **Minimal API support** for modern .NET applications

For using the full benefit of the library, Create a simple asp.net core project and install *EasyApi.AspNetCore.Bootstrap* nuget package.

```cmd

dotnet add package LittleBlocks.AspNetCore.Bootstrap

or
 
Install-Package LittleBlocks.AspNetCore.Bootstrap

```

## Traditional Usage with Startup Class

In order to achieve all of this functionality you merely need a few lines of code. At this point your *Program.cs* should look like this:

```csharp

    public class Program
    {
        public static void Main(string[] args)
        {
            HostAsWeb.Run<Startup>(s =>  s.ConfigureLogger<Startup>());
        }
    }

```

 And *startup.cs* as follows:

 ```csharp

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BootstrapApp<Startup>(Configuration,
                            app => app
                                .HandleApplicationException<TemplateApiApplicationException>()
                                .AddServices((container, config) => { })
                        );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDefaultApiPipeline(Configuration, env, loggerFactory);
        }
    }

 ```

## Minimal API Usage

For minimal APIs, you can use the new simplified approach:

```csharp

using LittleBlocks.AspNetCore.Bootstrap;

var builder = WebApplication.CreateBuilder(args);

// Bootstrap LittleBlocks services
builder.BootstrapLittleBlocks(app => app
    .AddConfigSection<AppSettings>()
    .HandleApplicationException<MyApplicationException>()
    .ConfigureCorrelation(m => m.AutoCorrelateRequests())
    .ConfigureHealthChecks(c =>
    {
        c.AddUrlGroup(new Uri("http://www.google.com"), HttpMethod.Get, "google");
    })
    .AddServices((container, config) =>
    {
        container.AddScoped<IMyService, MyService>();
    })
);

var app = builder.Build();

// Configure the pipeline
app.UseLittleBlocksPipeline();

// Define your minimal API endpoints
app.MapGet("/api/hello", () => "Hello World!");

app.Run();

```

This provides the same rich feature set including global error handling, logging, health checks, authentication, CORS, and Swagger documentation, but with the simplified minimal API approach.

The project/solution is ready to be running in visual studio or using dotnet cli.

More detail information can be found in [wiki](https://github.com/LittleBlocks/LittleBlocks.API/wiki)

