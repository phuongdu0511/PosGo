using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishCategory;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.DishCategory;

public sealed class DeleteDishCategoryCommandHandler : ICommandHandler<Command.DeleteDishCategoryCommand>
{
    private readonly IRepositoryBase<Domain.Entities.DishCategory, int> _categoryRepository;

    public DeleteDishCategoryCommandHandler(IRepositoryBase<Domain.Entities.DishCategory, int> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result> Handle(Command.DeleteDishCategoryCommand request, CancellationToken cancellationToken)
    {
        // Find Category
        var category = await _categoryRepository.FindByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result.Failure(new Error("CATEGORY_NOT_FOUND", "Không tìm thấy danh mục."));
        }

        // Check if category has dishes
        var hasDishes = await _categoryRepository.FindAll()
            .Where(c => c.Id == request.Id)
            .SelectMany(c => c.Dishes)
            .AnyAsync(cancellationToken);

        if (hasDishes)
        {
            return Result.Failure(new Error("CATEGORY_HAS_DISHES", "Không thể xóa danh mục đang chứa món ăn."));
        }

        _categoryRepository.Remove(category);

        return Result.Success();
    }
}