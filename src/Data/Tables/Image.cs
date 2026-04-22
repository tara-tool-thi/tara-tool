namespace tara_tool.Data.Tables;

using System.Security.Cryptography;

public class Image
{
  public required long Id { get; set; }

  // Needed as a workaround for CS8618. See GitHub Issue:
  // https://github.com/dotnet/roslyn/issues/32358. Not the best approach and
  // one I really dislike tbh. - Ardwetha
  // Thanks to hyiev1 for improving the
  // class by adding/rewritting the behavior
  private byte[] _data = null!;

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

  public byte[] Hash { get; private set; } = null!;

  private byte[] ComputeHash(byte[] data)
  {
    return SHA256.Create().ComputeHash(data);
  }

  public Image(byte[] data) { Data = data; }
}
