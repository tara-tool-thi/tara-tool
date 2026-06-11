using System.Text.Json.Serialization;

namespace tara_tool.Data.Tables;

public class Image(byte[] data)
{
    [JsonIgnore]
    public long Id { get; set; }

    // Needed as a workaround for CS8618. See GitHub Issue:
    // https://github.com/dotnet/roslyn/issues/32358. Not the best approach and
    // one I really dislike tbh. - Ardwetha
    // Thanks to hyiev1 for improving the
    // class by adding/rewritting the behavior
    [JsonInclude]
    public byte[] Data { get; set; } = data;
}
