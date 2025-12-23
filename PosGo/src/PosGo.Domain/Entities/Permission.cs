using PosGo.Domain.Abstractions.Aggregates;

namespace PosGo.Domain.Entities;

// =====================================
//  PERMISSION (URL + METHOD)
// =====================================
public class Permission : AuditableAggregateRoot<Guid>
{
    public string Code { get; private set; } = null!;        // DASHBOARD_VIEW
    public string Name { get; private set; } = null!;        // View dashboard
    public string Url { get; private set; } = null!;         // /dashboard, /orders/*
    public string? HttpMethod { get; private set; }          // GET, POST, ...
    public bool IsActive { get; private set; } = true;

    public virtual User? CreatedByUser { get; private set; }

    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    public virtual ICollection<PermissionAssignment> PermissionAssignments { get; private set; } = new List<PermissionAssignment>();

    private Permission() { }

    public Permission(Guid id, string code, string name, string url, string? httpMethod)
    {
        Id = id;
        Code = code;
        Name = name;
        Url = url;
        HttpMethod = httpMethod;
        IsActive = true;
    }

    public void Rename(string name) => Name = name;

    public void ChangeUrl(string url, string? httpMethod)
    {
        Url = url;
        HttpMethod = httpMethod;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
