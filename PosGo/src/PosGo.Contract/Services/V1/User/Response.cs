using PosGo.Contract.Enumerations;

namespace PosGo.Contract.Services.V1.User;

public static class Response
{
    public record UserResponse(Guid Id, string UserName, string FullName, string PhoneNumber, EUserStatus Status);
    public record UpdateUserResponse(Guid Id, string UserName, string FullName, string PhoneNumber);
}
