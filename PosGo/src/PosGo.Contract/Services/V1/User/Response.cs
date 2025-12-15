namespace PosGo.Contract.Services.V1.User;

public static class Response
{
    public record UserResponse(Guid Id, string UserName, string FullName, string Phone);
    public record UpdateUserResponse(Guid Id, string FullName, string Phone);
}
