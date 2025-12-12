using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Abstractions.Aggregates;

public abstract class AuditableAggregateRoot<T> : AggregateRoot<T>, IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}
