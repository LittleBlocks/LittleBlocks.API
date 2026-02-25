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

public sealed class MinimalApiBootstrapOptions
{
    private readonly List<Action<ConfigurationOptionBuilder>> _configurationSections =
        new List<Action<ConfigurationOptionBuilder>>();

    public ApiBootstrapperFeatures Features { get; set; } = ApiBootstrapperFeatures.MinimalDefaults;

    public LevelOfDetails ErrorDetails { get; set; } = LevelOfDetails.StandardMessage;

    public Func<IExcludeRequests, IBuildOptions> CorrelationOptions { get; set; } =
        builder => builder.EnforceCorrelation();

    public Action<IHealthChecksBuilder> ConfigureHealthChecks { get; set; }

    public Action<IServiceCollection, IConfiguration> ConfigureServices { get; set; }

    public Action<IEndpointRouteBuilder> ConfigureEndpoints { get; set; }

    public Action<IEndpointRouteBuilder> PostConfigureEndpoints { get; set; }

    public bool EnableStartPage { get; set; } = true;

    internal IReadOnlyList<Action<ConfigurationOptionBuilder>> ConfigurationSections => _configurationSections;

    internal AppInfo AppInfo { get; set; }

    internal AuthOptions AuthOptions { get; set; }

    public MinimalApiBootstrapOptions AddSection<TSection>() where TSection : class, new()
    {
        _configurationSections.Add(builder => builder.AddSection<TSection>());
        return this;
    }

    public MinimalApiBootstrapOptions AddSection<TSection>(string sectionName) where TSection : class, new()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);
        _configurationSections.Add(builder => builder.AddSection<TSection>(sectionName));
        return this;
    }
}
