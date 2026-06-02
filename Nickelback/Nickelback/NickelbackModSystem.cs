using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;

namespace BitzArt.Nickelback;

public sealed class NickelbackModSystem : ModSystem
{
    internal const string ModId = "bitzartnickelback";

    private Harmony? _harmony;

    public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Server;

    public override void Start(ICoreAPI api)
    {
        try
        {
            if (_harmony is not null)
            {
                throw new UnreachableException("Harmony is already initialized. This should never happen.");
            }

            ModConfig.Load(api);

            _harmony = new Harmony(ModId);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            api.Logger.Error("Nickelback: Failed to initialize. Mod will not start.");
            api.Logger.Error(ex);

            return;
        }
    }

    public override void Dispose()
    {
        _harmony?.UnpatchAll(ModId);
        _harmony = null;

        base.Dispose();
    }
}
