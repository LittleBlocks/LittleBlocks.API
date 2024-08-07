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

namespace LittleBlocks.AspNetCore.Logging.SeriLog.Fluent;

public sealed class LoggerBuilder(
    IServiceCollection services, 
    IHostEnvironment environment,
    IConfiguration configuration)
    : ILoggerBuilder, IBuildLogger
{
    private const string LogMessageTemplate =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{MachineName}] [{EnvironmentUserName}] [{ProcessId}] " +
        "[{UserName}] [{CorrelationId}] [{ThreadId}] [{Level}] {Message}{NewLine}{Exception}";

    private const string SerilogConfigSectionName = "Logging:Serilog";
    private const string SerilogMinimumLevelKey = "MinimumLevel";
    private const LogEventLevel DefaultLogLevel = LogEventLevel.Information;
    
    private readonly IServiceCollection _services = services ?? throw new ArgumentNullException(nameof(services));
    private readonly IHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    private Func<ISetFileSizeLimit, IBuildSeriLogOptions> _optionsProvider;
    private Func<ISinkBuilderContext, ISinkBuilderContext> _sinksProvider;

    public void Build<TStartup>() where TStartup : class
    {
        _services.AddSerilog(lg => ConfigureLogger<TStartup>(lg, _optionsProvider, _sinksProvider));
    }
    public IConfiguration Configuration => _configuration;
    public IHostEnvironment Environment => _environment;

    public IBuildLogger ConfigureLogger<TStartup>() where TStartup : class
    {
        _sinksProvider = null;
        _optionsProvider = provider => provider;

        return this;
    }

    public IBuildLogger ConfigureLogger<TStartup>(Func<ISinkBuilderContext, ISinkBuilderContext> sinksProvider)
        where TStartup : class
    {
        _sinksProvider = sinksProvider;
        _optionsProvider = provider => provider;

        return this;
    }

    public IBuildLogger ConfigureLogger<TStartup>(Func<ISetFileSizeLimit, IBuildSeriLogOptions> optionsProvider,
        Func<ISinkBuilderContext, ISinkBuilderContext> sinksProvider) where TStartup : class
    {
        _optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
        _sinksProvider = sinksProvider;

        return this;
    }

    public IBuildLogger ConfigureLogger<TStartup>(Func<ISetFileSizeLimit, IBuildSeriLogOptions> optionsProvider)
        where TStartup : class
    {
        _optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
        _sinksProvider = null;

        return this;
    }

    private void ConfigureLogger<TStartup>(LoggerConfiguration loggerConfiguration,
        Func<ISetFileSizeLimit, IBuildSeriLogOptions> optionsProvider,
        Func<ISinkBuilderContext, ISinkBuilderContext> sinksProvider) where TStartup : class
    {
        ArgumentNullException.ThrowIfNull(loggerConfiguration);

        var env = _environment;
        var configuration = _configuration;
        var applicationInfo = configuration.GetApplicationInfo();

        var optionsBuilder = new SeriLogOptionsBuilder(configuration);
        var options = optionsProvider(optionsBuilder).Build();
        var assemblyName = typeof(TStartup).GetTypeInfo().Assembly.GetName().Name;
        var environmentName = applicationInfo.Environment ?? env.EnvironmentName;
        var logFilePath = GetLogFilePath(env, options);

        loggerConfiguration
            .MinimumLevel.ControlledBy(LoggingLevelSwitchProvider.Instance)
            .IgnoreSystemLogs()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithApplicationName(assemblyName)
            .Enrich.WithEnvironmentName(environmentName)
            .WriteTo.RollingFile(Path.Combine(logFilePath, $"{assemblyName}-{environmentName}-{{Date}}.log"),
                outputTemplate: LogMessageTemplate,
                retainedFileCountLimit: options.LogFilesToRetain,
                fileSizeLimitBytes: options.LogFileSizeLimitInBytes,
                flushToDiskInterval: options.FlushToDiskInterval)
            .WriteTo.Console()
            .WriteTo.InMemoryCache();

        LoggingLevelSwitchProvider.Instance.MinimumLevel =
            GetMinimumLogLevelOrUseDefault(configuration, DefaultLogLevel);

        sinksProvider?.Invoke(new SinkBuilderContext(loggerConfiguration, env));
    }

    private static string GetLogFilePath(IHostEnvironment env, LoggingOptions options)
    {
        var defaultPath = env.IsDevelopment() ? $"{env.ContentRootPath}\\logs" : "C:\\logs";
        return options.LogsPathSet ? options.LogsPath : defaultPath;
    }

    private static LogEventLevel GetMinimumLogLevelOrUseDefault(IConfiguration configuration,
        LogEventLevel defaultLogEventLevel)
    {
        try
        {
            var config = configuration.GetSection(SerilogConfigSectionName);
            var minimumLogLevelToParse = config[SerilogMinimumLevelKey];
            return (LogEventLevel) Enum.Parse(typeof(LogEventLevel), minimumLogLevelToParse);
        }
        catch (ArgumentException)
        {
            return defaultLogEventLevel;
        }
    }
}
