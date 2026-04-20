namespace tara_tool.Data.Tabels;

using tara_tool.Data.Enums;

public class ImpactRating
{
    public long Id { get; set; }
    public virtual DamageScenario? DamageScenario { get; set; }
    public long DamageScenarioId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public long RiskValue { get; set; }
    public ImpactCategory ImpactCategory { get; set; }
    public virtual TreatmentDecision? TreatmentDecision { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
}