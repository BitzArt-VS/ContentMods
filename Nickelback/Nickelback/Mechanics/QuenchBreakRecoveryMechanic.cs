using HarmonyLib;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace BitzArt.Nickelback.Mechanics;

internal static class QuenchBreakRecoveryMechanic
{
    private static ModConfig Config => ModConfig.Instance!;

    private static readonly FieldInfo MetalGroupCodeField =
        AccessTools.Field(typeof(CollectibleBehaviorQuenchable), "metalGroupCode")
        ?? throw new MissingFieldException(nameof(CollectibleBehaviorQuenchable), "metalGroupCode");

    internal static void RecoverBits(
        CollectibleBehaviorQuenchable quenchableBehavior,
        IWorldAccessor world,
        ItemSlot slot,
        Vec3d pos)
    {
        if (slot.Itemstack is null)
        {
            return;
        }

        if (!TryGetRecoveredMetalVariant(quenchableBehavior, slot.Itemstack, out var metal))
        {
            return;
        }

        var bitItem = world.GetItem(new AssetLocation("game", $"metalbit-{metal}"));

        if (bitItem is null)
        {
            return;
        }

        var remainingBits = world.GetRandomValue(Config.RecoveredBits);

        while (remainingBits > 0)
        {
            var stackSize = world.GetRandomValue(Config.Scatter.StackSize);

            if (stackSize > remainingBits)
            {
                stackSize = remainingBits;
            }

            ItemStack stack = new(bitItem, stacksize: stackSize);

            world.SpawnItemEntity(stack, pos, GetScatterVelocity(world));

            remainingBits -= stackSize;
        }
    }

    private static bool TryGetRecoveredMetalVariant(
        CollectibleBehaviorQuenchable quenchableBehavior,
        ItemStack brokenStack,
        out string? metal)
    {
        if (MetalGroupCodeField.GetValue(quenchableBehavior) is not string metalVariantGroupCode)
        {
            metal = null;
            return false;
        }

        var originalMetalVariant = brokenStack.Collectible.Variant[metalVariantGroupCode];

        if (originalMetalVariant is null)
        {
            metal = null;
            return false;
        }

        return Config.RecoverMetal.TryGetValue(originalMetalVariant, out metal);
    }

    private static Vec3d GetScatterVelocity(IWorldAccessor world)
    {
        var angle = world.Rand.NextDouble() * Math.PI * 2;
        var horizontalVelocity = world.GetRandomValue(Config.Scatter.HorizontalVelocity);
        var verticalVelocity = world.GetRandomValue(Config.Scatter.VerticalVelocity);

        return new Vec3d(
            Math.Cos(angle) * horizontalVelocity,
            verticalVelocity,
            Math.Sin(angle) * horizontalVelocity);
    }
}
