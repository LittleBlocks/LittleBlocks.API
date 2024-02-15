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

using Microsoft.AspNetCore;

namespace LittleBlocks.Testing.Integration;

public class IntegrationTestApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
 
    protected override IWebHostBuilder CreateWebHostBuilder()
    {
        var hostBuilder = WebHost.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                var env = context.HostingEnvironment;
                env.EnvironmentName = "Development";

                builder.SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", false, true);
            })
            .UseStartup<TStartup>()
            .UseTestServer();

        return hostBuilder;
    }
}
