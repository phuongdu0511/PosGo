namespace PosGo.Contract.Abstractions.Shared.CommonServices;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    IReadOnlyList<string> Roles { get; }
}
