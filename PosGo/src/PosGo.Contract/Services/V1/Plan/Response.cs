namespace PosGo.Contract.Services.V1.Plan;

public static class Response
{
    public record PlanResponse(string Code, string? Description);
}
