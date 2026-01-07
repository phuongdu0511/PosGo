using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Plan;

public static class Command
{
    public record PlanFunctionItem(int FunctionId, int ActionValue);
    
    public record CreatePlanCommand(string Code, string? Description, bool isActive, List<PlanFunctionItem> Functions) : ICommand;
    
    public record UpdatePlanCommand(Guid Id, string Code, string? Description, bool IsActive) : ICommand;
    
    public record DeletePlanCommand(Guid Id) : ICommand;
    
    public record AddPlanFunctionCommand(Guid PlanId, int FunctionId, int ActionValue) : ICommand;
    
    public record UpdatePlanFunctionCommand(Guid PlanId, int FunctionId, int ActionValue) : ICommand;
    
    public record RemovePlanFunctionCommand(Guid PlanId, int FunctionId) : ICommand;
}
