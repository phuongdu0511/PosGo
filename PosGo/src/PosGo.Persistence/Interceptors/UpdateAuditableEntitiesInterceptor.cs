using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PosGo.Domain.Abstractions.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Persistence.Interceptors;

public sealed class UpdateAuditableEntitiesInterceptor
    : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UpdateAuditableEntitiesInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context;

        if (dbContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        IEnumerable<EntityEntry<IAuditableEntity>> entries =
            dbContext
                .ChangeTracker
                .Entries<IAuditableEntity>();
        foreach (EntityEntry<IAuditableEntity> entityEntry in entries)
        {
            var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();

            // Added
            if (entityEntry is { State: EntityState.Added, Entity: IAuditableEntity auditableAdd })
            {
                entityEntry.Property(nameof(auditableAdd.CreatedAt)).CurrentValue = DateTimeOffset.UtcNow;
                entityEntry.Property(nameof(auditableAdd.CreatedByUserId)).CurrentValue = userId;
            }

            // Sort Delete Pattern
            if (entityEntry is { State: EntityState.Deleted, Entity: ISoftDeletableEntity auditableDelete })
            {
                // Change State from Deleted to Modified =>> Will run into status = updated
                entityEntry.State = EntityState.Modified;
                entityEntry.Property(nameof(auditableDelete.IsDeleted)).CurrentValue = true;
                entityEntry.Property(nameof(auditableDelete.DeletedAt)).CurrentValue = DateTimeOffset.UtcNow;
                entityEntry.Property(nameof(auditableDelete.DeletedByUserId)).CurrentValue = userId;
            }

            // updated
            if (entityEntry is { State: EntityState.Modified, Entity: IAuditableEntity auditableModify })
            {
                entityEntry.Property(nameof(auditableModify.UpdatedAt)).CurrentValue = DateTimeOffset.UtcNow;
                entityEntry.Property(nameof(auditableModify.UpdatedByUserId)).CurrentValue = userId;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
