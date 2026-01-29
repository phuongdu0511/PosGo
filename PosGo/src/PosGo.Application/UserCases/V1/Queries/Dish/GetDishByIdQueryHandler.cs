using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Dish;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Queries.Dish;

public sealed class GetDishByIdQueryHandler : IQueryHandler<Query.GetDishByIdQuery, Response.DishDetailResponse>
{
    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;
    private readonly IMapper _mapper;

    public GetDishByIdQueryHandler(
        IRepositoryBase<Domain.Entities.Dish, int> dishRepository,
        IMapper mapper)
    {
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    public async Task<Result<Response.DishDetailResponse>> Handle(Query.GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        // Load all translations - frontend will handle language selection
        var dish = await _dishRepository.FindAll()
            .Include(d => d.Restaurant)
            .Include(d => d.Category)
            .ThenInclude(c => c.Translations)
            .ThenInclude(t => t.Language)
            .Include(d => d.Unit)
            .ThenInclude(u => u.Translations)
            .ThenInclude(t => t.Language)
            .Include(d => d.DishType)
            .Include(d => d.Translations)
            .ThenInclude(t => t.Language)
            .Include(d => d.Variants)
            .ThenInclude(v => v.Translations)
            .ThenInclude(t => t.Language)
            .Include(d => d.Variants)
            .ThenInclude(v => v.Options)
            .ThenInclude(o => o.Translations)
            .ThenInclude(t => t.Language)
            .Include(d => d.Skus)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (dish == null)
        {
            return Result.Failure<Response.DishDetailResponse>(
                new Error("DISH_NOT_FOUND", "Không tìm thấy món ăn."));
        }

        var result = _mapper.Map<Response.DishDetailResponse>(dish);
        return Result.Success(result);
    }
}