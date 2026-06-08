using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using tara_tool.Data.Enums;

namespace tara_tool.Data.Tables;

public class AttackPath
{
    [JsonIgnore]
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
    [JsonIgnore]
    public virtual ThreatScenario? ThreatScenarios { get; set; }
    public bool RiskTreatmentBool { get; set; }
    public string RiskTreatmentText { get; set; } = string.Empty;
    public List<AttackStep> Steps { get; set; } = new();

    [NotMapped, JsonIgnore]
    public bool AccordionExpanded = false;
}

public class AttackStep
{
    [JsonIgnore]
    public long Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }

    // Navigation property back to the parent
    [JsonIgnore]
    public virtual required AttackPath AttackPath { get; set; }
}