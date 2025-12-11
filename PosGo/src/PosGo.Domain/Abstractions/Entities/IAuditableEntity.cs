namespace PosGo.Domain.Abstractions.Entities;

public interface IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}
