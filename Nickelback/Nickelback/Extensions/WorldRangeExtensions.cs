using Vintagestory.API.Common;

namespace BitzArt.Nickelback;

public static class WorldRangeExtensions
{
    extension(IWorldAccessor world)
    {
        public int GetRandomValue(RangeConfig<int> range, bool includeMax = true)
        => GetRandomValue(world, range.Min, range.Max, includeMax);

        public int GetRandomValue(int min, int max, bool includeMax = true)
            => includeMax switch
            {
                true => world.Rand.Next(min, max + 1),

                false => world.Rand.Next(min, max)
            };

        public double GetRandomValue(RangeConfig<double> range)
            => GetRandomValue(world, range.Min, range.Max);

        public double GetRandomValue(double min, double max)
            => world.Rand.NextDouble() * (max - min) + min;

        public double GetRandomValue(RangeConfig<float> range)
            => GetRandomValue(world, range.Min, range.Max);

        public double GetRandomValue(float min, float max)
            => world.Rand.NextSingle() * (max - min) + min;
    }
}
