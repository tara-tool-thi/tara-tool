namespace tara_tool.Data.Services;

public static class DatabaseServicesExtension
{
    extension(IServiceCollection services)
    {
        public void AddDatabaseServices()
        {
            services.AddTransient<AccessControlService>();
            services.AddTransient<PendingRegistrationService>();
            services.AddTransient<ProjectService>();
            services.AddTransient<ItemDefinitionService>();
            services.AddTransient<SessionService>();
            services.AddTransient<AssetService>();
            services.AddTransient<TagService>();
            services.AddTransient<DamageScenarioService>();
        }
    }
}
