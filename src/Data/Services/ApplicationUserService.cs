using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
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
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.ApplicationUsers.AnyAsync(p => p.Email == email);
    }

    public async Task<bool> CheckNameOfUserDuplicate(string username)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.ApplicationUsers.AnyAsync(p => p.NameOfUser == username);
    }
}
