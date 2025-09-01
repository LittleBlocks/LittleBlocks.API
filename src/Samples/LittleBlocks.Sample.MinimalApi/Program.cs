using LittleBlocks.AspNetCore.Bootstrap;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Bootstrap LittleBlocks services
builder.BootstrapLittleBlocks(app => app
    .AddConfigSection<AppSettings>()
    .HandleApplicationException<MyApplicationException>()
    .ConfigureCorrelation(m => m.AutoCorrelateRequests())
    .ConfigureHealthChecks(c =>
    {
        c.AddUrlGroup(new Uri("http://www.google.com"), HttpMethod.Get, "google");
        c.AddUrlGroup(new Uri("https://github.com/littleblocks"), HttpMethod.Get, "LittleBlocks");
    })
    .AddServices((container, config) =>
    {
        // Add custom services here
        container.AddScoped<IGreetingService, GreetingService>();
    })
);

var app = builder.Build();

// Configure the pipeline
app.UseLittleBlocksPipeline();

// Define minimal API endpoints
app.MapGet("/api/greeting", (IGreetingService greetingService) => 
    greetingService.GetGreeting());

app.MapGet("/api/greeting/{name}", (string name, IGreetingService greetingService) => 
    greetingService.GetGreeting(name));

app.MapPost("/api/greeting", (GreetingRequest request, IGreetingService greetingService) => 
    greetingService.CreateGreeting(request));

app.Run();

// Sample models and services for demonstration
public class AppSettings
{
    public string DefaultGreeting { get; set; } = "Hello";
}

public class GreetingRequest
{
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class GreetingResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public interface IGreetingService
{
    GreetingResponse GetGreeting();
    GreetingResponse GetGreeting(string name);
    GreetingResponse CreateGreeting(GreetingRequest request);
}

public class GreetingService : IGreetingService
{
    private readonly AppSettings _settings;
    private readonly ILogger<GreetingService> _logger;

    public GreetingService(IOptions<AppSettings> settings, ILogger<GreetingService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public GreetingResponse GetGreeting()
    {
        _logger.LogInformation("Getting default greeting");
        return new GreetingResponse
        {
            Message = $"{_settings.DefaultGreeting}, World!",
            Timestamp = DateTime.UtcNow
        };
    }

    public GreetingResponse GetGreeting(string name)
    {
        _logger.LogInformation("Getting greeting for {Name}", name);
        return new GreetingResponse
        {
            Message = $"{_settings.DefaultGreeting}, {name}!",
            Timestamp = DateTime.UtcNow
        };
    }

    public GreetingResponse CreateGreeting(GreetingRequest request)
    {
        _logger.LogInformation("Creating custom greeting for {Name}", request.Name);
        return new GreetingResponse
        {
            Message = $"{request.Message}, {request.Name}!",
            Timestamp = DateTime.UtcNow
        };
    }
}

public class MyApplicationException : Exception
{
    public MyApplicationException(string message) : base(message) { }
    public MyApplicationException(string message, Exception innerException) : base(message, innerException) { }
}
