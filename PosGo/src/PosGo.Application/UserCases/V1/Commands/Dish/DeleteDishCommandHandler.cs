using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Dish;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.Dish;

public sealed class DeleteDishCommandHandler : ICommandHandler<Command.DeleteDishCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;

    public DeleteDishCommandHandler(IRepositoryBase<Domain.Entities.Dish, int> dishRepository)
    {
        _dishRepository = dishRepository;
    }

    public async Task<Result> Handle(Command.DeleteDishCommand request, CancellationToken cancellationToken)
    {
        // Find Dish
        var dish = await _dishRepository.FindByIdAsync(request.Id, cancellationToken);
        if (dish == null)
        {
            return Result.Failure(new Error("DISH_NOT_FOUND", "Không tìm thấy món ăn."));
        }

        // Check if dish is being used in orders
        var hasOrders = await _dishRepository.FindAll()
            .Where(d => d.Id == request.Id)
            .SelectMany(d => d.OrderItems)
            .AnyAsync(cancellationToken);

        if (hasOrders)
        {
            return Result.Failure(new Error("DISH_IN_USE", "Không thể xóa món ăn đang được sử dụng trong đơn hàng."));
        }

        // Soft delete
        _dishRepository.Remove(dish);

        return Result.Success();
    }
}