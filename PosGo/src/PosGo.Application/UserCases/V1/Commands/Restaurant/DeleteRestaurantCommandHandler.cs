using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Restaurant;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Commands.Restaurant;

public sealed class DeleteRestaurantCommandHandler : ICommandHandler<Command.DeleteRestaurantCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Restaurant, Guid> _restaurantRepository;
    public DeleteRestaurantCommandHandler(IRepositoryBase<Domain.Entities.Restaurant, Guid> restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task<Result> Handle(Command.DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.FindByIdAsync(request.RestaurantId) ??
            throw new CommonNotFoundException.CommonException(request.RestaurantId, nameof(Restaurant));
        _restaurantRepository.Remove(restaurant);
        return Result.Success();
    }
}
