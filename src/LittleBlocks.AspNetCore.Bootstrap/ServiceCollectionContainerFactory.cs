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

namespace LittleBlocks.AspNetCore.Bootstrap;

public sealed class
    ServiceCollectionContainerFactory : ContainerFactory<IServiceCollection>
{
    public ServiceCollectionContainerFactory(
        Action<IServiceCollection, IConfiguration> serviceConfigurationProvider) : base(
        serviceConfigurationProvider)
    {
    }

    protected override void ConfigureContainer(
        Action<IServiceCollection, IConfiguration> serviceConfigurationProvider,
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IComponentResolver, ServiceProviderComponentResolver>();
        serviceConfigurationProvider(services, configuration);
    }
}
