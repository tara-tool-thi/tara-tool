using System.Text.Json.Serialization;

namespace tara_tool.Data.Tables;

public class ItemDefinition
{
    [JsonIgnore]
    public long Id { get; set; }
    public long ItemNumber { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public string ItemBoundaryText { get; set; } = string.Empty;
    public string ItemFunction { get; set; } = string.Empty;
    public string PreliminaryArchitectureText { get; set; } = string.Empty;
    public string OperationalEnvironmentText { get; set; } = string.Empty;
    public virtual Image? TechnicalSketch { get; set; } = null;
    public virtual Image? PreliminaryArchitecture { get; set; } = null;
    public virtual Image? ItemBoundary { get; set; } = null;
    public virtual Image? OperationalEnvironmentImage { get; set; } = null;

    [JsonIgnore]
    public long IdProject { get; set; }
    [JsonIgnore]
    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<Asset> Assets { get; set; } = [];
}
