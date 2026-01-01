using PosGo.Contract.Enumerations;

namespace PosGo.Contract.Services.V1.Role;

public static class Response
{
    public record RoleResponse(string Scope, string RoleCode, string Name, string Description);
}
