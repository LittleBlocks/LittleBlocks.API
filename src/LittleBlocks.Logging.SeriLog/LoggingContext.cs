using LittleBlocks.Configurations;
using Microsoft.Extensions.Hosting;

namespace LittleBlocks.Logging.SeriLog;

public record LoggingContext(AppInfo AppInfo, string[] Tags)
{
    public string Host { get; init; } = "";
    public string ExcludeEventFilter { get; init; } = "";
    public bool EnableDebugging { get; init; } = false;
    public string DefaultFilePath { get; init; } = @"logs\log.txt";
    public LogEventLevel DefaultLogLevel { get; init; } = LogEventLevel.Information;

    public bool IsDevelopment()
    {
        return AppInfo?.Environment == Environments.Development;
    }
}