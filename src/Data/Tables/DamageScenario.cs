namespace tara_tool.Data.Tables;

using tara_tool.Data.Enums;

public class DamageScenario
{
    public long Id { get; set; }
    public Asset? Asset { get; set; }
    public string Description { get; set; } = string.Empty;
    public long DamageScenarioNumber { get; set; } = 0;

    public (bool Confidentiality, bool Integrity, bool Availability) AffectedSecurityGoals { get; set; } = (false, false, false);

    public (ImpactRatingValue Safety, ImpactRatingValue Financial,
        ImpactRatingValue Operational, ImpactRatingValue Privacy)
        ImpactRating
    { get; set; } = (ImpactRatingValue.Negligible, ImpactRatingValue.Negligible,
        ImpactRatingValue.Negligible, ImpactRatingValue.Negligible);

    public virtual ICollection<ThreatScenario> ThreatScenarios { get; set; } = [];

}
