using System.Runtime.CompilerServices;

namespace tara_tool.Data.Tables;

public class Tag
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    //Tags need to be per project, so Access Rights can be checked
    public virtual Project? Project { get; set; }
    public long IdProject { get; set; }

}