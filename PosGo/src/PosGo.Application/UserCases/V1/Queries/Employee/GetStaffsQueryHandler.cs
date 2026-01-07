using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

public sealed class GetStaffsQueryHandler : IQueryHandler<Query.GetStaffsQuery, PagedResult<Response.StaffResponse>>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IRepositoryBase<RestaurantUser, int> _restaurantUserRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStaffsQueryHandler(
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

    public async Task<Result<PagedResult<Response.StaffResponse>>> Handle(
        Query.GetStaffsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Lấy restaurantId từ token
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure<PagedResult<Response.StaffResponse>>(
                new Error("NO_RESTAURANT", "Bạn cần chọn cửa hàng trước."));
        }

        // 2. Lấy danh sách UserId của Staff thuộc Restaurant này
        var staffUserIds = await _restaurantUserRepository
            .FindAll(x => x.RestaurantId == restaurantId.Value
                       && x.Status == ERestaurantUserStatus.Active)
            .Join(_context.Roles, ru => ru.RoleId, r => r.Id, (ru, r) => new { ru.UserId, r.RoleCode })
            .Where(x => x.RoleCode == SystemConstants.RoleCode.STAFF || x.RoleCode == SystemConstants.RoleCode.MANAGER)
            .Select(x => x.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!staffUserIds.Any())
        {
            var emptyResult = PagedResult<Response.StaffResponse>.Create(
                new List<Response.StaffResponse>(),
                request.PageIndex > 0 ? request.PageIndex : 1,
                request.PageSize > 0 ? request.PageSize : 10,
                0);
            return Result.Success(emptyResult);
        }

        // 3. Query Users với filter
        var usersQuery = _userManager.Users
            .Where(u => staffUserIds.Contains(u.Id));

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            usersQuery = usersQuery.Where(u =>
                u.FullName.Contains(request.SearchTerm) ||
                u.UserName.Contains(request.SearchTerm) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(request.SearchTerm)));
        }

        // 4. Sort
        if (request.SortColumnAndOrder?.Any() == true)
        {
            // Multi-column sort - sử dụng raw SQL nếu cần
            // Tạm thời dùng single column sort
            usersQuery = request.SortOrder == SortOrder.Descending
                ? usersQuery.OrderByDescending(GetSortProperty(request))
                : usersQuery.OrderBy(GetSortProperty(request));
        }
        else
        {
            usersQuery = request.SortOrder == SortOrder.Descending
                ? usersQuery.OrderByDescending(GetSortProperty(request))
                : usersQuery.OrderBy(GetSortProperty(request));
        }

        // 5. Pagination
        var users = await PagedResult<Domain.Entities.User>.CreateAsync(
            usersQuery,
            request.PageIndex,
            request.PageSize);

        // 6. Map sang Response
        var result = _mapper.Map<PagedResult<Response.StaffResponse>>(users);
        return Result.Success(result);
    }

    private static Expression<Func<Domain.Entities.User, object>> GetSortProperty(Query.GetStaffsQuery request)
        => request.SortColumn?.ToLower() switch
        {
            "fullname" => user => user.FullName,
            "username" => user => user.UserName,
            _ => user => user.CreatedAt
        };
}
