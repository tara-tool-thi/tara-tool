namespace tara_tool.Data.Tabels;

public class Asset
{
    public long Id { get; set; }
    public long AssetNumber { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public virtual ICollection<Tag> AssetGroup { get; set; } = [];
    public virtual ICollection<DamageScenario> DamageScenarios { get; set; } = [];

    // Due to the many to many nature, we need to take care that there are no empty/unattached assets flying around
    public virtual ICollection<ItemDefinition> ItemDefinitions { get; set; } = [];
}