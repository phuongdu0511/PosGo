using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Language;

public static class Command
{
    public record CreateLanguageCommand(string Code, string Name, bool IsActive = true) : ICommand;
    
    public record UpdateLanguageCommand(int Id, string Code, string Name, bool IsActive) : ICommand;
    
    public record DeleteLanguageCommand(int Id) : ICommand;
    
    public record UpdateLanguageStatusCommand(int Id, bool IsActive) : ICommand;
}