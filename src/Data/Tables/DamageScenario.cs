namespace tara_tool.Data.Tabels;

public class DamageScenario
{
    public long Id { get; set; }
    public ICollection<Asset> Assets { get; set; } = [];
    public string Description { get; set; } = string.Empty;
    public bool ConfidentialityImpact { get; set; } = false;
    public bool IntegrityImpact { get; set; } = false;
    public bool AvailabilityImpcat { get; set; } = false;
    public long DamageScenarioNumber { get; set; } = 0;
    public ImpactRating? ImpactRating { get; set; }
    public ICollection<ThreatScenario> ThreatScenarios { get; set; } = [];

}