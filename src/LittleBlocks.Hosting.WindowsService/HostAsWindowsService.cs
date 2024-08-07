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

namespace LittleBlocks.Hosting.WindowsService;

public static class HostAsWindowsService
{
    // TODO: Should be configured with Http and Https for ease of redirect
    private static IHost Build<TStartup>(
        Func<IConfigurationBuilder, IConfigurationBuilder> configurationConfigure,
        Func<ILoggerBuilder, IBuildLogger> loggerConfigure, string[] args)
        where TStartup : class
    {
        if (loggerConfigure == null) throw new ArgumentNullException(nameof(loggerConfigure));

        var host = Host
            .CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<TStartup>();
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;
                var options = new ConfigurationOptions(env.ContentRootPath, env.EnvironmentName,
                    env.ApplicationName, args);
                config.ConfigureBuilder(options);
                configurationConfigure(config);
            })
            .ConfigureServices((context, services) =>
            {
                loggerConfigure(new LoggerBuilder(services, context.HostingEnvironment, context.Configuration)).Build<TStartup>();
            })
            .Build();

        return host;
    }

    public static void Run<TStartup>(Func<ILoggerBuilder, IBuildLogger> loggerConfigure) where TStartup : class
    {
        Run<TStartup>(c => c, loggerConfigure, new string[] { });
    }

    public static void Run<TStartup>(Func<ILoggerBuilder, IBuildLogger> loggerConfigure, string[] args)
        where TStartup : class
    {
        Run<TStartup>(c => c, loggerConfigure, args);
    }

    public static Task RunAsync<TStartup>(Func<ILoggerBuilder, IBuildLogger> loggerConfigure) where TStartup : class
    {
        return RunAsync<TStartup>(c => c, loggerConfigure, new string[] { });
    }

    public static Task RunAsync<TStartup>(Func<ILoggerBuilder, IBuildLogger> loggerConfigure, string[] args)
        where TStartup : class
    {
        return RunAsync<TStartup>(c => c, loggerConfigure, args);
    }

    public static void Run<TStartup>(
        Func<IConfigurationBuilder, IConfigurationBuilder> configurationConfigure,
        Func<ILoggerBuilder, IBuildLogger> loggerConfigure) where TStartup : class
    {
        Run<TStartup>(configurationConfigure, loggerConfigure, new string[] { });
    }

    public static void Run<TStartup>(
        Func<IConfigurationBuilder, IConfigurationBuilder> configurationConfigure,
        Func<ILoggerBuilder, IBuildLogger> loggerConfigure,
        string[] args)
        where TStartup : class
    {
        Build<TStartup>(configurationConfigure, loggerConfigure, args).Run();
    }

    public static Task RunAsync<TStartup>(
        Func<IConfigurationBuilder, IConfigurationBuilder> configurationConfigure,
        Func<ILoggerBuilder, IBuildLogger> loggerConfigure) where TStartup : class
    {
        return RunAsync<TStartup>(configurationConfigure, loggerConfigure, new string[] { });
    }

    public static Task RunAsync<TStartup>(
        Func<IConfigurationBuilder, IConfigurationBuilder> configurationConfigure,
        Func<ILoggerBuilder, IBuildLogger> loggerConfigure,
        string[] args)
        where TStartup : class
    {
        return Build<TStartup>(configurationConfigure, loggerConfigure, args).RunAsync();
    }
}
