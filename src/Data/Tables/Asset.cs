namespace tara_tool.Data.Tables;

public class Asset
{
    public long Id { get; set; }
    public long AssetNumber { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public long IdTag { get; set; }
    public virtual Tag? Tag { get; set; }
    public virtual ICollection<DamageScenario> DamageScenarios { get; set; } = [];

    public long IdItemDefinition { get; set; }
    public virtual ItemDefinition? ItemDefinition { get; set; }
}