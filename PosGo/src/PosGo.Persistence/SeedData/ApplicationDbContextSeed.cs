using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Enumerations;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Constants;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Persistence.SeedData;

public static class ApplicationDbContextSeed
{
    public static async Task InitializeDatabase(ApplicationDbContext context)
    {
        await SeedDataFunctions(context);
    }

    public static async Task InitializeIdentityServerDatabase(RoleManager<Role> roleManager, UserManager<User> userManager)
    {
        var adminUserName = "admin";
        var password = "Phuongduy0511!";
        var adminRoleName = "SystemAdmin";
        var userAdmin = await userManager.FindByNameAsync(adminUserName);
        if (userAdmin == null)
        {
            userAdmin = new User
            {
                Id = Guid.NewGuid(),
                UserName = adminUserName,
                FullName = adminUserName,
                Email = $"{adminUserName.ToLower()}@gmail.com",
                EmailConfirmed = true, // Set for admin,
                CreatedByUserId = Guid.Parse("F3F48780-6A03-4C88-AFB5-E42A01D395F8"),
                UpdatedByUserId = Guid.Parse("F3F48780-6A03-4C88-AFB5-E42A01D395F8")
            };
            await userManager.CreateAsync(userAdmin, password);
        }

        if (!(await roleManager.RoleExistsAsync(adminRoleName)))
        {
            var role = new Role
            {
                Scope = "SYSTEM",
                Name = adminRoleName,
                RoleCode = adminRoleName.ToLower(),
                Description = $"Role for {adminUserName}",
                Id = Guid.NewGuid(),
                CreatedByUserId = Guid.Parse("F3F48780-6A03-4C88-AFB5-E42A01D395F8"),
                UpdatedByUserId = Guid.Parse("F3F48780-6A03-4C88-AFB5-E42A01D395F8")
            };
            await roleManager.CreateAsync(role);
            await userManager.AddToRoleAsync(userAdmin, role.Name);

            //add role claims
            role = await roleManager.FindByNameAsync(role.Name);
            var roleClaims = await roleManager.GetClaimsAsync(role);

            foreach (var claim in roleClaims)
            {
                await roleManager.RemoveClaimAsync(role, claim);
            }

            var actionValue1 = EnumHelper<ActionType>.GetValues().Sum(p => (int)p);

            foreach (var key in PermissionConstants.PermissionKeys)
            {
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(key, $"{actionValue1}"));
            }

            // add user claims
            var userClaims = await userManager.GetClaimsAsync(userAdmin);

            foreach (var claim in userClaims)
            {
                await userManager.RemoveClaimAsync(userAdmin, claim);
            }

            var actionValue2 = EnumHelper<ActionType>.GetValues().Sum(p => (int)p);
            foreach (var key in PermissionConstants.PermissionKeys)
            {
                await userManager.AddClaimAsync(userAdmin, new System.Security.Claims.Claim(key, $"{actionValue2}"));
            }
        }
    }

    public static async Task InitializeGuestServerDatabase(RoleManager<Role> roleManager, UserManager<User> userManager)
    {
        var adminUserName = "owner1";
        var password = "Owner@123";
        var adminRoleName = "Owner";
        var userAdmin = await userManager.FindByNameAsync(adminUserName);
        if (userAdmin == null)
        {
            userAdmin = new User
            {
                Id = Guid.NewGuid(),
                UserName = adminUserName,
                FullName = adminUserName,
                Email = $"{adminUserName.ToLower()}@gmail.com",
                EmailConfirmed = true, // Set for user test
                CreatedByUserId = Guid.Parse("F3F48780-6A03-4C88-AFB5-E42A01D395F8"),
                UpdatedByUserId = Guid.Parse("F3F48780-6A03-4C88-AFB5-E42A01D395F8")
            };
            await userManager.CreateAsync(userAdmin, password);
        }

        if (!(await roleManager.RoleExistsAsync(adminRoleName)))
        {
            var role = new Role
            {
                Scope = "RESTAURANT",
                Name = adminRoleName,
                RoleCode = adminRoleName.ToLower(),
                Description = $"Role for {adminUserName}",
                Id = Guid.NewGuid(),
                CreatedByUserId = Guid.Parse("F3F48780-6A03-4C88-AFB5-E42A01D395F8"),
                UpdatedByUserId = Guid.Parse("F3F48780-6A03-4C88-AFB5-E42A01D395F8")
            };
            await roleManager.CreateAsync(role);
            await userManager.AddToRoleAsync(userAdmin, role.Name);

            //add role claims
            role = await roleManager.FindByNameAsync(role.Name);
            var roleClaims = await roleManager.GetClaimsAsync(role);

            foreach (var claim in roleClaims)
            {
                await roleManager.RemoveClaimAsync(role, claim);
            }

            var actionValue1 = (int)EnumHelper<ActionType>.GetValues()[0];

            foreach (var key in PermissionConstants.PermissionKeys)
            {
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(key, $"{actionValue1}"));
            }

            // add user claims
            var userClaims = await userManager.GetClaimsAsync(userAdmin);

            foreach (var claim in userClaims)
            {
                await userManager.RemoveClaimAsync(userAdmin, claim);
            }

            var actionValue2 = (int)EnumHelper<ActionType>.GetValues()[0];
            foreach (var key in PermissionConstants.PermissionKeys)
            {
                await userManager.AddClaimAsync(userAdmin, new System.Security.Claims.Claim(key, $"{actionValue2}"));
            }
        }
    }

    private static async Task SeedDataFunctions(ApplicationDbContext context)
    {
        //if (!(await context.Functions.AnyAsync()))
        //{
        //    context.Functions.Add(new Function
        //    {
        //        Id = 1,
        //        Key = PermissionConstants.ManageRestaurantGroups,
        //        ActionValue = 15,
        //        Code = PermissionConstants.ManageRestaurantGroups,
        //        Name = "Quản lý nhóm cửa hàng",
        //        Url = @"/" + PermissionConstants.ManageRestaurantGroups,
        //        Status = Status.Active,
        //        SortOrder = 11
        //    });

        //    context.Functions.Add(new Function
        //    {
        //        Id = 2,
        //        Key = PermissionConstants.ManageRestaurants,
        //        ActionValue = 15,
        //        Code = PermissionConstants.ManageRestaurants,
        //        Name = "Quản lý cửa hàng",
        //        Url = @"/" + PermissionConstants.ManageRestaurants,
        //        Status = Status.Active,
        //        SortOrder = 10
        //    });

        //    context.Functions.Add(new Function
        //    {
        //        Id = 3,
        //        Key = PermissionConstants.ManageUsers,
        //        ActionValue = 15,
        //        Code = PermissionConstants.ManageUsers,
        //        Name = "Quản lý người dùng",
        //        Url = @"/" + PermissionConstants.ManageUsers,
        //        Status = Status.Active,
        //        SortOrder = 9
        //    });

        //    context.Functions.Add(new Function
        //    {
        //        Id = 4,
        //        Key = PermissionConstants.Dashboard,
        //        ActionValue = 15,
        //        Code = PermissionConstants.Dashboard,
        //        Name = "Tổng quan",
        //        Url = @"/" + PermissionConstants.Dashboard,
        //        Status = Status.Active,
        //        SortOrder = 8
        //    });

        //    context.Functions.Add(new Function
        //    {
        //        Id = 5,
        //        Key = PermissionConstants.Report,
        //        ActionValue = 15,
        //        Code = PermissionConstants.Report,
        //        Name = "Báo cáo",
        //        Url = @"/" + PermissionConstants.Report,
        //        Status = Status.Active,
        //        SortOrder = 7
        //    });

        //    context.Functions.Add(new Function
        //    {
        //        Id = 6,
        //        Key = PermissionConstants.ManageOrders,
        //        ActionValue = 15,
        //        Code = PermissionConstants.ManageOrders,
        //        Name = "Quản lý đơn",
        //        Url = @"/" + PermissionConstants.ManageOrders,
        //        Status = Status.Active,
        //        SortOrder = 6
        //    });

        //    context.Functions.Add(new Function
        //    {
        //        Id = 7,
        //        Key = PermissionConstants.ManageTables,
        //        ActionValue = 15,
        //        Code = PermissionConstants.ManageTables,
        //        Name = "Quản lý bàn",
        //        Url = @"/" + PermissionConstants.ManageTables,
        //        Status = Status.Active,
        //        SortOrder = 5
        //    });

        //    context.Functions.Add(new Function
        //    {
        //        Id = 8,
        //        Key = PermissionConstants.ManageDishes,
        //        ActionValue = 15,
        //        Code = PermissionConstants.ManageDishes,
        //        Name = "Quản lý món",
        //        Url = @"/" + PermissionConstants.ManageDishes,
        //        Status = Status.Active,
        //        SortOrder = 4
        //    });

        //    context.Functions.Add(new Function
        //    {
        //        Id = 9,
        //        Key = PermissionConstants.ManageUnits,
        //        ActionValue = 15,
        //        Code = PermissionConstants.ManageUnits,
        //        Name = "Quản lý đơn vị",
        //        Url = @"/" + PermissionConstants.ManageUnits,
        //        Status = Status.Active,
        //        SortOrder = 3
        //    });

        //    context.Functions.Add(new Function
        //    {
        //        Id = 10,
        //        Key = PermissionConstants.ManageEmployees,
        //        ActionValue = 15,
        //        Code = PermissionConstants.ManageEmployees,
        //        Name = "Quản lý nhân viên",
        //        Url = @"/" + PermissionConstants.ManageEmployees,
        //        Status = Status.Active,
        //        SortOrder = 2
        //    });

        //    context.Functions.Add(new Function
        //    {
        //        Id = 11,
        //        Key = PermissionConstants.SwitchBranch,
        //        ActionValue = 15,
        //        Code = PermissionConstants.SwitchBranch,
        //        Name = "Chuyển chi nhánh",
        //        Url = @"/" + PermissionConstants.SwitchBranch,
        //        Status = Status.Active,
        //        SortOrder = 1
        //    });
        //    // New Permission Objects
        //    await context.SaveChangesAsync();
        //}
    }
}
