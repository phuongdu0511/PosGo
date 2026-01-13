using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Table;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Table;

public sealed class UpdateTableAreaCommandHandler : ICommandHandler<Command.UpdateTableAreaCommand>
{
    private readonly IRepositoryBase<TableArea, int> _tableAreaRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public UpdateTableAreaCommandHandler(
        IRepositoryBase<TableArea, int> tableAreaRepository,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _tableAreaRepository = tableAreaRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.UpdateTableAreaCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();
        
        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // TenantFilter đã tự động filter theo RestaurantId
        var tableArea = await _tableAreaRepository.FindAll(x => x.Id == request.Id)
            .Include(x => x.Tables)
            .FirstOrDefaultAsync(cancellationToken);
            
        if (tableArea == null)
        {
            return Result.Failure(new Error("AREA_NOT_FOUND", "Không tìm thấy khu vực."));
        }

        // Kiểm tra trùng tên (trừ chính nó) - TenantFilter đã tự động filter theo RestaurantId
        var existingArea = await _tableAreaRepository.FindSingleAsync(
            x => x.Name.ToLower() == request.Name.ToLower() && 
                 x.Id != request.Id);

        if (existingArea != null)
        {
            return Result.Failure(new Error("AREA_NAME_EXISTS", "Tên khu vực đã tồn tại."));
        }

        tableArea.Update(request.Name, request.SortOrder, request.IsActive);

        var result = _mapper.Map<Response.TableAreaResponse>(tableArea);
        return Result.Success(result);
    }
}