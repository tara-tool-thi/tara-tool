using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using tara_tool.Data;
using tara_tool.Data.Tables;

public class PendingRegistrationService(
    IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<PendingRegistrationService> logger)
{
    public async Task<string?> Create(string email)
    {
        string id = RandomNumberGenerator.GetHexString(64, true);

        PendingRegistration pendingRegistration = new(){Email=email, Id=id};

        if (!await Save(pendingRegistration))
            return null;

        return id;
    }

    private async Task<bool> Save(PendingRegistration entityToSave)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        try
        {
            await context.PendingRegistrations.AddAsync(entityToSave);
            await context.SaveChangesAsync();
        } catch(Exception ex)
        {
            logger.LogError("Failed to save pending registration: {message}", ex.Message);
            return false;
        }

        return true;
    }

    public async Task<bool> Check(string email, string id, bool firstUser)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        if (firstUser)
        {
            return await context.PendingRegistrations.AnyAsync(p => p.Id == id);
        }

        return await context.PendingRegistrations.AnyAsync(p => p.Id == id && p.Email == email);
    }

    public async Task Delete(string id)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        PendingRegistration? pendingRegistration = await context.PendingRegistrations.FirstOrDefaultAsync(p => p.Id == id);

        if (pendingRegistration is null)
        {
            return;
        }

        context.PendingRegistrations.Remove(pendingRegistration);
        await context.SaveChangesAsync();
    }

    public async Task<List<PendingRegistration>> GetAllPendingRegistrations()
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.PendingRegistrations.ToListAsync();
    }

    public async Task<bool> Any()
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        return await context.PendingRegistrations.AnyAsync();
    }

    public async Task<string?> GetIdCountOne()
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        int count = context.PendingRegistrations.Count();
        if(count == 1)
        {
            List<PendingRegistration> pendingRegistrations = await GetAllPendingRegistrations();
            return pendingRegistrations.First().Id;
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> EmailUsed(string email)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        return await context.PendingRegistrations.AnyAsync(p => p.Email == email);
    }

}
