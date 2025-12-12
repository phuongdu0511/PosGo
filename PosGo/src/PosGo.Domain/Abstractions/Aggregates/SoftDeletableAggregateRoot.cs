using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Abstractions.Aggregates;

public abstract class SoftDeletableAggregateRoot<T> : AuditableAggregateRoot<T>, ISoftDeletableEntity
{
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }
}
