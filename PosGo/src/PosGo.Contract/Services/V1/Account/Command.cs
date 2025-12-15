using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Account;

public static class Command
{
    public record UpdateProfileCommand(string FullName, string Phone) : ICommand;
    public record ChangePasswordCommand(
        string CurrentPassword,
        string NewPassword,
        string ConfirmNewPassword
    ) : ICommand;
}
