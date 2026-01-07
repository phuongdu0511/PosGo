using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Plan;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.Plan;

public sealed class RemovePlanFunctionCommandHandler : ICommandHandler<Command.RemovePlanFunctionCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Plan, Guid> _planRepository;
    private readonly IRepositoryBase<Domain.Entities.PlanFunction, int> _planFunctionRepository;

    public RemovePlanFunctionCommandHandler(
        IRepositoryBase<Domain.Entities.Plan, Guid> planRepository,
        IRepositoryBase<Domain.Entities.PlanFunction, int> planFunctionRepository)
    {
        _planRepository = planRepository;
        _planFunctionRepository = planFunctionRepository;
    }

    public async Task<Result> Handle(Command.RemovePlanFunctionCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra Plan tồn tại
        var plan = await _planRepository.FindByIdAsync(request.PlanId);
        if (plan is null)
        {
            return Result.Failure(new Error("NOT_FOUND", $"Plan với Id {request.PlanId} không tồn tại."));
        }

        // 2. Kiểm tra PlanFunction tồn tại
        var planFunction = await _planFunctionRepository.FindSingleAsync(
            x => x.PlanId == request.PlanId && x.FunctionId == request.FunctionId,
            cancellationToken);

        if (planFunction is null)
        {
            return Result.Failure(new Error(
                "NOT_FOUND",
                "Function này không tồn tại trong gói."));
        }

        // 3. Xóa PlanFunction
        _planFunctionRepository.Remove(planFunction);

        return Result.Success();
    }
}
