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

using System.Net.Http;

namespace LittleBlocks.Sample.WebAPI.IntegrationTests.Helpers;

public abstract class StartupForHealth<T> where T: class
{
    protected StartupForHealth(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    protected virtual Action<IHealthChecksBuilder> ConfigureHealth => o => { };

    public void ConfigureServices(IServiceCollection services)
    {
        services.BootstrapApp<T>(Configuration,
            app => app.AddConfigSection<Clients>()
                .AndSection<Section1>()
                .AndSection<Section2>()
                .HandleApplicationException<TemplateApiApplicationException>()
                .AndHandle<ThirdPartyPluginException>()
                .UseUserErrors()
                .ConfigureHealthChecks(ConfigureHealth)
                .AddServices((container, config) =>
                {
                    container.AddRestClient<IValuesClient, Clients>(c => c.ProducerClientUrl, o => o.ExcludeAuthorizationHeader());
                    container.TryAddTransient<IMyService, MyService>();
                })
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseDefaultApiPipeline(Configuration, env, loggerFactory);
    }
}

public sealed class StartupForHealthy : StartupForHealth<StartupForHealthy>
{
    public StartupForHealthy(IConfiguration configuration) : base(configuration)
    {
    }

    protected override Action<IHealthChecksBuilder> ConfigureHealth =>
        b => b.AddUrlGroup(new Uri("http://httpbin.org/status/200"), HttpMethod.Get, "microsoft");
}

public sealed class StartupForUnhealthy : StartupForHealth<StartupForUnhealthy>
{
    public StartupForUnhealthy(IConfiguration configuration) : base(configuration)
    {
    }

    protected override Action<IHealthChecksBuilder> ConfigureHealth =>
        b =>
        {
            b.AddUrlGroup(new Uri("http://httpbin.org/status/200"), HttpMethod.Get, "microsoft");
            b.AddUrlGroup(new Uri("http://httpbintest.org/status/200"), HttpMethod.Get, "microsoftwithwrongurl");
        };
}
