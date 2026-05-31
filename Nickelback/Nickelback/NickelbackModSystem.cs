using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;

namespace BitzArt.Nickelback;

public sealed class NickelbackModSystem : ModSystem
{
    internal const string ModId = "bitzartnickelback";

    internal static ILogger Logger { get; private set; } = null!;
    internal static ICoreAPI Api { get; private set; } = null!;

    private Harmony? harmony;

    public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Server;

    public override void Start(ICoreAPI api)
    {
        Api = api;
        Logger = Mod.Logger;

        harmony ??= new Harmony(ModId);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public override void Dispose()
    {
        harmony?.UnpatchAll(ModId);
        harmony = null;

        Api = null!;
        Logger = null!;

        base.Dispose();
    }
}
