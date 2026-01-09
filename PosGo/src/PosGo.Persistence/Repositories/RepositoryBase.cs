using PosGo.Domain.Abstractions.Entities;
using PosGo.Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.AspNetCore.Http;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Contract.Extensions;

namespace PosGo.Persistence.Repositories;

public class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>, IDisposable
        where TEntity : Entity<TKey>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public RepositoryBase(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Dispose()
        => _context?.Dispose();
    public async Task<List<TEntity>> FromSqlRawAsync(string sql, object[] parameters, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        // Tự động thêm tenant filter vào SQL
        if (typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity)))
        {
            var restaurantId = _httpContextAccessor.HttpContext?.GetRestaurantId();

            if (restaurantId.HasValue)
            {
                var parametersList = parameters.ToList();
                sql = sql.AddWhereCondition("t.RestaurantId", ref parametersList, restaurantId.Value);
                parameters = parametersList.ToArray();
            }
            // System scope - không filter
        }

        IQueryable<TEntity> items = _context.Set<TEntity>().FromSqlRaw(sql, parameters).AsNoTracking();

        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
                items = items.Include(includeProperty);

        return await items.ToListAsync();
    }

    public IQueryable<TEntity> FromSqlRaw(string sql, object[] parameters, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        // Tự động thêm tenant filter vào SQL
        if (typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity)))
        {
            var restaurantId = _httpContextAccessor.HttpContext?.GetRestaurantId();

            if (restaurantId.HasValue)
            {
                var parametersList = parameters.ToList();
                sql = sql.AddWhereCondition("t.RestaurantId", ref parametersList, restaurantId.Value);
                parameters = parametersList.ToArray();
            }
            // System scope - không filter
        }

        IQueryable<TEntity> items = _context.Set<TEntity>().FromSqlRaw(sql, parameters).AsNoTracking();

        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
                items = items.Include(includeProperty);

        return items;
    }
    public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> items = _context.Set<TEntity>().AsNoTracking(); // Importance Always include AsNoTracking for Query Side
        if (typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity)))
        {
            items = _context.ApplyTenantFilter(items);
        }
        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
                items = items.Include(includeProperty);

        if (predicate is not null)
            items = items.Where(predicate);

        return items;
    }

    public async Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(null, includeProperties).AsTracking().SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    public async Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(null, includeProperties).AsTracking().SingleOrDefaultAsync(predicate, cancellationToken);

    public void Add(TEntity entity)
        => _context.Add(entity);
    public void AddRange(List<TEntity> entities)
        => _context.AddRange(entities);

    public void Remove(TEntity entity)
        => _context.Set<TEntity>().Remove(entity);

    public void RemoveMultiple(List<TEntity> entities)
        => _context.Set<TEntity>().RemoveRange(entities);

    public void Update(TEntity entity)
        => _context.Set<TEntity>().Update(entity);
    public void UpdateRange(List<TEntity> entities)
        => _context.Set<TEntity>().UpdateRange(entities);
}

