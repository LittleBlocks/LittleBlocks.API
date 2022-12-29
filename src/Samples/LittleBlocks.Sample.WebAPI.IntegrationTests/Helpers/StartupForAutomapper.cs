// This software is part of the LittleBlocks framework
// Copyright (C) 2022 LittleBlocks
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

namespace LittleBlocks.Sample.WebAPI.IntegrationTests.Helpers;

public class StartupForAutomapper
{
    public StartupForAutomapper(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.BootstrapApp<StartupForAutomapper>(Configuration,
            app => app
                .HandleApplicationException<TemplateApiApplicationException>()
                .UseDetailedErrors().ConfigureMappings(c =>
                {
                    c.CreateMap<PersonEntity, PersonDo>();
                    c.CreateMap<AssetEntity, AssetDo>().ConvertUsing<AssetConverter>();
                })
                .AddServices((container, config) =>
                {
                    container.AddTransient<IRateProvider, DummyRateProvider>();
                    container.AddTransient<AssetConverter, AssetConverter>();
                    container.AddTransient<ITypeConverter<AssetEntity, AssetDo>, AssetConverter>();
                })
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseDefaultApiPipeline(Configuration, env, loggerFactory);
    }
}
