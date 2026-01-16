using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Dish;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.Dish;

public sealed class UpdateDishMenuVisibilityCommandHandler : ICommandHandler<Command.UpdateDishMenuVisibilityCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;
    private readonly IMapper _mapper;

    public UpdateDishMenuVisibilityCommandHandler(
        IRepositoryBase<Domain.Entities.Dish, int> dishRepository,
        IMapper mapper)
    {
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.UpdateDishMenuVisibilityCommand request, CancellationToken cancellationToken)
    {
        // Find Dish with Translations
        var dish = await _dishRepository.FindByIdAsync(request.Id, cancellationToken, d => d.Translations, d => d.Translations.Select(t => t.Language));
        if (dish == null)
        {
            return Result.Failure(new Error("DISH_NOT_FOUND", "Không tìm thấy món ăn."));
        }

        // Update menu visibility
        if (request.ShowOnMenu)
            dish.ShowOnMenuDisplay();
        else
            dish.HideFromMenu();

        var result = _mapper.Map<Response.DishResponse>(dish);
        return Result.Success(result);
    }
}