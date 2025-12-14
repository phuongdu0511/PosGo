using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Account;

public static class Command
{
    public record UpdateProfileCommand(string FullName, string Phone) : ICommand;
    public record CreateAccountCommand(string Name, decimal Price, string Description, string sku) : ICommand;

    public record UpdateAccountCommand(Guid Id, string Name, decimal Price, string Description) : ICommand;

    public record DeleteAccountCommand(Guid Id) : ICommand;
}
