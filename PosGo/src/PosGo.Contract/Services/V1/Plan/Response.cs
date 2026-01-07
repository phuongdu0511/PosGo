namespace PosGo.Contract.Services.V1.Plan;

public static class Response
{
    public record PlanResponse(
        Guid Id,
        string Code,
        string? Description,
        bool IsActive
    );

    public record PlanFunctionResponse(
        int Id,
        int FunctionId,
        string FunctionName,
        string FunctionKey,
        int ActionValue,
        bool IsActive
    );
}
