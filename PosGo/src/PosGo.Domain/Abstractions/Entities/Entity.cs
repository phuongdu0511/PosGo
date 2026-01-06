namespace PosGo.Domain.Abstractions.Entities;

public abstract class Entity<T> : IEntity<T>
{
    public T Id { get; protected set; }
}

public abstract class AuditableEntity<T> : Entity<T>, IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}

public abstract class SoftDeletableEntity<T> : AuditableEntity<T>, ISoftDeletableEntity
{
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }
}
