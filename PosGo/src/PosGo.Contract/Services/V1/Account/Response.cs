namespace PosGo.Contract.Services.V1.Account;

public static class Response
{
    public record AccountResponse(Guid Id, string UserName, string FullName, string PhoneNumber);
    public record UpdateProfileResponse(Guid Id, string FullName, string PhoneNumber);
}
