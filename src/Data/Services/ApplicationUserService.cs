using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using tara_tool.Data;
using tara_tool.Data.Tables;

public class ApplicationUserService(IDbContextFactory<ApplicationDbContext> contextFactory)
{
    public async Task<List<ApplicationUser>> GetAllUsers()
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.ApplicationUsers.ToListAsync(); //Alle Benutzer aus der Datenbank, Tabelle A.U abrufen
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
