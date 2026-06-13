using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using tara_tool.Components.Account.Pages;
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
