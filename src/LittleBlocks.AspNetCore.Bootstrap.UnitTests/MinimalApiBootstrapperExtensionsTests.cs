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

namespace LittleBlocks.AspNetCore.Bootstrap.UnitTests;

public class MinimalApiBootstrapperExtensionsTests
{
    [Fact]
    public void Should_Register_Default_MinimalApi_Services()
    {
        // Arrange
        var builder = CreateBuilder();

        // Act
        var options = builder.BootstrapMinimalApi();
        var provider = builder.Services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<ICorrelationIdProvider>());
        Assert.NotNull(provider.GetService<MinimalApiBootstrapOptions>());

        var application = provider.GetRequiredService<IOptions<Application>>().Value;
        Assert.Equal("TestApp", application.Name);
        Assert.Equal("v1", application.Version);

        Assert.Equal(ApiBootstrapperFeatures.MinimalDefaults, options.Features);
    }

    [Fact]
    public void Should_Respect_Feature_Flags_When_Disabling_Correlation()
    {
        // Arrange
        var builder = CreateBuilder();

        // Act
        builder.BootstrapMinimalApi(o =>
        {
            o.Features &= ~ApiBootstrapperFeatures.RequestCorrelation;
        });
        var provider = builder.Services.BuildServiceProvider();

        // Assert
        Assert.Null(provider.GetService<ICorrelationIdProvider>());
    }

    [Fact]
    public void Should_Bind_Additional_Configuration_Sections()
    {
        // Arrange
        var builder = CreateBuilder(new Dictionary<string, string>
        {
            {"CustomOptions:Enabled", "true"}
        });

        // Act
        builder.BootstrapMinimalApi(o => o.AddSection<CustomOptions>("CustomOptions"));
        var provider = builder.Services.BuildServiceProvider();

        // Assert
        var options = provider.GetRequiredService<IOptions<CustomOptions>>().Value;
        Assert.True(options.Enabled);
    }

    private static WebApplicationBuilder CreateBuilder(Dictionary<string, string> additionalSettings = null)
    {
        var settings = new Dictionary<string, string>
        {
            {"Application:Name", "TestApp"},
            {"Application:Version", "v1"},
            {"Application:Environment:Name", "Development"},
            {"AuthOptions:AuthenticationMode", "None"}
        };

        if (additionalSettings != null)
        {
            foreach (var pair in additionalSettings)
                settings[pair.Key] = pair.Value;
        }

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development,
            ContentRootPath = Directory.GetCurrentDirectory()
        });

        builder.Configuration.AddInMemoryCollection(settings);
        return builder;
    }

    private sealed class CustomOptions
    {
        public bool Enabled { get; set; }
    }
}
