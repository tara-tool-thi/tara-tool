namespace tara_tool.Data.Tabels;

public class Asset
{
    public long Id { get; set; }
    public long AssetNumber { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public ICollection<Tag> AssetGroup { get; set; } = [];
    public ICollection<DamageScenario> DamageScenarios { get; set; } = [];

    // Due to the many to many nature, we need to take care that there are no empty/unattached assets flying around
    public ICollection<ItemDefinition> ItemDefinitions { get; set; } = [];
}