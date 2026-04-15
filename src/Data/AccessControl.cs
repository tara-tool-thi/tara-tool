using System.ComponentModel.DataAnnotations.Schema;

namespace tara_tool.Data;

public class AccessControl
{
  // Setting this to true, because it does not make sense to have a person who
  // cannot do anything
  public long Id { get; set; }
  public bool ReadAccess { get; set; } = true;
  public bool WriteAccess { get; set; } = false;
  public bool Manage { get; set; } = false;
  public bool Owner { get; set; } = false;


  public required ApplicationUser ApplicationUser { get; set; }
  public required Project Project { get; set; }
}
