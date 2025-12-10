namespace PosGo.Domain.Abstractions.Entities;

public interface IAuditableEntity
{
    DateTimeOffset CreatedOnUtc { get; set; }
    DateTimeOffset? ModifedOnUtc { get; set; }
}
