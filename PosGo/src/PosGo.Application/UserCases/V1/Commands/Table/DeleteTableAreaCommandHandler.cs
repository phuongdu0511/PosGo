using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Table;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Table;

public sealed class DeleteTableAreaCommandHandler : ICommandHandler<Command.DeleteTableAreaCommand>
{
    private readonly IRepositoryBase<TableArea, int> _tableAreaRepository;
    private readonly IRepositoryBase<Domain.Entities.Table, int> _tableRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteTableAreaCommandHandler(
        IRepositoryBase<TableArea, int> tableAreaRepository,
        IRepositoryBase<Domain.Entities.Table, int> tableRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _tableAreaRepository = tableAreaRepository;
        _tableRepository = tableRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(Command.DeleteTableAreaCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();
        
        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // TenantFilter đã tự động filter theo RestaurantId
        var tableArea = await _tableAreaRepository.FindByIdAsync(request.Id);
        if (tableArea == null)
        {
            return Result.Failure(new Error("AREA_NOT_FOUND", "Không tìm thấy khu vực."));
        }

        // Kiểm tra có bàn nào đang sử dụng khu vực này không - TenantFilter đã tự động filter
        var hasActiveTables = await _tableRepository.FindAll(x => x.AreaId == request.Id)
            .AnyAsync(cancellationToken);

        if (hasActiveTables)
        {
            return Result.Failure(new Error("AREA_HAS_TABLES", "Không thể xóa khu vực đang có bàn."));
        }

        _tableAreaRepository.Remove(tableArea);

        return Result.Success();
    }
}