using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Employee;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Constants;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Queries.Employee;

public sealed class GetStaffPermissionsQueryHandler : IQueryHandler<Query.GetStaffPermissionsQuery, List<Response.StaffPermissionResponse>>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IRepositoryBase<RestaurantUser, int> _restaurantUserRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public GetStaffPermissionsQueryHandler(
        UserManager<Domain.Entities.User> userManager,
        IRepositoryBase<RestaurantUser, int> restaurantUserRepository,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _restaurantUserRepository = restaurantUserRepository;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<Result<List<Response.StaffPermissionResponse>>> Handle(
        Query.GetStaffPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Lấy restaurantId từ token
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure<List<Response.StaffPermissionResponse>>(
                new Error("NO_RESTAURANT", "Bạn cần chọn cửa hàng trước."));
        }

        // 2. Lấy Staff
        var staff = await _userManager.FindByIdAsync(request.StaffId.ToString());
        if (staff is null)
        {
            return Result.Failure<List<Response.StaffPermissionResponse>>(
                new Error("NOT_FOUND", "Không tìm thấy nhân viên."));
        }

        // 3. Kiểm tra Staff thuộc Restaurant của Owner
        var staffRestaurantUser = await _restaurantUserRepository.FindSingleAsync(
            x => x.UserId == request.StaffId
              && x.RestaurantId == restaurantId.Value
              && x.Status == ERestaurantUserStatus.Active,
            cancellationToken);

        if (staffRestaurantUser is null)
        {
            return Result.Failure<List<Response.StaffPermissionResponse>>(
                new Error("FORBIDDEN", "Nhân viên này không thuộc cửa hàng của bạn."));
        }

        // 4. Lấy UserClaims của Staff
        var staffClaims = await _userManager.GetClaimsAsync(staff);
        var permissionClaims = staffClaims
            .Where(c => PermissionConstants.PermissionKeys.Contains(c.Type))
            .ToList();

        // 5. Lấy danh sách Functions để map tên và MaxActionValue
        var functions = await _context.Functions
            .Where(f => f.Status == Status.Active)
            .ToDictionaryAsync(f => f.Key, f => new { f.Name, f.ActionValue }, cancellationToken);

        // 6. Build response
        var result = permissionClaims.Select(c =>
        {
            int.TryParse(c.Value, out var actionValue);
            var function = functions.TryGetValue(c.Type, out var func) ? func : null;

            return new Response.StaffPermissionResponse(
                PermissionKey: c.Type,
                PermissionName: function?.Name ?? c.Type,
                ActionValue: actionValue,
                MaxActionValue: function?.ActionValue ?? 0,
                AllowedActions: GetAllowedActions(actionValue)
            );
        }).ToList();

        return Result.Success(result);
    }

    private List<string> GetAllowedActions(int actionValue)
    {
        var actions = new List<string>();
        if ((actionValue & (int)ActionType.View) != 0) actions.Add("View");
        if ((actionValue & (int)ActionType.Add) != 0) actions.Add("Add");
        if ((actionValue & (int)ActionType.Update) != 0) actions.Add("Update");
        if ((actionValue & (int)ActionType.Delete) != 0) actions.Add("Delete");
        return actions;
    }
}
