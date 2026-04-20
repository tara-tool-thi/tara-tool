using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.FluentUI.AspNetCore.Components;
using tara_tool.Components;
using tara_tool.Components.Account;
using tara_tool.Data;
using tara_tool.Data.Tabels;
using tara_tool.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider,
                           IdentityRevalidatingAuthenticationStateProvider>();

builder.Services
    .AddAuthentication(options =>
    {
      options.DefaultScheme = IdentityConstants.ApplicationScheme;
      options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDbContextFactory<ApplicationDbContext>(
    options => ApplicationDbContext.GetDbConfig(options, builder),
    ServiceLifetime.Scoped);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();



builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
      options.SignIn.RequireConfirmedAccount = true;
      options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<AccessControlService>();
builder.Services.AddTransient<ProjectService>();
builder.Services.AddTransient<ItemDefinitionService>();
builder.Services.AddTransient<SessionService>();
builder.Services.AddTransient<AssetService>();
builder.Services
    .AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for
  // production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found",
                                    createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
