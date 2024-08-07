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

namespace LittleBlocks.AspNetCore.Mvc;

public static class MvcServiceCollectionExtensions
{
    private const string AssemblyNameSectionSeparator = ".";

    public static IServiceCollection AddDefaultMvc<TStartup>(this IServiceCollection services)
        where TStartup : class
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var prefix = typeof(TStartup).GetAssemblyPrefix(AssemblyNameSectionSeparator);
        services.AddControllers(options =>
            {
                options.Filters.Add(typeof(LoggingActionFilter));
                options.Filters.Add(typeof(ValidateModelStateActionFilter));
            })
            .AddNewtonsoftJson(o => o.SerializerSettings.ConfigureJsonSettings());

        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        services.RegisterValidatorsFromDomain<TStartup>(prefix);
        return services;
    }

    private static void RegisterValidatorsFromDomain<T>(this IServiceCollection services,
        string assemblyNameStartsWith) where T : class
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(assemblyNameStartsWith))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(assemblyNameStartsWith));

        var assemblies = GetReferencedAssembliesFromType<T>(assemblyNameStartsWith);
        services.AddValidatorsFromAssemblies(assemblies);
    }

    private static IEnumerable<Assembly> GetReferencedAssembliesFromType<T>(string assemblyNameStartsWith)
    {
        var type = typeof(T);
        return type.GetReferencedAssemblies(assemblyNameStartsWith);
    }
}
