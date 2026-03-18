using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SupperChainErpDemo.Web.Data;
using SupperChainErpDemo.Web.Services;

var builder = WebApplication.CreateBuilder(args);
var sqliteConnection = new SqliteConnection("Data Source=SupperChainErpDemo;Mode=Memory;Cache=Shared");
sqliteConnection.Open();

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(sqliteConnection);
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
    options.UseSqlite(serviceProvider.GetRequiredService<SqliteConnection>()));
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IFilterService, FilterService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddSingleton<IInventoryService, InventoryService>();
builder.Services.AddScoped<IInventoryQueryService, InventoryQueryService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddSingleton<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IUserManagementCoordinator, UserManagementCoordinator>();
builder.Services.AddScoped<IRoleManagementCoordinator, RoleManagementCoordinator>();
builder.Services.AddScoped<ICategoryManagementCoordinator, CategoryManagementCoordinator>();
builder.Services.AddScoped<IProductManagementCoordinator, ProductManagementCoordinator>();
builder.Services.AddScoped<ITransferCoordinator, TransferCoordinator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
