using System.Linq.Expressions;
using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cuzdan.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly CuzdanContext _context;
    private readonly DbSet<T> _dbSet;

    protected CuzdanContext Context => _context;

    public Repository(CuzdanContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }
    public async Task<PagedResult<T>> FindAsync(
    Expression<Func<T, bool>>? predicate, 
    int pageNumber = 1, 
    int pageSize = 10,
    Expression<Func<T, object>>? orderBy = null,
    bool isDescending = true,
    CancellationToken cancellationToken = default)
        {
        IQueryable<T> query = _dbSet;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
        {
            query = isDescending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        var pagedData = await query.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync(cancellationToken);

        return new PagedResult<T>(pageNumber, pageSize , totalCount, pagedData);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}