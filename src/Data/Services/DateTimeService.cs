using Microsoft.JSInterop;

public class DateTimeService(IJSRuntime jSRuntime)
{
    private readonly IJSRuntime _jsRuntime = jSRuntime;

    private TimeSpan? _userOffset;

    public async ValueTask<DateTime> GetLocalDateTime(DateTime dateTime)
    {
        if (_userOffset == null)
        {
            int offsetInMinutes = await _jsRuntime.InvokeAsync<int>("blazorGetTimezoneOffset");
            _userOffset = TimeSpan.FromMinutes(-offsetInMinutes);
        }

        return dateTime.Add(_userOffset ?? TimeSpan.FromTicks(0));
    }

}