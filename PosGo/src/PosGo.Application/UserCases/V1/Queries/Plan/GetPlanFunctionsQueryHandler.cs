using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Plan;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Queries.Plan;

public sealed class GetPlanFunctionsQueryHandler : IQueryHandler<Query.GetPlanFunctionsQuery, List<Response.PlanFunctionResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Plan, Guid> _planRepository;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPlanFunctionsQueryHandler(
        IRepositoryBase<Domain.Entities.Plan, Guid> planRepository,
        ApplicationDbContext context,
        IMapper mapper)
    {
        _planRepository = planRepository;
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<Response.PlanFunctionResponse>>> Handle(Query.GetPlanFunctionsQuery request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra Plan tồn tại
        var plan = await _planRepository.FindByIdAsync(request.PlanId);
        if (plan is null)
        {
            return Result.Failure<List<Response.PlanFunctionResponse>>(
                new Error("NOT_FOUND", $"Plan với Id {request.PlanId} không tồn tại."));
        }

        // 2. Lấy danh sách PlanFunction kèm Function
        var planFunctions = await (
            from pf in _context.PlanFunctions
            join f in _context.Functions on pf.FunctionId equals f.Id
            where pf.PlanId == request.PlanId
            select new Response.PlanFunctionResponse(
                pf.Id,
                pf.FunctionId,
                f.Name,
                f.Key,
                pf.ActionValue,
                pf.IsActive
            )
        ).ToListAsync(cancellationToken);

        return Result.Success(planFunctions);
    }
}
