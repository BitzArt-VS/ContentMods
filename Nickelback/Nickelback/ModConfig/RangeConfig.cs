using Newtonsoft.Json;

namespace BitzArt.Nickelback;

public record RangeConfig<T>
    where T : struct
{
    [JsonProperty("min", Order = 1)]
    public T Min { get; set; }

    [JsonProperty("max", Order = 2)]
    public T Max { get; set; }

    public RangeConfig(T min, T max)
    {
        Min = min;
        Max = max;
    }

    public RangeConfig() { }
}
