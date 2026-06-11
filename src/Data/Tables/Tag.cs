using System.Text.Json.Serialization;

namespace tara_tool.Data.Tables;

public class Tag
{
    [JsonIgnore]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Tags need to be per project, so Access Rights can be checked
    public virtual Project? Project { get; set; }
}
