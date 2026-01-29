using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishVariant;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;

namespace PosGo.Application.UserCases.V1.Queries.DishVariant;

public sealed class GetDishVariantTreeQueryHandler : IQueryHandler<Query.GetDishVariantTreeQuery, Response.DishVariantTreeResponse>
{
    private readonly IRepositoryBase<Domain.Entities.DishVariant, int> _variantRepository;
    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;
    private readonly IMapper _mapper;

    public GetDishVariantTreeQueryHandler(
        IRepositoryBase<Domain.Entities.DishVariant, int> variantRepository,
        IRepositoryBase<Domain.Entities.Dish, int> dishRepository,
        IMapper mapper)
    {
        _variantRepository = variantRepository;
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    public async Task<Result<Response.DishVariantTreeResponse>> Handle(Query.GetDishVariantTreeQuery request, CancellationToken cancellationToken)
    {
        var language = request.Language ?? "vi";

        // Get Dish
        var dish = await _dishRepository.FindAll()
            .Include(d => d.Translations.Where(t => t.Language.Code == language))
            .FirstOrDefaultAsync(d => d.Id == request.DishId, cancellationToken);

        if (dish == null)
        {
            return Result.Failure<Response.DishVariantTreeResponse>(
                new Error("DISH_NOT_FOUND", "Không tìm thấy món ăn."));
        }

        // Get Variants with Options
        var variants = await _variantRepository.FindAll(v => v.DishId == request.DishId)
            .Include(v => v.Translations.Where(t => t.Language.Code == language))
            .ThenInclude(t => t.Language)
            .Include(v => v.Options)
            .ThenInclude(o => o.Translations.Where(t => t.Language.Code == language))
            .ThenInclude(t => t.Language)
            .OrderBy(v => v.SortOrder)
            .ToListAsync(cancellationToken);

        var result = new Response.DishVariantTreeResponse(
            dish.Id,
            dish.Translations.FirstOrDefault()?.Name ?? "",
            _mapper.Map<List<Response.DishVariantResponse>>(variants)
        );

        return Result.Success(result);
    }
}