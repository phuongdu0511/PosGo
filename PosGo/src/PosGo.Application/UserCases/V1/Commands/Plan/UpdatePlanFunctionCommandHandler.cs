using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Plan;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.Plan;

public sealed class UpdatePlanFunctionCommandHandler : ICommandHandler<Command.UpdatePlanFunctionCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Plan, int> _planRepository;
    private readonly IRepositoryBase<Domain.Entities.PlanFunction, int> _planFunctionRepository;

    public UpdatePlanFunctionCommandHandler(
        IRepositoryBase<Domain.Entities.Plan, int> planRepository,
        IRepositoryBase<Domain.Entities.PlanFunction, int> planFunctionRepository)
    {
        _planRepository = planRepository;
        _planFunctionRepository = planFunctionRepository;
    }

    public async Task<Result> Handle(Command.UpdatePlanFunctionCommand request, CancellationToken cancellationToken)
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
                "Function này chưa được thêm vào gói. Sử dụng API AddPlanFunction để thêm mới."));
        }

        // 3. Validate ActionValue
        if (request.ActionValue <= 0)
        {
            return Result.Failure(new Error(
                "INVALID_ACTION_VALUE",
                "ActionValue phải lớn hơn 0."));
        }

        // 4. Update ActionValue
        planFunction.Update(request.ActionValue);

        return Result.Success();
    }
}
