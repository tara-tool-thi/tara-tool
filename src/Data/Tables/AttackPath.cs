using System.ComponentModel.DataAnnotations.Schema;
using tara_tool.Data.Enums;

namespace tara_tool.Data.Tables;

public class AttackPath
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ElapsedTime ElapsedTime { get; set; }
    public SpecialistExpertise SpecialistExpertise { get; set; }
    public KnowledgeOfComponents KnowledgeOfComponents { get; set; }
    public WindowOfOpportunity WindowOfOpportunity { get; set; }
    public Equipment Equipment { get; set; }
    public long Value { get; set; }
    public AttackFeasibilityRating AttackFeasibilityRating { get; set; }
    public virtual ThreatScenario? ThreatScenarios { get; set; }
    public bool RiskTreatmentBool { get; set; }
    public string RiskTreatmentText { get; set; } = string.Empty;
    public List<AttackStep> Steps { get; set; } = new();

    [NotMapped]
    public bool AccordionExpanded = false;
}

public class AttackStep
{
    public long Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }

    // Foreign Key back to the parent
    public long AttackPathId { get; set; }
}