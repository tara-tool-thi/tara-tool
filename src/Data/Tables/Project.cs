using System.Text.Json.Serialization;

namespace tara_tool.Data.Tables;

public class Project
{
    [JsonIgnore]
    public long Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateLastChanged { get; set; } = DateTime.UtcNow;
    public bool IsArchived { get; set; } = false;

    [JsonIgnore]
    public virtual ICollection<AccessControl> Access { get; set; } = [];
    public virtual ICollection<ItemDefinition> ItemDefinitions { get; set; } = [];
    public virtual ICollection<Tag> Tags { get; set; } = [];
}
