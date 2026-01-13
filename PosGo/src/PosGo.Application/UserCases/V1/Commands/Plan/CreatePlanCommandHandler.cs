using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Plan;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;

namespace PosGo.Application.UserCases.V1.Commands.Plan;

public sealed class CreatePlanCommandHandler : ICommandHandler<Command.CreatePlanCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Plan, int> _planRepository;
    private readonly IRepositoryBase<Domain.Entities.Function, int> _functionRepository;
    private readonly IRepositoryBase<Domain.Entities.PlanFunction, int> _planFunctionRepository;
    public CreatePlanCommandHandler(
        IRepositoryBase<Domain.Entities.Plan, int> planRepository,
        IRepositoryBase<Domain.Entities.Function, int> functionRepository,
        IRepositoryBase<Domain.Entities.PlanFunction, int> planFunctionRepository)
    {
        _planRepository = planRepository;
        _functionRepository = functionRepository;
        _planFunctionRepository = planFunctionRepository;
    }

    public async Task<Result> Handle(Command.CreatePlanCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra trùng Code
        var normalizedCode = request.Code.Trim().ToLowerInvariant();
        var exists = await _planRepository.FindSingleAsync(x => x.Code == normalizedCode);
        if (exists is not null)
        {
            return Result.Failure<Response.PlanResponse>(
                new Error("CODE_EXISTS", "Code gói đã tồn tại."));
        }

        // 2. Validate danh sách FunctionId
        var functionIds = request.Functions
            .Select(f => f.FunctionId)
            .Distinct()
            .ToList();

        if (!functionIds.Any())
        {
            return Result.Failure(new Error(
                "PLAN_FUNCTION_EMPTY",
                "Gói phải có ít nhất một chức năng."));
        }

        var existingFunctionIds = await _functionRepository.FindAll(x => functionIds.Contains(x.Id)).Select(x => x.Id).ToListAsync(cancellationToken);

        var missingIds = functionIds
            .Except(existingFunctionIds)
            .ToList();

        if (missingIds.Any())
        {
            return Result.Failure(new Error(
                "FUNCTION_NOT_FOUND",
                $"Các FunctionId không tồn tại: {string.Join(", ", missingIds)}"));
        }

        // 3. Tạo Plan
        var plan = Domain.Entities.Plan.Create(normalizedCode, request.Description);
        _planRepository.Add(plan);

        // 4. Tạo PlanFunction cho từng function trong request
        var planFunctions = new List<PlanFunction>();
        foreach (var item in request.Functions)
        {
            if (item.ActionValue <= 0)
            {
                // Nếu cần: bỏ qua những cái không có quyền gì
                continue;
            }

            var pf = PlanFunction.Create(plan, item.FunctionId, item.ActionValue);
            planFunctions.Add(pf);
        }

        if (planFunctions.Count == 0)
        {
            return Result.Failure(new Error(
                "PLAN_FUNCTION_INVALID",
                "Danh sách chức năng của gói không hợp lệ."));
        }

        _planFunctionRepository.AddRange(planFunctions);

        return Result.Success();
    }
}
