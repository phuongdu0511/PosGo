using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishSku;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;

namespace PosGo.Application.UserCases.V1.Queries.DishSku;

public sealed class GetDefaultSkuQueryHandler : IQueryHandler<Query.GetDefaultSkuQuery, Response.DishSkuResponse>
{
    private readonly IRepositoryBase<Domain.Entities.DishSku, int> _skuRepository;
    private readonly IMapper _mapper;

    public GetDefaultSkuQueryHandler(
        IRepositoryBase<Domain.Entities.DishSku, int> skuRepository,
        IMapper mapper)
    {
        _skuRepository = skuRepository;
        _mapper = mapper;
    }

    public async Task<Result<Response.DishSkuResponse>> Handle(Query.GetDefaultSkuQuery request, CancellationToken cancellationToken)
    {
        var sku = await _skuRepository.FindAll(s => s.DishId == request.DishId && s.IsActive)
            .Include(s => s.Dish)
            .ThenInclude(d => d.Translations)
            .Include(s => s.VariantOptions)
            .ThenInclude(vo => vo.VariantOption)
            .ThenInclude(vo => vo.Variant)
            .ThenInclude(v => v.Translations)
            .Include(s => s.VariantOptions)
            .ThenInclude(vo => vo.VariantOption)
            .ThenInclude(vo => vo.Translations)
            .FirstOrDefaultAsync(cancellationToken);

        if (sku == null)
        {
            return Result.Failure<Response.DishSkuResponse>(
                new Error("DEFAULT_SKU_NOT_FOUND", "Không tìm thấy SKU mặc định cho món ăn này."));
        }

        var result = _mapper.Map<Response.DishSkuResponse>(sku);
        return Result.Success(result);
    }
}