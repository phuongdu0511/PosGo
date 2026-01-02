using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Plan;

public static class Command
{
    public record PlanFunctionItem(int FunctionId, int ActionValue);
    public record CreatePlanCommand(string Code, string? Description, bool isActive, List<PlanFunctionItem> Functions) : ICommand;
}
