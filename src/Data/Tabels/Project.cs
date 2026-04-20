namespace tara_tool.Data.Tabels;

public class Project
{
  public long Id { get; set; }
  public string ProjectName { get; set; } = string.Empty;
  public DateTime DateCreated { get; set; } = DateTime.UtcNow;
  public DateTime DateLastChanged { get; set; } = DateTime.UtcNow;

  public ICollection<AccessControl> Access { get; set; } = [];
  public ICollection<ItemDefinition> ItemDefinitions { get; set; } = [];
}
