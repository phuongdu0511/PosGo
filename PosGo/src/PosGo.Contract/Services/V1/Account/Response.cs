namespace PosGo.Contract.Services.V1.Account;

public static class Response
{
    public record AccountResponse(Guid Id, string UserName, string FullName, string Phone);
    public record UpdateProfileResponse(Guid Id, string FullName, string Phone);
}
