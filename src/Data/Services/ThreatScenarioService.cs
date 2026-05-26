using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using tara_tool.Data.Tables;
using tara_tool.Data.Enums;

namespace tara_tool.Data.Services;

public class ThreatScenarioService(IDbContextFactory<ApplicationDbContext> contextFactory, AccessControlService accessControlService) : IDataService<ThreatScenario>
{
    /// <summary>
    /// Helper to find the Project ID associated with a Threat Scenario via its deep relations.
    /// </summary>
    private async Task<long?> GetProjectIdForScenarioAsync(ApplicationDbContext context, long threatScenarioId)
    {
        return await context.ThreatScenarios
            .Where(ts => ts.Id == threatScenarioId)
            .Select(ts => ts.DamageScenarios)
            .Select(ds => ds!.Asset)
            .Select(a => a!.ItemDefinition!.IdProject)
            .FirstOrDefaultAsync();
    }

    public async Task<ThreatScenario?> CreateThreatScenarioAsync(long projectID, string name, DamageScenario DS)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        /*if (await accessControlService.CheckUserAccessRightsWrite(projectID) == false)
        {
            return null;
        }*/

        context.Attach(DS);

        ThreatScenario newScenario = new ThreatScenario
        {
            Name = name,
            DamageScenarios = DS,
        };

        await context.ThreatScenarios.AddAsync(newScenario);
        await context.SaveChangesAsync();

        return newScenario;
    }

    /// <summary>
    /// Creates a new AttackPath and associates it with an existing ThreatScenario.
    /// </summary>
    public async Task<AttackPath?> CreateAttackPathAsync(long threatScenarioId, AttackPath newPath)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        // 1. Find the parent scenario and include its Project link for security
        ThreatScenario? scenario = await context.ThreatScenarios
            .Include(s => s.AttackPaths)
                .ThenInclude(ap => ap.Steps.OrderBy(step => step.Order))
            .FirstOrDefaultAsync(s => s.Id == threatScenarioId);

        if (scenario == null) return null;

        // 2. Perform Security Check: Does the user have rights to the project this scenario belongs to?
        // We look for the first available project ID in the chain
        long? projectId = await GetProjectIdForScenarioAsync(context, threatScenarioId);


        /*if (projectId == 0 || await accessControlService.CheckUserAccessRightsWrite(projectId) == false)
        {
            return null;
        }*/

        // 3. Create and Link the AttackPath
        newPath.Id = 0; // Ensure it's treated as a new entity

        // We add the path to the scenario's collection.
        // EF Core will handle the foreign key relationship automatically.
        scenario.AttackPaths.Add(newPath);

        await context.SaveChangesAsync();
        return newPath;
    }

    public async Task<ThreatScenario?> GetItemByIdAsync(long id, Func<IQueryable<ThreatScenario>, IQueryable<ThreatScenario>>? include = null, CancellationToken cancellationToken = default)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        IQueryable<ThreatScenario> query = context.ThreatScenarios.AsNoTracking().AsQueryable();

        if (include is not null)
        {
            query = include(query);
        }

        ThreatScenario? scenario = await query.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (scenario is null) return null;

        // Deep Security Check
        /*long? projectId = await GetProjectIdForScenarioAsync(context, id);
        if (projectId == null || await accessControlService.CheckUserAccessRightsRead(projectId.Value) is false)
        {
            return null;
        }*/

        return scenario;
    }

    /*public async Task<List<ThreatScenario>> GetThreatScenariosAsync(long projectID, Func<DbSet<ThreatScenario>, IQueryable<ThreatScenario>>? extend = null)
    {
        // Since ThreatScenario has no direct IdProject, we must filter by the relationship chain
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        IQueryable<ThreatScenario> query = context.ThreatScenarios;

        if (extend != null)
        {
            query = extend.Invoke(context.ThreatScenarios);
        }

        // Filter scenarios that are connected to assets belonging to this project
        query = query.Where(ts => ts.DamageScenarios
                        .SelectMany(ds => ds.Assets)
                        .Any(a => a.ItemDefinition!.IdProject == projectID));

        // Check access for the project itself before returning results
        if (await accessControlService.CheckUserAccessRightsRead(projectID) is false)
        {
            return new List<ThreatScenario>();
        }

        return await query.ToListAsync();
    }*/

    public async Task<ThreatScenario?> Save(ThreatScenario scenario)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        // We must find the existing entity and include its relations to handle updates correctly
        ThreatScenario? existingScenario = await context.ThreatScenarios
            .Include(s => s.AttackPaths)
                .ThenInclude(ap => ap.Steps.OrderBy(step => step.Order))
            .FirstOrDefaultAsync(s => s.Id == scenario.Id);

        if (existingScenario == null) return null;

        // Security Check: Verify the user has rights to the project this scenario belongs to
        /*long? projectId = await GetProjectIdForScenarioAsync(context, scenario.Id);
        if (projectId == null || !await accessControlService.CheckUserAccessRightsWrite(projectId.Value))
            return null;*/

        // Update scalar properties
        context.Entry(existingScenario).CurrentValues.SetValues(scenario);

        // Sync the AttackPaths
        foreach (AttackPath incomingPath in scenario.AttackPaths)
        {
            if (incomingPath.Id == 0)
            {
                // Case A: It's a brand new path.
                existingScenario.AttackPaths.Add(incomingPath);
            }
            else
            {
                // Case B: It's an existing path.
                AttackPath? trackedPath = existingScenario.AttackPaths
                    .FirstOrDefault(p => p.Id == incomingPath.Id);

                if (trackedPath != null)
                {
                    // Update the properties of the already-tracked object.
                    context.Entry(trackedPath).CurrentValues.SetValues(incomingPath);

                    // Step A: Identify steps to REMOVE
                    // We remove them if:
                    // their text is now empty/whitespace (deleted via backspace)
                    List<AttackStep> stepsToRemove = trackedPath.Steps
                        .Where(ts =>
                            !incomingPath.Steps.Any(ip => ip.Id == ts.Id) || // Not in incoming list
                            string.IsNullOrWhiteSpace(incomingPath.Steps.FirstOrDefault(ip => ip.Id == ts.Id)?.Text)              // OR text is now empty
                        )
                        .ToList();

                    foreach (AttackStep stepToRemove in stepsToRemove)
                    {
                        trackedPath.Steps.Remove(stepToRemove);
                    }

                    // 2. Update existing steps or add new ones
                    for (int i = 0; i < incomingPath.Steps.Count; i++)
                    {
                        AttackStep incomingStep = incomingPath.Steps[i];

                        if (string.IsNullOrWhiteSpace(incomingStep.Text)) continue;
                        if (incomingStep.Id == 0)
                        {
                            // New step
                            incomingStep.Order = i;
                            trackedPath.Steps.Add(incomingStep);
                        }
                        else
                        {
                            // Existing step: Update its text
                            AttackStep? trackedStep = trackedPath.Steps
                                .FirstOrDefault(ts => ts.Id == incomingStep.Id);

                            if (trackedStep != null)
                            {
                                context.Entry(trackedStep).CurrentValues.SetValues(incomingStep);
                                trackedStep.Order = i;
                            }
                        }
                    }
                }
            }
        }

        await context.SaveChangesAsync();
        return existingScenario;
    }

    /// <summary>
    /// Deletes an AttackPath from a ThreatScenario. If the AttackPath belongs to only this scenario,
    /// it will be completely removed from the database. Otherwise, only the relationship is removed.
    /// </summary>
    public async Task DeleteAttackPathAsync(long threatScenarioId, long attackPathId)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        // 1. Find the scenario and attack path
        ThreatScenario? scenario = await context.ThreatScenarios
            .Include(s => s.AttackPaths)
            .Include(s => s.DamageScenarios)
                .ThenInclude(ds => ds!.Asset)
                    .ThenInclude(a => a!.ItemDefinition)
            .FirstOrDefaultAsync(s => s.Id == threatScenarioId);

        if (scenario == null) return;

        // Security Check: Does user have write access to this project?
        long? projectId = await GetProjectIdForScenarioAsync(context, threatScenarioId);


        /*if (projectId == 0 || !await accessControlService.CheckUserAccessRightsWrite(projectId))
            return;*/

        // 2. Find the attack path to remove
        AttackPath? attackPathToRemove = scenario.AttackPaths.FirstOrDefault(ap => ap.Id == attackPathId);
        if (attackPathToRemove == null) return;

        // 3. Remove the relationship
        scenario.AttackPaths.Remove(attackPathToRemove);

        // 4. Check if this was the only scenario for this attack path
        int otherScenariosCount = await context.ThreatScenarios
            .Where(ts => ts.Id != threatScenarioId && ts.AttackPaths.Any(ap => ap.Id == attackPathId))
            .CountAsync();

        // 5. If no other scenarios reference this attack path, delete it completely
        if (otherScenariosCount == 0)
        {
            context.AttackPaths.Remove(attackPathToRemove);
        }

        await context.SaveChangesAsync();
    }

    public async Task Delete(ThreatScenario scenario)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        ThreatScenario? existingScenario = await context.ThreatScenarios
            .Include(s => s.DamageScenarios)
                .ThenInclude(ds => ds!.Asset)
            .FirstOrDefaultAsync(s => s.Id == scenario.Id);

        if (existingScenario == null) return;

        // Security Check
        long? projectId = await GetProjectIdForScenarioAsync(context, scenario.Id);
        if (projectId == null || !await accessControlService.CheckUserAccessRightsWrite(projectId.Value))
            return;

        context.ThreatScenarios.Remove(existingScenario);
        await context.SaveChangesAsync();
    }

    public GridItemsProvider<ThreatScenario> GetItemsProvider(long projectId, Func<IQueryable<ThreatScenario>, IQueryable<ThreatScenario>>? include = null, Func<IQueryable<ThreatScenario>, IQueryable<ThreatScenario>>? filter = null)
    {
        return async request =>
        {
            await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync(request.CancellationToken);
            if (await accessControlService.CheckUserAccessRightsRead(projectId) is false)
            {
                return GridItemsProviderResult.From(new List<ThreatScenario>(), 0);
            }

            IQueryable<ThreatScenario> query = context.ThreatScenarios.AsNoTracking();

            if (include != null) query = include(query);
            if (filter != null) query = filter(query);

            // Apply the Project filter via the relationship chain
            query = query.Where(ts => ts.DamageScenarios!.Asset!.ItemDefinition!.IdProject == projectId);


            int total = await query.CountAsync(request.CancellationToken);
            List<ThreatScenario> items = await request.ApplySorting(query)
                .Skip(request.StartIndex)
                .Take(request.Count ?? 20)
                .ToListAsync(request.CancellationToken);

            return GridItemsProviderResult.From(items, total);
        };
    }

    private static float GetFeasibilityRiskFactor(AttackFeasibilityRating rating)
        => rating switch
        {
            AttackFeasibilityRating.Critical => 2,
            AttackFeasibilityRating.High => 2,
            AttackFeasibilityRating.Medium => 1.5f,
            AttackFeasibilityRating.Low => 1,
            AttackFeasibilityRating.VeryLow => 0,
            _ => 0,
        };

    public string CalculateAttackPathRiskValue(AttackPath attackPath, float impactRating)
    {
        float risk = 1 + impactRating * GetFeasibilityRiskFactor(attackPath.AttackFeasibilityRating);
        attackPath.Value = (long)risk;
        return ((long)risk).ToString();
    }

    public async Task<AttackFeasibilityRating> FindMostCriticalRiskValue(ThreatScenario thisTS)
    {
        // 1. Ensure we have data to work with to avoid errors
        if (thisTS?.AttackPaths == null || !thisTS.AttackPaths.Any())
        {
            return AttackFeasibilityRating.VeryLow; // Return the "least critical" if no paths exist
        }

        // 2. Use LINQ to find the minimum value.
        // Since Critical = 0 and VeryLow = 4, Min() will find the most critical rating.
        AttackFeasibilityRating mostCritical = thisTS.AttackPaths
            .Min(ap => ap.AttackFeasibilityRating);

        return mostCritical;
    }

    public async Task<string> CalculateRiskValue(ThreatScenario thisTS, float ImpactRating)
    {
        AttackFeasibilityRating mostCritical = await FindMostCriticalRiskValue(thisTS);
        float risk = 1 + ImpactRating * GetFeasibilityRiskFactor(mostCritical);

        thisTS!.RiskValue = (long)risk;

        return ((long)risk).ToString();
    }
}
