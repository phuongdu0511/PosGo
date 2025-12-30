using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;

namespace PosGo.Contract.Services.V1.User;

public static class Command
{
    public record CreateUserCommand(string UserName, string Password, string ConfirmPassword, string FullName, string? PhoneNumber) : ICommand;

    public record UpdateUserCommand(Guid Id, string FullName, string? PhoneNumber) : ICommand;

    public record ChangePasswordUserCommand(Guid Id, string NewPassword, string ConfirmNewPassword) : ICommand;

    public record ChangeStatusUserCommand(Guid Id, EUserStatus status) : ICommand;
    public record UpdateUserRolesCommand(Guid Id, IReadOnlyList<string> RoleCodes) : ICommand;
}
