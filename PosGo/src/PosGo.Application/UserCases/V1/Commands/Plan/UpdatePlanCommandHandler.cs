using System;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Plan;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Commands.Plan;

public sealed class UpdatePlanCommandHandler : ICommandHandler<Command.UpdatePlanCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Plan, int> _planRepository;

    public UpdatePlanCommandHandler(IRepositoryBase<Domain.Entities.Plan, int> planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<Result> Handle(Command.UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra Plan tồn tại
        var plan = await _planRepository.FindByIdAsync(request.Id);
        if (plan is null)
        {
            return Result.Failure(new Error("NOT_FOUND", $"Plan với Id {request.Id} không tồn tại."));
        }

        // 2. Kiểm tra Code trùng (nếu thay đổi Code)
        if (!plan.Code.Equals(request.Code, StringComparison.OrdinalIgnoreCase))
        {
            var normalizedCode = request.Code.Trim().ToLowerInvariant();
            var exists = await _planRepository.FindSingleAsync(x => x.Code == normalizedCode && x.Id != request.Id);
            if (exists is not null)
            {
                return Result.Failure(new Error("CODE_EXISTS", "Code gói đã tồn tại."));
            }
        }

        // 3. Update Plan
        plan.Update(request.Code, request.Description, request.IsActive);

        return Result.Success();
    }
}
