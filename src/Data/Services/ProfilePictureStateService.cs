namespace tara_tool.Data.Services;

/// <summary>
/// Shared per-circuit (Scoped) service so ProfilePictureUpload can notify
/// TaraProfileMenu immediately when the picture changes — no page reload needed.
/// </summary>
public class ProfilePictureStateService
{
    private string? _url;

    public string? PictureDataUrl
    {
        get => _url;
        set { _url = value; OnChange?.Invoke(); }
    }

    public event Action? OnChange;
}
