namespace tara_tool.Data.Tables;

using System.ComponentModel.DataAnnotations.Schema;
using tara_tool.Data.Enums;

public class DamageScenario
{
    public long Id { get; set; }
    public Asset? Asset { get; set; }
    public string Description { get; set; } = string.Empty;
    public long DamageScenarioNumber { get; set; } = 0;

    [NotMapped]
    public (bool Confidentiality, bool Integrity, bool Availability) AffectedSecurityGoals
    {
        get => (confidentialityAffected, integrityAffected, availabilityAffected);
        set
        {
            confidentialityAffected = value.Confidentiality;
            integrityAffected = value.Integrity;
            availabilityAffected = value.Availability;
        }
    }

    private bool confidentialityAffected = false;
    private bool integrityAffected = false;
    private bool availabilityAffected = false;

    [NotMapped]
    public (ImpactRatingValue Safety, ImpactRatingValue Financial,
        ImpactRatingValue Operational, ImpactRatingValue Privacy) ImpactRating
    {
        get => (safety, financial, operational, privacy);
        set
        {
            safety = value.Safety;
            financial = value.Financial;
            operational = value.Operational;
            privacy = value.Privacy;
        }
    }

    private ImpactRatingValue safety = ImpactRatingValue.Negligible;
    private ImpactRatingValue financial = ImpactRatingValue.Negligible;
    private ImpactRatingValue operational = ImpactRatingValue.Negligible;
    private ImpactRatingValue privacy = ImpactRatingValue.Negligible;

    public virtual ICollection<ThreatScenario> ThreatScenarios { get; set; } = [];
}
