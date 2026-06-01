using HarmonyLib;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace BitzArt.Nickelback.Mechanics;

internal static class QuenchBreakRecoveryMechanic
{
    private const int BitsPerItem = 20;

    private const double MaxRecoveryRatio = 1;
    private const double MaxLossRatio = 0.1;
    private const double MinRecoveryRatio = MaxRecoveryRatio - MaxLossRatio;

    private const double MinScatterHorizontalVelocity = 0.01;
    private const double MaxScatterHorizontalVelocity = 0.08;
    private const double MinScatterVerticalVelocity = 0.01;
    private const double MaxScatterVerticalVelocity = 0.10;

    private const int MinDroppedBitStackSize = 1;
    private const int MaxDroppedBitStackSize = 4;

    private static readonly int MinRecoveredBits = (int)Math.Floor(BitsPerItem * MinRecoveryRatio);
    private static readonly int MaxRecoveredBits = (int)Math.Floor(BitsPerItem * MaxRecoveryRatio);

    private static readonly FieldInfo MetalGroupCodeField =
        AccessTools.Field(typeof(CollectibleBehaviorQuenchable), "metalGroupCode")
        ?? throw new MissingFieldException(nameof(CollectibleBehaviorQuenchable), "metalGroupCode");

    private static readonly Dictionary<string, string> RecoveredMetalByOriginalMetal = new()
    {
        ["iron"] = "iron",
        ["steel"] = "iron",
        ["meteoriciron"] = "meteoriciron",
        ["meteoricsteel"] = "meteoriciron",
    };

    internal static void Recover(
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

        var recoveredBitCount = world.Rand.Next(MinRecoveredBits, MaxRecoveredBits + 1);
        var remainingBitCount = recoveredBitCount;

        while (remainingBitCount > 0)
        {
            var droppedStackSize = Math.Min(
                world.Rand.Next(MinDroppedBitStackSize, MaxDroppedBitStackSize + 1),
                remainingBitCount);

            var bitStack = new ItemStack(bitItem)
            {
                StackSize = droppedStackSize
            };

            remainingBitCount -= droppedStackSize;
            world.SpawnItemEntity(bitStack, pos, GetScatterVelocity(world));
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

        return RecoveredMetalByOriginalMetal.TryGetValue(originalMetalVariant, out metal);
    }

    private static Vec3d GetScatterVelocity(IWorldAccessor world)
    {
        var angle = world.Rand.NextDouble() * Math.PI * 2;
        var horizontalVelocity = GetRandomDouble(
            world,
            MinScatterHorizontalVelocity,
            MaxScatterHorizontalVelocity);
        var verticalVelocity = GetRandomDouble(
            world,
            MinScatterVerticalVelocity,
            MaxScatterVerticalVelocity);

        return new Vec3d(
            Math.Cos(angle) * horizontalVelocity,
            verticalVelocity,
            Math.Sin(angle) * horizontalVelocity);
    }

    private static double GetRandomDouble(IWorldAccessor world, double min, double max)
    {
        return min + world.Rand.NextDouble() * (max - min);
    }
}
