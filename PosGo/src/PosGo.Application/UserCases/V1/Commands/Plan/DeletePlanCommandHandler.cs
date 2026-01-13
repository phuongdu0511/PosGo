using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Plan;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Commands.Plan;

public sealed class DeletePlanCommandHandler : ICommandHandler<Command.DeletePlanCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Plan, int> _planRepository;
    private readonly ApplicationDbContext _context;

    public DeletePlanCommandHandler(
        IRepositoryBase<Domain.Entities.Plan, int> planRepository,
        ApplicationDbContext context)
    {
        _planRepository = planRepository;
        _context = context;
    }

    public async Task<Result> Handle(Command.DeletePlanCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra Plan tồn tại
        var plan = await _planRepository.FindByIdAsync(request.Id);
        if (plan is null)
        {
            return Result.Failure(new Error("NOT_FOUND", $"Plan với Id {request.Id} không tồn tại."));
        }

        // 2. Kiểm tra Plan có đang được sử dụng bởi Restaurant không
        var isInUse = await _context.RestaurantPlans
            .AnyAsync(rp => rp.PlanId == request.Id && rp.IsActive, cancellationToken);

        if (isInUse)
        {
            return Result.Failure(new Error(
                "PLAN_IN_USE",
                "Không thể xóa gói đang được sử dụng bởi các nhà hàng. Vui lòng vô hiệu hóa gói trước."));
        }

        // 3. Xóa PlanFunctions trước
        var planFunctions = await _context.PlanFunctions
            .Where(pf => pf.PlanId == request.Id)
            .ToListAsync(cancellationToken);

        if (planFunctions.Any())
        {
            _context.PlanFunctions.RemoveRange(planFunctions);
        }

        // 4. Xóa Plan
        _planRepository.Remove(plan);

        return Result.Success();
    }
}
