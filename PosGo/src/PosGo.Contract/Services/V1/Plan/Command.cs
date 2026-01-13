using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Plan;

public static class Command
{
    public record PlanFunctionItem(int FunctionId, int ActionValue);
    
    public record CreatePlanCommand(string Code, string? Description, bool isActive, List<PlanFunctionItem> Functions) : ICommand;
    
    public record UpdatePlanCommand(int Id, string Code, string? Description, bool IsActive) : ICommand;
    
    public record DeletePlanCommand(int Id) : ICommand;
    
    public record AddPlanFunctionCommand(int PlanId, int FunctionId, int ActionValue) : ICommand;
    
    public record UpdatePlanFunctionCommand(int PlanId, int FunctionId, int ActionValue) : ICommand;
    
    public record RemovePlanFunctionCommand(int PlanId, int FunctionId) : ICommand;
}
