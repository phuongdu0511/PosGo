using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Table;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Queries.Table;

public sealed class GetTablesByAreaQueryHandler : IQueryHandler<Query.GetTablesByAreaQuery, List<Response.TableResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Table, Guid> _tableRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public GetTablesByAreaQueryHandler(
        IRepositoryBase<Domain.Entities.Table, Guid> tableRepository,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _tableRepository = tableRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<Result<List<Response.TableResponse>>> Handle(Query.GetTablesByAreaQuery request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();
        
        if (!restaurantId.HasValue)
        {
            return Result.Failure<List<Response.TableResponse>>(
                new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // TenantFilter đã tự động filter theo RestaurantId
        var tables = await _tableRepository.FindAll(x => x.AreaId == request.AreaId && x.IsActive)
            .Include(x => x.Area)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<Response.TableResponse>>(tables);
        return Result.Success(result);
    }
}