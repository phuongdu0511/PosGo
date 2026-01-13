using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class PlanFunction : Entity<int>
{
    public int PlanId { get; private set; }
    public int FunctionId { get; private set; }
    public int ActionValue { get; private set; }
    public bool IsActive { get; private set; }
    public virtual Plan Plan { get; private set; }
    public virtual Function Function { get; private set; }
    // Private constructor
    private PlanFunction(int planId, int functionId, int actionValue, bool isActive = true)
    {
        PlanId = planId;
        FunctionId = functionId;
        ActionValue = actionValue;
        IsActive = isActive;
    }

    // Constructor với Navigation Property (cho EF)
    private PlanFunction(Plan plan, int functionId, int actionValue, bool isActive = true)
    {
        Plan = plan;
        FunctionId = functionId;
        ActionValue = actionValue;
        IsActive = isActive;
    }

    // Factory methods
    public static PlanFunction Create(int planId, int functionId, int actionValue, bool isActive = true)
        => new PlanFunction(planId, functionId, actionValue, isActive);

    public static PlanFunction Create(Plan plan, int functionId, int actionValue, bool isActive = true)
        => new PlanFunction(plan, functionId, actionValue, isActive);

    // Update method
    public void Update(int functionId, int actionValue, bool isActive)
    {
        FunctionId = functionId;
        ActionValue = actionValue;
        IsActive = isActive;
    }
    public void Update(int actionValue)
    {
        ActionValue = actionValue;
    }

    // Business methods
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void UpdateActionValue(int actionValue) => ActionValue = actionValue;
}
