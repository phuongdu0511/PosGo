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
        var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
        var now = DateTimeOffset.UtcNow;

        IEnumerable<EntityEntry> entries = dbContext.ChangeTracker.Entries();
        foreach (EntityEntry entityEntry in entries)
        {
            // Added
            if (entityEntry is { State: EntityState.Added, Entity: IAuditableEntity auditableAdd })
            {
                entityEntry.Property(nameof(auditableAdd.CreatedAt)).CurrentValue = now;
                entityEntry.Property(nameof(auditableAdd.CreatedByUserId)).CurrentValue = userId;
            }

            // Sort deleted
            var isSoftDeleting = false;
            if (entityEntry is { State: EntityState.Deleted, Entity: ISoftDeletableEntity auditableDelete })
            {
                // Change State from Deleted to Modified =>> Will run into status = updated
                isSoftDeleting = true;
                entityEntry.State = EntityState.Modified;
                entityEntry.Property(nameof(auditableDelete.IsDeleted)).CurrentValue = true;
                entityEntry.Property(nameof(auditableDelete.DeletedAt)).CurrentValue = now;
                entityEntry.Property(nameof(auditableDelete.DeletedByUserId)).CurrentValue = userId;
            }

            // Updated
            if (entityEntry is { State: EntityState.Modified, Entity: IAuditableEntity auditableModify } && !isSoftDeleting)
            {
                entityEntry.Property(nameof(auditableModify.UpdatedAt)).CurrentValue = now;
                entityEntry.Property(nameof(auditableModify.UpdatedByUserId)).CurrentValue = userId;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
