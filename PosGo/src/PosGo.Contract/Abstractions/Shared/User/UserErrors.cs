namespace PosGo.Contract.Abstractions.Shared.User;

public static class UserErrors
{
    public static readonly Error NotFound = new(
        "User.NotFound",
        "User not found.");
}
