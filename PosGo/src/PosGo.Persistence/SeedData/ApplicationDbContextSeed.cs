//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using PosGo.Domain.Entities;

//namespace PosGo.Persistence.SeedData;

//public static class ApplicationDbContextSeed
//{
//    public static async Task SeedAsync(IServiceProvider services)
//    {
//        using var scope = services.CreateScope();
//        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

//        if (!await db.Roles.AnyAsync())
//        {
//            var systemAdminRole = new Role
//            {
//                Id = Guid.NewGuid(),
//                Scope = "SYSTEM",
//                Code = "SystemAdmin",
//                Name = "System Administrator",
//                Rank = 999,
//                IsActive = true
//            };
//            db.Roles.Add(systemAdminRole);

//            await db.SaveChangesAsync();
//        }

//        if (!await db.Users.AnyAsync())
//        {
//            var admin = new User
//            {
//                Id = Guid.NewGuid(),
//                UserName = "admin",
//                FullName = "System Admin",
//                CreatedAt = DateTime.UtcNow,
//                UpdatedAt = DateTime.UtcNow
//            };

//            admin.Password = hasher.HashPassword(admin, "Admin@123");
//            db.Users.Add(admin);
//            await db.SaveChangesAsync();

//            var adminRole = await db.Roles.FirstAsync(r => r.Code == "SystemAdmin");
//            db.UserSystemRoles.Add(new UserSystemRole
//            {
//                UserId = admin.Id,
//                RoleId = adminRole.Id,
//            });

//            await db.SaveChangesAsync();
//        }
//    }
//}
