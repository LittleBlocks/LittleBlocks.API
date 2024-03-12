using LittleBlocks.Logging.SeriLog;

namespace LittleBlocks.AspNetCore.Bootstrap.Extensions;

public static class ConfigurationExtensions
{
    public static AppInfo GetApplicationInfo(this WebApplicationBuilder builder)
    {
        var appName = builder.Configuration["Application:Name"] ?? builder.Environment.ApplicationName;
        var appVersion = builder.Configuration["Application:Version"] ?? "1.0.0";
        var appDescription = builder.Configuration["Application:Description"] ?? "";
        var appEnvironment = builder.Environment.EnvironmentName;
        return new AppInfo(appName, appVersion, appEnvironment, appDescription);
    }

    public static LoggingContext GetLoggingContext(this WebApplicationBuilder builder)
    {
        var applicationInfo = builder.GetApplicationInfo();
        return new LoggingContext(applicationInfo, applicationInfo.ToTags())
        {
            Host = "ASPNetCore",
            ExcludeEventFilter = "ClientAgent = 'AlwaysOn' or (RequestPath = '/health' and StatusCode < 400)"
        };
    }

    public static HostInfo GetHostInfo(this WebApplicationBuilder builder)
    {
        var hostType = Enum.TryParse(typeof(HostType), builder.Configuration["Host:Type"], out var result)
            ? (HostType) result
            : HostType.Kestrel;
        return new HostInfo(hostType, hostType == HostType.Kestrel);
    }

    private static string[] ToTags(this AppInfo appInfo)
    {
        ArgumentNullException.ThrowIfNull(appInfo);

        return new[]
        {
            $"app:{appInfo.Name}",
            $"version:{appInfo.Version}",
            $"env:{appInfo.Environment}"
        };
    }
}