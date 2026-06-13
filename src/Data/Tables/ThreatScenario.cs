using System.Text.Json.Serialization;
using tara_tool.Data.Enums;
namespace tara_tool.Data.Tables;

public class ThreatScenario
{
    [JsonIgnore]
    public long Id { get; set; }
    [JsonIgnore]
    public DamageScenario? DamageScenarios { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Stride StrideCategorie { get; set; }
    public long RiskValue { get; set; }
    public virtual ICollection<AttackPath> AttackPaths { get; set; } = [];
}
