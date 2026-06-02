using Newtonsoft.Json;

namespace BitzArt.Nickelback;

public record ScatterConfig
{
    [JsonProperty("stackSize", Order = 1)]
    public RangeConfig<int> StackSize { get; set; } = new(1, 4);

    [JsonProperty("horizontalVelocity", Order = 2)]
    public RangeConfig<double> HorizontalVelocity { get; set; } = new(0.01, 0.08);

    [JsonProperty("verticalVelocity", Order = 3)]
    public RangeConfig<double> VerticalVelocity { get; set; } = new(0.01, 0.10);
}
