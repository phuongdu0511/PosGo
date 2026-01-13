using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Extensions;
using PosGo.Contract.Services.V1.Table;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Persistence;
using PosGo.Persistence.Constants;

namespace PosGo.Application.UserCases.V1.Queries.Table;

public sealed class GetTablesByRestaurantQueryHandler : IQueryHandler<Query.GetTablesByRestaurantQuery, PagedResult<Response.TableResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Table, int> _tableRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public GetTablesByRestaurantQueryHandler(
        IRepositoryBase<Domain.Entities.Table, int> tableRepository,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        ApplicationDbContext context)
    {
        _tableRepository = tableRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _context = context;
    }

    public async Task<Result<PagedResult<Response.TableResponse>>> Handle(Query.GetTablesByRestaurantQuery request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();
        
        if (!restaurantId.HasValue)
        {
            return Result.Failure<PagedResult<Response.TableResponse>>(
                new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        if (request.SortColumnAndOrder?.Any() == true) // Raw Query when order by multi column
        {
            var PageIndex = request.PageIndex <= 0 ? PagedResult<Domain.Entities.Table>.DefaultPageIndex : request.PageIndex;
            var PageSize = request.PageSize <= 0
                ? PagedResult<Domain.Entities.Table>.DefaultPageSize
                : request.PageSize > PagedResult<Domain.Entities.Table>.UpperPageSize
                ? PagedResult<Domain.Entities.Table>.UpperPageSize : request.PageSize;

            // Build base query
            var tablesQuery = $"SELECT t.*, ta.Name as AreaName FROM {nameof(TableNames.Tables)} t LEFT JOIN {nameof(TableNames.TableAreas)} ta ON t.AreaId = ta.Id";
            var parameters = new List<object>();

            if (request.AreaId.HasValue)
            {
                tablesQuery = tablesQuery.AddWhereCondition("t.AreaId", ref parameters, request.AreaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var hasWhere = tablesQuery.ToUpper().Contains(" WHERE ");
                var connector = hasWhere ? " AND " : " WHERE ";
                var orderByIndex = tablesQuery.ToUpper().IndexOf(" ORDER BY");
                var insertPosition = orderByIndex > 0 ? orderByIndex : tablesQuery.Length;
                
                tablesQuery = tablesQuery.Insert(insertPosition, $"{connector}t.Name LIKE {{{parameters.Count}}}");
                parameters.Add($"%{request.SearchTerm}%");
            }

            if (request.IsActive.HasValue)
            {
                tablesQuery = tablesQuery.AddWhereCondition("t.IsActive", ref parameters, request.IsActive.Value);
            }

            tablesQuery += " ORDER BY ";

            foreach (var item in request.SortColumnAndOrder)
                tablesQuery += item.Value == SortOrder.Descending
                    ? $"t.{item.Key} DESC, "
                    : $"t.{item.Key} ASC, ";

            tablesQuery = tablesQuery.Remove(tablesQuery.Length - 2);
            tablesQuery += $" OFFSET {(PageIndex - 1) * PageSize} ROWS FETCH NEXT {PageSize} ROWS ONLY";

            var tables = await _tableRepository.FromSqlRawAsync(tablesQuery, parameters.ToArray(), x => x.Area);

            var totalCount = await _tableRepository.FindAll().CountAsync(cancellationToken);

            var tablePagedResult = PagedResult<Domain.Entities.Table>.Create(tables, PageIndex, PageSize, totalCount);
            var result = _mapper.Map<PagedResult<Response.TableResponse>>(tablePagedResult);

            return Result.Success(result);
        }
        else // Entity Framework
        {
            IQueryable<Domain.Entities.Table> tablesQuery = _tableRepository.FindAll()
                .Include(x => x.Area);

            // Apply filters
            if (request.AreaId.HasValue)
            {
                tablesQuery = tablesQuery.Where(x => x.AreaId == request.AreaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                tablesQuery = tablesQuery.Where(x => x.Name.Contains(request.SearchTerm));
            }

            if (request.IsActive.HasValue)
            {
                tablesQuery = tablesQuery.Where(x => x.IsActive == request.IsActive.Value);
            }

            // Apply sorting
            tablesQuery = request.SortOrder == SortOrder.Descending
                ? tablesQuery.OrderByDescending(GetSortProperty(request))
                : tablesQuery.OrderBy(GetSortProperty(request));

            var tables = await PagedResult<Domain.Entities.Table>.CreateAsync(tablesQuery, request.PageIndex, request.PageSize);
            var result = _mapper.Map<PagedResult<Response.TableResponse>>(tables);

            return Result.Success(result);
        }
    }

    private static Expression<Func<Domain.Entities.Table, object>> GetSortProperty(Query.GetTablesByRestaurantQuery request)
        => request.SortColumn?.ToLower() switch
        {
            "name" => table => table.Name,
            "areaname" => table => table.Area != null ? table.Area.Name : "",
            "isactive" => table => table.IsActive,
            "seats" => table => table.Seats ?? 0,
            _ => table => table.CreatedAt // Default Sort Descending on CreatedDate column
        };
}