using tara_tool.Data.Enums;
using tara_tool.Data.Tables;
namespace tara_tool.Data.Tables;

public class ThreatScenario
{
    public long Id { get; set; }
    public virtual ICollection<DamageScenario> DamageScenarios { get; set; } = [];
    public string Description { get; set; } = string.Empty;
    public Stride StrideCategorie { get; set; }
    public long RiskValue { get; set; }
    public virtual ICollection<AttackPath> AttackPaths { get; set; } = [];
}

