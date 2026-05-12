namespace tara_tool.Data.Tables;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using tara_tool.Data.Enums;

public class DamageScenario
{
    public long Id { get; set; }
    public Asset? Asset { get; set; }
    public string Description { get; set; } = string.Empty;
    [NotMapped] public string DescriptionTruncated => Description switch
    {
        { Length: >= 25 } => $"{Description[..25]}…",
        _ => Description
    };

    public long DamageScenarioNumber { get; set; } = 0;

    public bool ConfidentialityAffected { get; set; } = false;
    public bool IntegrityAffected { get; set; } = false;
    public bool AvailabilityAffected { get; set; } = false;

    [NotMapped]
    public (ImpactRatingValue Safety, ImpactRatingValue Financial,
        ImpactRatingValue Operational, ImpactRatingValue Privacy) ImpactRating
    {
        get => (Safety, Financial, Operational, Privacy);
        set
        {
            Safety = value.Safety;
            Financial = value.Financial;
            Operational = value.Operational;
            Privacy = value.Privacy;
        }
    }

    public ImpactRatingValue Safety { get; set; } = ImpactRatingValue.Negligible;
    public ImpactRatingValue Financial { get; set; } = ImpactRatingValue.Negligible;
    public ImpactRatingValue Operational { get; set; } = ImpactRatingValue.Negligible;
    public ImpactRatingValue Privacy { get; set; } = ImpactRatingValue.Negligible;

    public virtual ICollection<ThreatScenario> ThreatScenarios { get; set; } = [];
}
