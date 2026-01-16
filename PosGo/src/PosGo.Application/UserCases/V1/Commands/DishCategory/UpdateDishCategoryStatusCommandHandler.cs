using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishCategory;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.DishCategory;

public sealed class UpdateDishCategoryStatusCommandHandler : ICommandHandler<Command.UpdateDishCategoryStatusCommand>
{
    private readonly IRepositoryBase<Domain.Entities.DishCategory, int> _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateDishCategoryStatusCommandHandler(
        IRepositoryBase<Domain.Entities.DishCategory, int> categoryRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.UpdateDishCategoryStatusCommand request, CancellationToken cancellationToken)
    {
        // Find Category with Translations
        var category = await _categoryRepository.FindByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result.Failure(new Error("CATEGORY_NOT_FOUND", "Không tìm thấy danh mục."));
        }

        // Update status
        if (request.IsActive)
            category.Activate();
        else
            category.Deactivate();

        var result = _mapper.Map<Response.DishCategoryResponse>(category);
        return Result.Success(result);
    }
}