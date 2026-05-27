using Microsoft.EntityFrameworkCore;
using tara_tool.Data;

public class ApplicationUserService(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task<bool> CheckEmailDuplicate(string email)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        return await context.ApplicationUsers.AnyAsync(p => p.Email == email);
    }

    public async Task<bool> CheckNameOfUserDuplicate(string username)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        return await context.ApplicationUsers.AnyAsync(p => p.NameOfUser == username);
    }
}
