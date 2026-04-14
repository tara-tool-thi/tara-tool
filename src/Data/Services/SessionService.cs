using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using tara_tool.Data;

public class SessionService(
    AuthenticationStateProvider AuthState,
    UserManager<ApplicationUser> UserManager
)
{
    public async Task<ApplicationUser?> GetApplicationUserAsync()
    {
        AuthenticationState authState = await AuthState.GetAuthenticationStateAsync();
        ClaimsPrincipal? principal = authState.User;
    
      
        ApplicationUser? user  = await UserManager.GetUserAsync(principal);
        return user;
    }

}