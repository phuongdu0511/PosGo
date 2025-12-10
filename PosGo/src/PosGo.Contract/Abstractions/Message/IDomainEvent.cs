using MediatR;

namespace PosGo.Contract.Abstractions.Message;

public interface IDomainEvent : INotification
{
    public Guid IdEvent { get; init; }
}
