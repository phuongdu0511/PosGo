using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Restaurant;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Queries.Restaurant;

public sealed class GetRestaurantByIdQueryHandler : IQueryHandler<Query.GetRestaurantByIdQuery, Response.RestaurantResponse>
{
    private readonly IRepositoryBase<Domain.Entities.Restaurant, Guid> _restaurantRepository;
    private readonly IMapper _mapper;

    public GetRestaurantByIdQueryHandler(IRepositoryBase<Domain.Entities.Restaurant, Guid> restaurantRepository,
        IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }
    public async Task<Result<Response.RestaurantResponse>> Handle(Query.GetRestaurantByIdQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.FindByIdAsync(request.Id)
            ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(Restaurant));

        var result = _mapper.Map<Response.RestaurantResponse>(restaurant);

        return Result.Success(result);
    }
}
