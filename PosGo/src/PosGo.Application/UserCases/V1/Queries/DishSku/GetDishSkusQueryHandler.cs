using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishSku;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;

namespace PosGo.Application.UserCases.V1.Queries.DishSku;

public sealed class GetDishSkusQueryHandler : IQueryHandler<Query.GetDishSkusQuery, List<Response.DishSkuResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.DishSku, int> _skuRepository;
    private readonly IMapper _mapper;

    public GetDishSkusQueryHandler(
        IRepositoryBase<Domain.Entities.DishSku, int> skuRepository,
        IMapper mapper)
    {
        _skuRepository = skuRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<Response.DishSkuResponse>>> Handle(Query.GetDishSkusQuery request, CancellationToken cancellationToken)
    {
        var skus = await _skuRepository.FindAll(s => s.DishId == request.DishId)
            .Include(s => s.Dish)
            .ThenInclude(d => d.Translations)
            .Include(s => s.VariantOptions)
            .ThenInclude(vo => vo.VariantOption)
            .ThenInclude(vo => vo.Variant)
            .ThenInclude(v => v.Translations)
            .Include(s => s.VariantOptions)
            .ThenInclude(vo => vo.VariantOption)
            .ThenInclude(vo => vo.Translations)
            .OrderBy(s => s.IsDefault ? 0 : 1)
            .ThenBy(s => s.Code)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<Response.DishSkuResponse>>(skus);
        return Result.Success(result);
    }
}