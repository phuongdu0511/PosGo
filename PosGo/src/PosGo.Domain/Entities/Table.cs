using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  TABLE / QR
// =====================================
public class Table : AuditableEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public Guid? AreaId { get; private set; }
    public string Name { get; private set; } = null!;
    public string QrCodeToken { get; private set; } = null!;   // unique
    public int? Seats { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool DoNotAllowOrder { get; private set; }
    public decimal? MinOrderAmount { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual TableArea? Area { get; private set; }
    public virtual ICollection<Order> Orders { get; private set; }

    public Table(
        Guid id, 
        Guid restaurantId, 
        string name, 
        string qrCodeToken, 
        Guid? areaId = null, 
        int? seats = null, 
        bool isActive = true, 
        bool doNotAllowOrder = false, 
        decimal? minOrderAmount = null)
    {
        Id = id;
        RestaurantId = restaurantId;
        AreaId = areaId;
        Name = name;
        QrCodeToken = qrCodeToken;
        Seats = seats;
        IsActive = isActive;
        DoNotAllowOrder = doNotAllowOrder;
        MinOrderAmount = minOrderAmount;
    }

    public static Table Create(Guid id, Guid restaurantId, string code, string qrCodeToken, Guid? areaId = null, int? seats = null, bool doNotAllowOrder = false, bool isActive = true, decimal? minOrderAmount = null)
    {
        return new Table(id, restaurantId, code, qrCodeToken, areaId, seats, doNotAllowOrder, isActive, minOrderAmount);
    }

    public void Update(Guid? areaId, string name, int? seats, bool doNotAllowOrder, decimal? minOrderAmount)
    {
        AreaId = areaId;
        Name = name;
        Seats = seats;
        DoNotAllowOrder = doNotAllowOrder;
        MinOrderAmount = minOrderAmount;
    }

    public void UpdateStatus(bool isActive)
    {
        IsActive = isActive;
    }
}
