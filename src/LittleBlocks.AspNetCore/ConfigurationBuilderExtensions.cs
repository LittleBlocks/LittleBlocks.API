﻿// This software is part of the LittleBlocks framework
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

namespace LittleBlocks.AspNetCore;

public static class ConfigurationBuilderExtensions
{
    public const string EnvironmentVariablePrefix = "LITTLEBLOCKS_";
    public static IConfigurationBuilder ConfigureBuilder(this IConfigurationBuilder builder, ConfigurationOptions options)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        if (options == null) throw new ArgumentNullException(nameof(options));

        builder
            .AddJsonFile(Path.Combine(options.BasePath, "appsettings.json"), false, true)
            .AddJsonFile(Path.Combine(options.BasePath, $"appsettings.{options.Environment}.json"), true, true);

        if (options.IsDevelopment)
        {
            var appAssembly = Assembly.Load(new AssemblyName(options.AppEntry));
            builder.AddUserSecrets(appAssembly, optional: true);
        }

        builder
            .AddEnvironmentVariables(EnvironmentVariablePrefix)
            .AddCommandLine(options.Args);

        return builder;
    }
}
