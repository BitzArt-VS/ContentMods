using Vintagestory.API.Common;

namespace BitzArt.Nickelback;

internal static class GetModConfigExtension
{
    public static T GetModConfig<T>(this ICoreAPI api, string filename)
        where T : class, new()
    {
        try
        {
            var config = api.LoadModConfig<T>(filename) ?? CreateModConfig<T>(api, filename);
            api.StoreModConfig(config, filename);

            return config;
        }
        catch (Exception ex)
        {
            api.Logger.Error($"Failed to load mod config from file '{filename}'.");
            api.Logger.Error(ex);

            return CreateModConfig<T>(api, filename);
        }
    }

    private static T CreateModConfig<T>(this ICoreAPI api, string filename)
        where T : class, new()
    {
        api.Logger.Warning($"Creating new mod config file '{filename}'.");

        T config = new();
        api.StoreModConfig(config, filename);

        return config;
    }
}
