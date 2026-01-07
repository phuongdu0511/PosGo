using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Plan;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Commands.Plan;

public sealed class AddPlanFunctionCommandHandler : ICommandHandler<Command.AddPlanFunctionCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Plan, Guid> _planRepository;
    private readonly IRepositoryBase<Domain.Entities.Function, int> _functionRepository;
    private readonly IRepositoryBase<Domain.Entities.PlanFunction, int> _planFunctionRepository;
    private readonly ApplicationDbContext _context;

    public AddPlanFunctionCommandHandler(
        IRepositoryBase<Domain.Entities.Plan, Guid> planRepository,
        IRepositoryBase<Domain.Entities.Function, int> functionRepository,
        IRepositoryBase<Domain.Entities.PlanFunction, int> planFunctionRepository,
        ApplicationDbContext context)
    {
        _planRepository = planRepository;
        _functionRepository = functionRepository;
        _planFunctionRepository = planFunctionRepository;
        _context = context;
    }

    public async Task<Result> Handle(Command.AddPlanFunctionCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra Plan tồn tại
        var plan = await _planRepository.FindByIdAsync(request.PlanId);
        if (plan is null)
        {
            return Result.Failure(new Error("NOT_FOUND", $"Plan với Id {request.PlanId} không tồn tại."));
        }

        // 2. Kiểm tra Function tồn tại
        var function = await _functionRepository.FindByIdAsync(request.FunctionId);
        if (function is null)
        {
            return Result.Failure(new Error("NOT_FOUND", $"Function với Id {request.FunctionId} không tồn tại."));
        }

        // 3. Kiểm tra PlanFunction đã tồn tại chưa
        var existing = await _planFunctionRepository.FindSingleAsync(
            x => x.PlanId == request.PlanId && x.FunctionId == request.FunctionId,
            cancellationToken);

        if (existing is not null)
        {
            return Result.Failure(new Error(
                "ALREADY_EXISTS",
                "Function này đã được thêm vào gói. Sử dụng API UpdatePlanFunction để cập nhật quyền."));
        }

        // 4. Validate ActionValue
        if (request.ActionValue <= 0)
        {
            return Result.Failure(new Error(
                "INVALID_ACTION_VALUE",
                "ActionValue phải lớn hơn 0."));
        }

        // 5. Tạo PlanFunction mới
        var planFunction = new Domain.Entities.PlanFunction
        {
            PlanId = request.PlanId,
            FunctionId = request.FunctionId,
            ActionValue = request.ActionValue,
            IsActive = true
        };

        _planFunctionRepository.Add(planFunction);

        return Result.Success();
    }
}
