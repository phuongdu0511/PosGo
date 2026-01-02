using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Restaurant;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.Restaurant;

public sealed class CreateRestaurantCommandHandler : ICommandHandler<Command.CreateRestaurantCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Restaurant, Guid> _restaurantRepository;
    private readonly IRepositoryBase<Domain.Entities.Language, Guid> _languageRepository;
    private readonly IMapper _mapper;

    public CreateRestaurantCommandHandler(
        IRepositoryBase<Domain.Entities.Restaurant, Guid> restaurantRepository,
        IRepositoryBase<Domain.Entities.Language, Guid> languageRepository,
        IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _languageRepository = languageRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();

        var existedSlug = await _restaurantRepository.FindSingleAsync(r => r.Slug == slug);

        if (existedSlug is not null)
        {
            return Result.Failure<Response.RestaurantResponse>(
                new Error("SLUG_EXISTS", "Slug nhà hàng đã tồn tại."));
        }

        var langExists = await _languageRepository.FindSingleAsync(l => l.Id == request.DefaultLanguageId);

        if (langExists is null)
        {
            return Result.Failure<Response.RestaurantResponse>(
                new Error("LANGUAGE_NOT_FOUND", "Ngôn ngữ mặc định không tồn tại."));
        }

        var restaurant = Domain.Entities.Restaurant.Create(
            Guid.NewGuid(),
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
            request.RestaurantGroupId
        );
        _restaurantRepository.Add(restaurant);

        var result = _mapper.Map<Response.RestaurantResponse>(restaurant);
        return Result.Success(result);
    }
}
