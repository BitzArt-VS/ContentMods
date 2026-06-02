using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace BitzArt.Nickelback;

public record ModConfig
{
    private const string FileName = "Nickelback.json";

    [JsonProperty("bitsPerItem", Order = 1)]
    public int BitsPerItem { get; set; } = 20;

    [JsonProperty("recoveryRatio", Order = 2)]
    public RangeConfig<double> RecoveryRatio { get; set; } = new(0.9, 1.0);

    [JsonProperty("scatter", Order = 3)]
    public ScatterConfig Scatter { get; set; } = new();

    [JsonProperty("recoverMetal", Order = 5, NullValueHandling = NullValueHandling.Ignore, ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public Dictionary<string, string> RecoverMetal = new()
    {
        ["iron"] = "iron",
        ["steel"] = "iron",
        ["meteoriciron"] = "meteoriciron",
        ["meteoricsteel"] = "meteoriciron",
    };

    [JsonIgnore]
    internal RangeConfig<int> RecoveredBits { get; private set; } = null!;

    public static ModConfig Instance { get; private set; } = null!;

    internal static void Load(ICoreAPI api)
    {
        var config = api.GetModConfig<ModConfig>(FileName);
        config.Initialize();
        Instance = config;
    }

    private void Initialize()
    {
        RecoveredBits = new((int)Math.Floor(BitsPerItem * RecoveryRatio.Min), (int)Math.Floor(BitsPerItem * RecoveryRatio.Max));
    }
}
