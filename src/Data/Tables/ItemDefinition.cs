namespace tara_tool.Data.Tables;

public class ItemDefinition
{
    public long Id { get; set; }
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

    // Adding the foreign Key here explicitly -> might safe us some extra entity
    // loading, when we want to filter/search/order the ItemDefinitions by Project
    // Id
    public long IdProject { get; set; }
    public required virtual Project Project { get; set; }
    public virtual ICollection<Asset> Assets { get; set; } = [];
}
