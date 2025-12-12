using PosGo.Domain.Abstractions.Aggregates;

namespace PosGo.Domain.Entities;

// =====================================
//  ORDER / REPORT
// =====================================
public class Order : AuditableAggregateRoot<Guid>
{
    public Guid RestaurantId { get; private set; }
    public Guid TableId { get; private set; }
    public string OrderCode { get; private set; } = null!;
    public Guid? StatusId { get; private set; }   // CodeSet=OrderStatus
    public int? NumberOfGuests { get; private set; }
    public string? Note { get; private set; }
    public decimal SubTotalAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal FinalAmount { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? PaymentRequestedAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual Table Table { get; private set; } = null!;
    public virtual CodeItem? Status { get; private set; }
    public Guid? ClosedByUserId { get; set; }
    public virtual User? CreatedByUser { get; private set; }
    public virtual User? ClosedByUser { get; private set; }
    public virtual ICollection<OrderItem> Items { get; private set; }
}
