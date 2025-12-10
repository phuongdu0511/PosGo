using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PosGo.Domain.Abstractions;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Persistence;

public class EFUnitOfWorkDbContext<TContext> : IUnitOfWorkDbContext<TContext>
    where TContext : DbContext
{
    private readonly TContext _context;

    public EFUnitOfWorkDbContext(TContext context)
        => _context = context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //UpdateAuditableEntities();
        await _context.SaveChangesAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
        => await _context.DisposeAsync();

    private void UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<IAuditableEntity>> entries =
            _context
                .ChangeTracker
                .Entries<IAuditableEntity>();
        foreach (EntityEntry<IAuditableEntity> entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.CreatedOnUtc).CurrentValue = DateTime.UtcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.ModifedOnUtc).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}