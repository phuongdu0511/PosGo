using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Table;

public static class Command
{
    // TableArea Commands
    public record CreateTableAreaCommand(string Name, int SortOrder, bool IsActive = true) : ICommand;
    
    public record UpdateTableAreaCommand(int Id, string Name, int SortOrder, bool IsActive) : ICommand;
    
    public record DeleteTableAreaCommand(int Id) : ICommand;
    
    // Table Commands
    public record CreateTableCommand(int? AreaId, string Name, string QrCodeToken, int? Seats, bool DoNotAllowOrder = false, decimal? MinOrderAmount = null) : ICommand;
    
    public record UpdateTableCommand(int Id, int? AreaId, string Name, int? Seats, bool DoNotAllowOrder, decimal? MinOrderAmount) : ICommand;
    
    public record DeleteTableCommand(int Id) : ICommand;
    
    public record UpdateTableStatusCommand(int Id, bool isActive) : ICommand;
}