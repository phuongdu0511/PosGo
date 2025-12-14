using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PosGo.Contract.Abstractions.Shared.CommonServices;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Persistence.Interceptors;

public sealed class UpdateAuditableEntitiesInterceptor
    : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public UpdateAuditableEntitiesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
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
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.CreatedAt).CurrentValue = DateTimeOffset.UtcNow;
                entityEntry.Property(a => a.CreatedByUserId).CurrentValue = _currentUserService?.UserId;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
                entityEntry.Property(a => a.UpdatedByUserId).CurrentValue = _currentUserService?.UserId;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
