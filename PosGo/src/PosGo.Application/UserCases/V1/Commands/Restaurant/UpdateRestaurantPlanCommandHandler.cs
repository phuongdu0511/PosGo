using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Restaurant;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;

namespace PosGo.Application.UserCases.V1.Commands.Restaurant;

public sealed class UpdateRestaurantPlanCommandHandler : ICommandHandler<Command.UpdateRestaurantPlanCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Restaurant, Guid> _restaurantRepository;
    private readonly IRepositoryBase<Domain.Entities.Plan, Guid> _planRepository;
    private readonly IRepositoryBase<Domain.Entities.RestaurantPlan, int> _restaurantPlanRepository;
    public UpdateRestaurantPlanCommandHandler(
        IRepositoryBase<Domain.Entities.Restaurant, Guid> restaurantRepository,
        IRepositoryBase<Domain.Entities.Plan, Guid> planRepository,
        IRepositoryBase<Domain.Entities.RestaurantPlan, int> restaurantPlanRepository)
    {
        _restaurantRepository = restaurantRepository;
        _planRepository = planRepository;
        _restaurantPlanRepository = restaurantPlanRepository;
    }

    public async Task<Result> Handle(Command.UpdateRestaurantPlanCommand request, CancellationToken cancellationToken)
    {
        // 1. Check restaurant
        var restaurant = await _restaurantRepository.FindSingleAsync(
            x => x.Id == request.RestaurantId,
            cancellationToken);

        if (restaurant is null)
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy nhà hàng."));

        // 2. Check plan
        var plan = await _planRepository.FindSingleAsync(
            x => x.Id == request.PlanId && x.IsActive,
            cancellationToken);

        if (plan is null)
            return Result.Failure(new Error("PLAN_NOT_FOUND", "Không tìm thấy gói hoặc gói không còn active."));

        // 3. Lấy gói hiện tại đang active (nếu có)
        var oldRps = await _restaurantPlanRepository
            .FindAll(x => x.RestaurantId == request.RestaurantId && x.IsActive)
            .ToListAsync(cancellationToken);

        var currentActive = oldRps.FirstOrDefault();
        if (currentActive is not null && currentActive.PlanId == request.PlanId)
        {
            return Result.Success();
        }

        foreach (var rp in oldRps)
        {
            rp.Deactivate();
        }

        _restaurantPlanRepository.UpdateRange(oldRps);

        // 4. Tạo dòng RestaurantPlan mới & active
        var newRp = RestaurantPlan.Create(request.RestaurantId, plan.Id);

        _restaurantPlanRepository.Add(newRp);

        return Result.Success();
    }
}
