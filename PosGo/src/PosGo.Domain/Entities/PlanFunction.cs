using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class PlanFunction : Entity<int>
{
    public Guid PlanId { get; set; }
    public int FunctionId { get; set; }
    public int ActionValue { get; set; }
    public bool IsActive { get; set; }
    public virtual Plan Plan { get; set; }
    public virtual Function Function { get; set; }
}
