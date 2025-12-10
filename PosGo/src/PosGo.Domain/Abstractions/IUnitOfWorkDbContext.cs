namespace PosGo.Domain.Abstractions;

public interface IUnitOfWorkDbContext<TContext> : IAsyncDisposable
{
    /// <summary>
    /// Call save change from db context
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
