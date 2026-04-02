namespace tara_tool.Data;
using System.Security.Cryptography;

public class Image
{
  public required long Id { get; set; }
  private byte[] _data;

  public byte[] Data
  {
    get => _data;
    set
    {
      _data = value ?? throw new ArgumentNullException(nameof(Data));
      // Recompute the Hash every time the image data changes 
      Hash = ComputeHash(_data);
    }
  }

  public byte[] Hash { get; private set; }

  private byte[] ComputeHash(byte[] data)
  {
    return SHA256.Create().ComputeHash(data);
  }
}