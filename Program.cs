using StrategicPlanningDashboard.Components;
using StrategicPlanningDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// SQLite is local to this independent demo. Override the path with NORTHSTAR_DB_PATH.
var configuredPath = builder.Configuration["NORTHSTAR_DB_PATH"];
var databaseFile = string.IsNullOrWhiteSpace(configuredPath)
    ? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "northstar.db")
    : Path.GetFullPath(configuredPath);

// Create and seed once on startup; subsequent circuits read the same database.
_ = new PlanningStore(databaseFile);
builder.Services.AddScoped(_ => new PlanningStore(databaseFile));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
