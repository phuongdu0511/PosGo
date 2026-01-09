using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Table;

public static class Command
{
    // TableArea Commands
    public record CreateTableAreaCommand(string Name, int SortOrder, bool IsActive = true) : ICommand;
    
    public record UpdateTableAreaCommand(Guid Id, string Name, int SortOrder, bool IsActive) : ICommand;
    
    public record DeleteTableAreaCommand(Guid Id) : ICommand;
    
    // Table Commands
    public record CreateTableCommand(Guid? AreaId, string Name, string QrCodeToken, int? Seats, bool DoNotAllowOrder = false, decimal? MinOrderAmount = null) : ICommand;
    
    public record UpdateTableCommand(Guid Id, Guid? AreaId, string Name, int? Seats, bool DoNotAllowOrder, decimal? MinOrderAmount) : ICommand;
    
    public record DeleteTableCommand(Guid Id) : ICommand;
    
    public record UpdateTableStatusCommand(Guid Id, bool isActive) : ICommand;
}