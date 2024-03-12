// namespace LittleBlocks.AspNetCore.Bootstrap.Extensions;
//
// public static class WebHostBuilderExtensions
// {
//     public static void ConfigureAzureKeyVault(this IWebHostBuilder builder,
//         Action<AzureKeyVaultOptions>? configure = null)
//     {
//         if (builder == null) throw new ArgumentNullException(nameof(builder));
//
//         var options = new AzureKeyVaultOptions();
//         configure?.Invoke(options);
//
//         builder.ConfigureAppConfiguration((ctx, c) => { c.ConfigureAzureKeyVault(options); });
//     }
// }