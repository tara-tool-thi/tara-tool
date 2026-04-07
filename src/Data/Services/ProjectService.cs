using Microsoft.EntityFrameworkCore;
using tara_tool.Data;

public class ProjectService(
    IDbContextFactory<ApplicationDbContext> contextFactory) {

  private readonly IDbContextFactory<ApplicationDbContext> _contextFactory =
      contextFactory;

  public async Task<Project?>
  GetProjectAsync(long id, Func<DbSet<Project>, DbSet<Project>>? extend) {
    using ApplicationDbContext context =
        await _contextFactory.CreateDbContextAsync();

    DbSet<Project> set = context.Projects;
    if (extend != null) {
      set = extend.Invoke(context.Projects);
    }
    return await set.FirstOrDefaultAsync(p => p.Id == id);
  }

  public async Task<List<Project>> GetProjectsAsync(
      string? IdUser = null,
      Func<IQueryable<Project>, IQueryable<Project>>? extend = null) {
    using ApplicationDbContext context =
        await _contextFactory.CreateDbContextAsync();
    IQueryable<Project> set = context.Projects.AsQueryable();

    if (IdUser != null) {
      set = set.Where(p => p.Access.Select(a => a.Member.Id).Contains(IdUser));
    }
    if (extend != null) {
      set = extend.Invoke(set);
    }
    return await set.ToListAsync();
  }
}
