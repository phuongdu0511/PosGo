using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Role;

public static class Command
{
    public record CreateUserRole() : ICommand;
}
