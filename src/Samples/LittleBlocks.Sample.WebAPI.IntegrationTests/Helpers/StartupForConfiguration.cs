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

using Microsoft.Extensions.Hosting;

namespace LittleBlocks.Sample.WebAPI.IntegrationTests.Helpers;

public class StartupForConfiguration(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.BootstrapApp<StartupForIntegration>(Configuration,
            app =>
                app.AddConfigSection<Clients>()
                    .HandleApplicationException<TemplateApiApplicationException>()
                    .AddServices((container, config) => { })
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, ILoggerFactory loggerFactory)
    {
        app.UseDefaultApiPipeline(Configuration, env, lifetime, loggerFactory);
    }
}
