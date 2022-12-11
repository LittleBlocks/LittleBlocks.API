// This software is part of the LittleBlocks framework
// Copyright (C) 2022 Little Blocks
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

namespace LittleBlocks.Sample.WebAPI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.BootstrapApp<Startup>(Configuration,
            app => app
                .AddConfigSection<Clients>()
                .AndSection<Section1>()
                .AndSection<Section2>()
                .HandleApplicationException<TemplateApiApplicationException>()
                .ConfigureCorrelation(m => m.AutoCorrelateRequests())
                .ConfigureHealthChecks(c =>
                {
                    c.AddUrlGroup(new Uri("http://www.google.com"), HttpMethod.Get, "google");
                    c.AddUrlGroup(new Uri("http://www.Microsoft.com"), HttpMethod.Get, "microsoft");
                    c.AddUrlGroup(new Uri("http://www.LittleBlocks.com"), HttpMethod.Get, "LittleBlocks");
                    c.AddSeqPublisher(setup =>
                    {
                        setup.Endpoint = Configuration["seq:ServerUrl"];
                        setup.ApiKey = Configuration["seq:ApiKey"];
                    });
                })
                .ConfigureMappings(c =>
                {
                    c.CreateMap<PersonEntity, PersonDo>();
                    c.CreateMap<AssetEntity, AssetDo>().ConvertUsing<AssetConverter>();
                })
                .AddServices((container, config) =>
                {
                    container.AddRestClient<IValuesClient, Clients>(c => c.ProducerClientUrl);
                    container.AddTransientWithInterception<IMyService, MyService>(by =>
                        by.InterceptBy<LogInterceptor>());
                    container.AddTransientWithInterception<IRateProvider, DummyRateProvider>(by =>
                        by.InterceptBy<LogInterceptor>());
                    container.AddSingleton<ITypeConverter<AssetEntity, AssetDo>, AssetConverter>();
                    services
                        .AddHealthChecksUI()
                        .AddInMemoryStorage();
                })
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseDefaultApiPipeline(Configuration, env, loggerFactory);
        app.UseEndpoints(config =>
        {
            config.MapHealthChecksUI(m => m.AddCustomStylesheet("health-ui.css"));
        });
    }
}
