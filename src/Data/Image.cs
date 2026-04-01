namespace tara_tool.Data;

public class Image
{
  public long Id { get; set; }
  public byte[]? Data { get; set; } = null;
  public byte[]? Hash { get; set; } = null;
}
