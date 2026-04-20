using tara_tool.Data.Enums;
using tara_tool.Data.Tabels;
namespace tara_tool.Data.Tabels;

public class ThreatScenario
{
    public long Id { get; set; }
    public ICollection<DamageScenario> DamageScenarios { get; set; } = [];
    public string Description { get; set; } = string.Empty;
    public Stride StrideCategorie { get; set; }
    public long RiskValue { get; set; }
    public ICollection<AttackPath> AttackPaths { get; set; } = [];
}

