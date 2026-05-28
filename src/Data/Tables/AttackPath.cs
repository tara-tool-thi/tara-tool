using tara_tool.Data.Enums;

namespace tara_tool.Data.Tables;

public class AttackPath
{
    public long Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public ElapsedTime ElapsedTime { get; set; }
    public SpecialistExpertise SpecialistExpertise { get; set; }
    public KnowledgeOfComponents KnowledgeOfComponents { get; set; }
    public WindowOfOpportunity WindowOfOpportunity { get; set; }
    public Equipment Equipment { get; set; }
    public long Value { get; set; }
    public AttackFeasibilityRating AttackFeasibilityRating { get; set; }
    public virtual ThreatScenario? ThreatScenarios { get; set; }
}