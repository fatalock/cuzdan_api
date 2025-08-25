using System.Linq.Expressions;
using Cuzdan.Application.DTOs;

namespace Cuzdan.Application.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();
    IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<PagedResult<T>> FindAsync(
        Expression<Func<T, bool>>? predicate,
        int pageNumber,
        int pageSize,
        Expression<Func<T, object>>? orderBy,
        bool isDescending,
        CancellationToken cancellationToken = default
    );
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}