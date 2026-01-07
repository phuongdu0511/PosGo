using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Employee;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Queries.Employee;

public sealed class GetStaffByIdQueryHandler : IQueryHandler<Query.GetStaffByIdQuery, Response.StaffDetailResponse>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IRepositoryBase<RestaurantUser, int> _restaurantUserRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStaffByIdQueryHandler(
        UserManager<Domain.Entities.User> userManager,
        IRepositoryBase<RestaurantUser, int> restaurantUserRepository,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context,
        IMapper mapper)
    {
        _userManager = userManager;
        _restaurantUserRepository = restaurantUserRepository;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<Response.StaffDetailResponse>> Handle(
        Query.GetStaffByIdQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Lấy restaurantId từ token
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure<Response.StaffDetailResponse>(
                new Error("NO_RESTAURANT", "Bạn cần chọn cửa hàng trước."));
        }

        // 2. Lấy Staff
        var staff = await _userManager.FindByIdAsync(request.StaffId.ToString());
        if (staff is null)
        {
            return Result.Failure<Response.StaffDetailResponse>(
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
            return Result.Failure<Response.StaffDetailResponse>(
                new Error("FORBIDDEN", "Nhân viên này không thuộc cửa hàng của bạn."));
        }

        // 4. Lấy thông tin Restaurant
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurantId.Value, cancellationToken);

        // 5. Build response
        var result = new Response.StaffDetailResponse(
            Id: staff.Id,
            UserName: staff.UserName ?? string.Empty,
            FullName: staff.FullName,
            PhoneNumber: staff.PhoneNumber,
            Status: staff.Status,
            RestaurantId: restaurantId.Value,
            RestaurantName: restaurant?.Name ?? string.Empty
        );

        return Result.Success(result);
    }
}
