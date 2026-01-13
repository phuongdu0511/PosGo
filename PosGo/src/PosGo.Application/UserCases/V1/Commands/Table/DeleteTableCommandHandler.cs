using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Table;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Table;

public sealed class DeleteTableCommandHandler : ICommandHandler<Command.DeleteTableCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Table, int> _tableRepository;
    private readonly IRepositoryBase<Order, int> _orderRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteTableCommandHandler(
        IRepositoryBase<Domain.Entities.Table, int> tableRepository,
        IRepositoryBase<Order, int> orderRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _tableRepository = tableRepository;
        _orderRepository = orderRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(Command.DeleteTableCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();
        
        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // TenantFilter đã tự động filter theo RestaurantId
        var table = await _tableRepository.FindByIdAsync(request.Id);
        if (table == null)
        {
            return Result.Failure(new Error("TABLE_NOT_FOUND", "Không tìm thấy bàn."));
        }

        // Kiểm tra có đơn hàng nào đang sử dụng bàn này không - TenantFilter đã tự động filter
        var hasActiveOrders = await _orderRepository.FindAll(x => x.TableId == request.Id)
            .AnyAsync(cancellationToken);

        if (hasActiveOrders)
        {
            return Result.Failure(new Error("TABLE_HAS_ORDERS", "Không thể xóa bàn đang có đơn hàng."));
        }

        _tableRepository.Remove(table);

        return Result.Success();
    }
}