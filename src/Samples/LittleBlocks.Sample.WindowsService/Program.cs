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

using ILogger = Serilog.ILogger;

namespace LittleBlocks.Sample.WindowsService;

class Program
{
    static void Main(string[] args)
    {
        var logger = ConfigureLogger();
        logger.Information("Start the service");
        try
        {
            HostAsWindowsService.Run<Startup>(c =>
                c.ConfigureLogger<Startup>(s => s.SaveLogsTo(BaseLogLocation)), args);
        }
        catch (Exception e)
        {
            logger.Error("{@e}", e);
            throw;
        }
    }

    private static ILogger ConfigureLogger()
    {
        var location = $@"{BaseLogLocation}\service-startup-{{Date}}.log";
        var logger = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .WriteTo.RollingFile(location)
            .CreateLogger();

        Log.Logger = logger;
        return logger;
    }

    private static string BaseLogLocation => $@"{AppContext.BaseDirectory}\logs";
}
