using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PosGo.Domain.Entities;
using PosGo.Persistence.SeedData;

namespace PosGo.Persistence.DependencyInjection.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrationsPersistence(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.MigrateAsync().Wait();
    }

    public static async Task SeedIdentityDataPersistence(this IApplicationBuilder app)
    {
        using (IServiceScope scope = app.ApplicationServices.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            await ApplicationDbContextSeed.InitializeIdentityServerDatabase(roleManager, userManager);
            await ApplicationDbContextSeed.InitializeGuestServerDatabase(roleManager, userManager);
        }
    }
    public static async Task SeedFunctionDataPersistence(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await ApplicationDbContextSeed.InitializeDatabase(dbContext);
    }

}
