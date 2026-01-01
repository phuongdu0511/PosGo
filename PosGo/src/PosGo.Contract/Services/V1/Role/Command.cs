using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Role;

public static class Command
{
    public record CreateRole(
        string Scope,
        string Description,
        string RoleCode,
        string Name
        ) : ICommand;
}
