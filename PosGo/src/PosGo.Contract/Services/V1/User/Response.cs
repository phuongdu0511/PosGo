using PosGo.Contract.Enumerations;

namespace PosGo.Contract.Services.V1.User;

public static class Response
{
    public record UserResponse(Guid Id, string UserName, string FullName, string Phone, EUserStatus Status, IReadOnlyList<string> Roles);
    public record UpdateUserResponse(Guid Id, string FullName, string Phone);
}
