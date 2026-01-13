using PosGo.Domain.Abstractions.Aggregates;

namespace PosGo.Domain.Entities;

// =====================================
//  CODE SERVICE
// =====================================
public class CodeSet : AuditableAggregateRoot<int>
{
    public string Code { get; private set; } = null!;  // OrderStatus, TableStatus...
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public virtual ICollection<CodeItem> Items { get; private set; }
}
