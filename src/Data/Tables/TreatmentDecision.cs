namespace tara_tool.Data.Tabels;

using tara_tool.Data.Enums;

public class TreatmentDecision
{
    public long Id { get; set; }
    public RiskTreatmentOption RiskTreatmentOption { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public ImpactRating? ImpactRating { get; set; }
}
