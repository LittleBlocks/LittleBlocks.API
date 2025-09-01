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

/// <summary>
/// Extension methods for WebApplicationBuilder to support LittleBlocks minimal API configuration.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Bootstraps the application services using LittleBlocks configuration for minimal APIs.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance</param>
    /// <param name="appBootstrapperProvider">Configuration function for the application bootstrapper</param>
    /// <returns>The configured WebApplicationBuilder</returns>
    public static WebApplicationBuilder BootstrapLittleBlocks(
        this WebApplicationBuilder builder,
        Func<IConfigureApplicationBootstrapper, IBootstrapApplication> appBootstrapperProvider)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(appBootstrapperProvider);

        // Create a type placeholder for the bootstrapper since minimal APIs don't have a startup class
        var bootstrapper = appBootstrapperProvider(new MinimalApiBootstrapper(builder.Services, builder.Configuration));
        bootstrapper.Bootstrap();

        return builder;
    }
}