using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class Function : Entity<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string? CssClass { get; set; }
    public int SortOrder { get; set; }
    public int ParrentId { get; set; }
    public Status Status { get; set; }
    public string Key { get; set; }
    public int ActionValue { get; set; }
    public virtual ICollection<PlanFunction> PlanFunctions { get; set; }
}
