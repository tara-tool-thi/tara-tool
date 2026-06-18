using Microsoft.EntityFrameworkCore;
using tara_tool.Data;
using tara_tool.Data.Tables;

public class ApplicationUserService(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task<List<ApplicationUser>> GetAllUsers()
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.ApplicationUsers.ToListAsync();
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.ApplicationUsers.FindAsync(userId);
    }

    public async Task UpdateProfilePictureAsync(string userId, byte[]? picture)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        ApplicationUser? user = await context.ApplicationUsers.FindAsync(userId);
        if (user is not null)
        {
            user.ProfilePicture = picture;
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> CheckEmailDuplicate(string email)
    {
        string normalized = email.ToUpper();
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.ApplicationUsers.AnyAsync(p => p.NormalizedEmail == normalized);
    }

    public async Task<bool> CheckNameOfUserDuplicate(string username)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.ApplicationUsers.AnyAsync(p => p.NameOfUser == username);
    }

    public async Task ToggleResetPassword(string id)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        ApplicationUser? user = await context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == id);
        if(user?.ResetPassword == false){
            user?.ResetPassword = true;
        }
        else{
            user?.ResetPassword = false;
        }
        await context.SaveChangesAsync();
    }

    public async Task<bool> CheckResetPassword(string id)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        return await context.ApplicationUsers.AnyAsync(p => p.Id == id && p.ResetPassword == true);

    }

}
