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

namespace LittleBlocks.Bootstrap;

public abstract class ContainerFactory<TContainer> where TContainer : class
{
    private readonly Action<TContainer, IConfiguration> _serviceConfigurationProvider;

    protected ContainerFactory(Action<TContainer, IConfiguration> serviceConfigurationProvider)
    {
        _serviceConfigurationProvider = serviceConfigurationProvider ??
                                        throw new ArgumentNullException(nameof(serviceConfigurationProvider));
    }

    protected abstract void ConfigureContainer(
        Action<TContainer, IConfiguration> serviceConfigurationProvider,
        IServiceCollection services,
        IConfiguration configuration);

    public void Create(IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        ConfigureContainer(_serviceConfigurationProvider, services, configuration);
    }
}
