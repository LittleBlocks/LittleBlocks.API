﻿namespace LittleBlocks.Sample.Minimal.WebAPI.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder CustomizeBuilder(this IConfigurationBuilder builder)
    {
        return builder ?? throw new ArgumentNullException(nameof(builder));
    }
}
