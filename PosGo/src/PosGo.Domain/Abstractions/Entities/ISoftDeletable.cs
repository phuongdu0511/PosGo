namespace PosGo.Domain.Abstractions.Entities;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; set; }
    DateTimeOffset? DeletedAt { get; set; }
    Guid? DeletedByUserId { get; set; }
}
