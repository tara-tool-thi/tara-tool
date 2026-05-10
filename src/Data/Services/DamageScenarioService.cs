namespace tara_tool.Data.Services;

using tara_tool.Data;
using tara_tool.Data.Services;
using tara_tool.Data.Tables;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;

public class DamageScenarioService(IDbContextFactory<ApplicationDbContext> contextFactory, AccessControlService accessControlService) : IDataService<DamageScenario>
{
    public async Task Delete(DamageScenario entityToDelete)
    {

    }

    public async Task<DamageScenario?> Save(DamageScenario entityToSave)
    {

    }

    public async Task<DamageScenario?> GetItemByIdAsync(long id, Func<IQueryable<DamageScenario>, IQueryable<DamageScenario>>? include = null, CancellationToken cancellationToken = default)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        IQueryable<DamageScenario> DamageScenarios = context.DamageScenarios.AsQueryable();
        if (include is not null)
        {
            DamageScenarios = include(DamageScenarios);
        }
        DamageScenario? DamageScenario = await DamageScenarios.FirstOrDefaultAsync(a => a.Id == id);
        if (DamageScenario is not null and { Asset.ItemDefinition.IdProject: long projId } && await accessControlService.CheckUserAccessRightsRead(projId) is true)
            return DamageScenario;
        else return null;
    }

    public GridItemsProvider<DamageScenario> GetItemsProvider(long ProjectId, Func<IQueryable<DamageScenario>, IQueryable<DamageScenario>>? include = null, Func<IQueryable<DamageScenario>, IQueryable<DamageScenario>>? filter = null);
}
