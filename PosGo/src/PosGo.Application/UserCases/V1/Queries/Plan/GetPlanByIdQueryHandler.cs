using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Plan;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Queries.Plan;

public sealed class GetPlanByIdQueryHandler : IQueryHandler<Query.GetPlanByIdQuery, Response.PlanResponse>
{
    private readonly IRepositoryBase<Domain.Entities.Plan, Guid> _planRepository;
    private readonly IMapper _mapper;

    public GetPlanByIdQueryHandler(
        IRepositoryBase<Domain.Entities.Plan, Guid> planRepository,
        IMapper mapper)
    {
        _planRepository = planRepository;
        _mapper = mapper;
    }

    public async Task<Result<Response.PlanResponse>> Handle(Query.GetPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var plan = await _planRepository.FindByIdAsync(request.Id)
            ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(Domain.Entities.Plan));

        var result = _mapper.Map<Response.PlanResponse>(plan);

        return Result.Success(result);
    }
}
