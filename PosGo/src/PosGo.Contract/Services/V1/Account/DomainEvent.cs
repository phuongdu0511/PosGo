using PosGo.Contract.Abstractions.Message;

namespace PosGo.Contract.Services.V1.Account;

public static class DomainEvent
{
    public record AccountCreated(Guid IdEvent, Guid Id, string Name, decimal Price, string Description, string sku) : IDomainEvent;
    public record AccountDeleted(Guid IdEvent, Guid Id) : IDomainEvent;
    public record AccountUpdated(Guid IdEvent, Guid Id, string Name, decimal Price, string Description) : IDomainEvent;
}
