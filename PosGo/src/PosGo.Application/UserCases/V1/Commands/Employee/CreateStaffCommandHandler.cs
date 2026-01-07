using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Services.V1.Employee;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Commands.Employee;

public sealed class CreateStaffCommandHandler : ICommandHandler<Command.CreateStaffCommand, Response.StaffResponse>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly RoleManager<Domain.Entities.Role> _roleManager;
    private readonly IRepositoryBase<Domain.Entities.RestaurantUser, int> _restaurantUserRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateStaffCommandHandler(
        UserManager<Domain.Entities.User> userManager,
        RoleManager<Domain.Entities.Role> roleManager,
        IRepositoryBase<Domain.Entities.RestaurantUser, int> restaurantUserRepository,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context,
        IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _restaurantUserRepository = restaurantUserRepository;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<Response.StaffResponse>> Handle(Command.CreateStaffCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin Owner đang đăng nhập
        var httpContext = _httpContextAccessor.HttpContext;
        var ownerId = httpContext.GetCurrentUserId();
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure<Response.StaffResponse>(new Error(
                "NO_RESTAURANT",
                "Bạn cần chọn cửa hàng trước khi tạo nhân viên."));
        }

        // 2. Kiểm tra UserName đã tồn tại
        var userExisted = await _userManager.FindByNameAsync(request.UserName);
        if (userExisted is not null)
        {
            return Result.Failure<Response.StaffResponse>(new Error("EXISTED", "UserName đã tồn tại"));
        }

        // 3. Kiểm tra Password và ConfirmPassword
        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return Result.Failure<Response.StaffResponse>(new Error(
                "PASSWORD_CONFIRM_NOT_MATCH",
                "Mật khẩu mới và xác nhận mật khẩu không trùng nhau."));
        }

        // 4. Lấy Role Staff
        var staffRole = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.RoleCode == SystemConstants.RoleCode.STAFF, cancellationToken);

        if (staffRole is null)
        {
            return Result.Failure<Response.StaffResponse>(new Error("ROLE_NOT_FOUND", "Không tìm thấy role Staff."));
        }

        // 5. Tạo User
        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
        };

        var createdResult = await _userManager.CreateAsync(user, request.Password);
        if (!createdResult.Succeeded)
        {
            var codes = createdResult.Errors
                .Select(e => e.Code)
                .Distinct()
                .ToList();

            return Result.Failure<Response.StaffResponse>(new Error(
                code: JsonSerializer.Serialize(codes),
                message: "VALIDATION_FAILED"
            ));
        }

        // 6. Gán role Staff cho user
        await _userManager.AddToRoleAsync(user, staffRole.Name);

        // 7. Assign Staff vào Restaurant của Owner
        var restaurantUser = RestaurantUser.Create(
            restaurantId: restaurantId.Value,
            userId: user.Id,
            roleId: staffRole.Id);

        _restaurantUserRepository.Add(restaurantUser);

        // 8. Map và trả về StaffResponse
        var result = _mapper.Map<Response.StaffResponse>(user);
        return Result.Success(result);
    }
}
