namespace tara_tool.Data.Services;

using tara_tool.Data;
using tara_tool.Data.Services;
using tara_tool.Data.Tables;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;

public class DamageScenarioService
(
    IDbContextFactory<ApplicationDbContext> contextFactory,
    AccessControlService accessControlService,
    AssetService assetService
) : IDataService<DamageScenario>
{

    public async Task<DamageScenario?> CreateNew(long idAsset)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        Asset? asset = await assetService.GetItemByIdAsync
        (
            idAsset,
            include: e => e.Include(i => i.ItemDefinition)
                           .ThenInclude(j => j != null ? j.Project : null)
        );

        if (asset is null or { ItemDefinition: null } or { ItemDefinition.Project: null }
            || (
                    await accessControlService.CheckUserAccessRightsWrite(asset.ItemDefinition.Project.Id) is false
               )
           )
        {
            return null;
        }

        context.Attach(asset);
        IQueryable<DamageScenario> existingDamageScenarios = context.DamageScenarios.Where(
            e => e.Asset != null &&
            e.Asset.ItemDefinition != null &&
            e.Asset.ItemDefinition.IdProject == asset.ItemDefinition.IdProject);

        DamageScenario damageScenario = new()
        {
            Description = "New unnamed Damage Scenario",
            DamageScenarioNumber = existingDamageScenarios.Any() ?
                                   existingDamageScenarios.Select(e => e.DamageScenarioNumber)
                                                          .Max() + 1
                                                          : 1,
            Asset = asset
        };

        await context.AddAsync(damageScenario);
        await context.SaveChangesAsync();

        return damageScenario;
    }

    public async Task Delete(DamageScenario entityToDelete)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        // DamageScenario? damageScenario = await context.DamageScenarios.Include(e => e.Asset)
        //                                               .ThenInclude(e => e != null ? e.ItemDefinition : null)
        //                                               .FirstOrDefaultAsync(a => a.Id == entityToDelete.Id);

        context.Attach(entityToDelete);
        if (entityToDelete is not null and { Asset.ItemDefinition.IdProject: long projId }
            && await accessControlService.CheckUserAccessRightsWrite(projId) is true)
        {
            context.Remove(entityToDelete);
            await context.SaveChangesAsync();
        }
        return;
    }

    public async Task<DamageScenario?> Save(DamageScenario entityToSave)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        DamageScenario? damageScenario = await context.DamageScenarios.Include(e => e.Asset)
                                                      .ThenInclude(e => e != null ? e.ItemDefinition : null)
                                                      .FirstOrDefaultAsync(a => a.Id == entityToSave.Id);

        if (damageScenario is not null and { Asset.ItemDefinition.IdProject: long projId }
            && await accessControlService.CheckUserAccessRightsWrite(projId) is true)
        {
            context.Entry(damageScenario).CurrentValues.SetValues(entityToSave);
            await context.SaveChangesAsync();
            return entityToSave;
        }
        else return null;

    }

    public async Task<DamageScenario?> GetItemByIdAsync(long id, Func<IQueryable<DamageScenario>, IQueryable<DamageScenario>>? include = null, CancellationToken cancellationToken = default)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        IQueryable<DamageScenario> damageScenarios = context.DamageScenarios.AsQueryable();

        if (include is not null)
            damageScenarios = include(damageScenarios);

        DamageScenario? damageScenario = await damageScenarios.FirstOrDefaultAsync(a => a.Id == id);
        if (damageScenario is not null and { Asset.ItemDefinition.IdProject: long projId }
            && await accessControlService.CheckUserAccessRightsRead(projId) is true)
            return damageScenario;
        else return null;
    }

    public GridItemsProvider<DamageScenario> GetItemsProvider(long ProjectId, Func<IQueryable<DamageScenario>, IQueryable<DamageScenario>>? include = null, Func<IQueryable<DamageScenario>, IQueryable<DamageScenario>>? filter = null)
    {
        return async request =>
        {
            using ApplicationDbContext context =
                await contextFactory.CreateDbContextAsync(request.CancellationToken);


            if (await accessControlService.CheckUserAccessRightsRead(ProjectId) is false)
                return GridItemsProviderResult.From(new List<DamageScenario>(), 0);

            IQueryable<DamageScenario> damageScenario = context.DamageScenarios.AsNoTracking().Where(a => a.Asset!.ItemDefinition!.IdProject == ProjectId);

            if (include is not null)
                damageScenario = include(damageScenario);

            if (filter is not null)
                damageScenario = filter(damageScenario);

            int total = await damageScenario.CountAsync();
            List<DamageScenario> items = await request.ApplySorting(damageScenario)
                                                      .Skip(request.StartIndex)
                                                      .Take(request.Count ?? 20)
                                                      .ToListAsync(request.CancellationToken);

            return GridItemsProviderResult.From(items, total);
        };
    }

    public async Task<(List<DamageScenario>, int TotalItems)> GetItems(long ProjectId, Func<IQueryable<DamageScenario>, IQueryable<DamageScenario>>? include = null, Func<IQueryable<DamageScenario>, IQueryable<DamageScenario>>? filter = null)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        if (await accessControlService.CheckUserAccessRightsRead(ProjectId) is false)
        {
            return ([], 0);
        }

        IQueryable<DamageScenario> damageScenarios = context.DamageScenarios;

        if (include is not null)
        {
            damageScenarios = include(damageScenarios);
        }

        damageScenarios = damageScenarios.Where(d => d.Asset!.ItemDefinition!.IdProject == ProjectId);
        if (filter is not null)
        {
            damageScenarios = filter(damageScenarios);
        }

        return (await damageScenarios.ToListAsync(), await damageScenarios.CountAsync());
    }

    public async Task<int> CountAllItems(long ProjectId, Func<IQueryable<DamageScenario>, IQueryable<DamageScenario>>? filter = null)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        if (await accessControlService.CheckUserAccessRightsRead(ProjectId) is false)
        {
            return 0;
        }

        IQueryable<DamageScenario> damageScenarios = context.DamageScenarios;

        damageScenarios = damageScenarios.Where(a => a.Asset!.ItemDefinition!.IdProject == ProjectId);

        if (filter is not null)
        {
            damageScenarios = filter(damageScenarios);
        }

        return await damageScenarios.CountAsync();
    }
}
