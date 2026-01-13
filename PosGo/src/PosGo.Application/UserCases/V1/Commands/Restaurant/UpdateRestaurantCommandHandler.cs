using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Restaurant;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Commands.Restaurant;

public sealed class UpdateRestaurantCommandHandler : ICommandHandler<Command.UpdateRestaurantCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Restaurant, Guid> _restaurantRepository;
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IMapper _mapper;

    public UpdateRestaurantCommandHandler(
        IRepositoryBase<Domain.Entities.Restaurant, Guid> restaurantRepository,
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _languageRepository = languageRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.FindByIdAsync(request.Id) ??
            throw new CommonNotFoundException.CommonException(request.Id, nameof(Restaurant));

        var slug = request.Slug.Trim().ToLowerInvariant();

        var existedSlug = await _restaurantRepository.FindSingleAsync(r => r.Slug == slug, cancellationToken);

        if (existedSlug is not null)
        {
            return Result.Failure<Response.RestaurantResponse>(
                new Error("SLUG_EXISTS", "Slug nhà hàng đã tồn tại."));
        }

        var langExists = await _languageRepository.FindSingleAsync(l => l.Id == request.DefaultLanguageId, cancellationToken);

        if (langExists is null)
        {
            return Result.Failure<Response.RestaurantResponse>(
                new Error("LANGUAGE_NOT_FOUND", "Ngôn ngữ mặc định không tồn tại."));
        }

        restaurant.Update(
            request.Name,
            slug,
            request.DefaultLanguageId,
            request.Address,
            request.City,
            request.Country,
            request.Phone,
            request.TimeZone,
            request.LogoUrl,
            request.Description,
            request.RestaurantGroupId,
            request.IsActive
        );

        var result = _mapper.Map<Response.RestaurantResponse>(restaurant);
        return Result.Success(result);
    }
}
