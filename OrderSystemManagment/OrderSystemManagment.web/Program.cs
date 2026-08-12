using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderSystemManagment.DataAccess.Context;
using OrderSystemManagment.DataAccess.Repositories;
using OrderSystemManagment.DataAccess.Services;
using OrderSystemManagment.Domain.Interfaces;
using OrderSystemManagment.web.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
    }

    options.UseSqlServer(connectionString);
});

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.Configure<OrderSystemManagment.DataAccess.Configuration.TcmbSettings>(
    builder.Configuration.GetSection("TcmbSettings"));

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/api/auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// DI Registrations
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IFindOrderFromOrderItemService, FindOrderFromOrderItemService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddHostedService<RetroactiveOrderFixerService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await dbContext.Database.MigrateAsync();

    // Seed Roles and Admin User
    if (!await dbContext.Roles.AnyAsync())
    {
        var adminRole = new OrderSystemManagment.Domain.Entities.Role
        {
            RoleName = "Admin",
            OrderManagement = new OrderSystemManagment.Domain.Entities.Module.ModulePermission
            {
                CanRead = true,
                CanCreate = true,
                CanUpdate = true,
                CanDelete = true
            },
            UserManagement = new OrderSystemManagment.Domain.Entities.Module.ModulePermission
            {
                CanRead = true,
                CanCreate = true,
                CanUpdate = true,
                CanDelete = true
            }
        };

        var userRole = new OrderSystemManagment.Domain.Entities.Role
        {
            RoleName = "User",
            OrderManagement = new OrderSystemManagment.Domain.Entities.Module.ModulePermission
            {
                CanRead = true,
                CanCreate = false,
                CanUpdate = false,
                CanDelete = false
            },
            UserManagement = new OrderSystemManagment.Domain.Entities.Module.ModulePermission
            {
                CanRead = false,
                CanCreate = false,
                CanUpdate = false,
                CanDelete = false
            }
        };

        await dbContext.Roles.AddRangeAsync(adminRole, userRole);
        await dbContext.SaveChangesAsync();

        if (!await dbContext.Users.AnyAsync())
        {
            var adminUser = new OrderSystemManagment.Domain.Entities.User
            {
                UserName = "admin",
                Password = "123",
                Role = adminRole
            };
            await dbContext.Users.AddAsync(adminUser);
            await dbContext.SaveChangesAsync();
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Logout endpoint
app.MapGet("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapStaticAssets();
app.MapRazorComponents<OrderSystemManagment.web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
