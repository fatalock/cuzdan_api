using System.Linq.Expressions;
using Cuzdan.Application.DTOs;
using Cuzdan.Application.Interfaces;
using Cuzdan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

    /// <summary>
    /// Bir kaydı, üzerinde güncelleme yapmak amacıyla veritabanında kilitler (exclusive lock).
    /// Bu işlem bitene kadar başka hiçbir transaction bu kaydı okuyamaz veya yazamaz.
    /// Kötümser Eşzamanlılık (Pessimistic Concurrency) için kullanılır.
    /// ÖNEMLİ: Bu metot mutlaka bir 'BeginTransactionAsync' bloğu içinde çağrılmalıdır.
    /// </summary>
    public async Task<T?> GetByIdForWriteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // EF Core metadata'sından tablo ve primary key bilgilerini dinamik olarak alıyoruz.
        var entityType = _context.Model.FindEntityType(typeof(T))!;
        var tableName = entityType.GetTableName()!;
        var pkPropertyName = entityType.FindPrimaryKey()!.Properties.Single().Name;
        var pkColumnName = entityType.FindProperty(pkPropertyName)!.GetColumnName(StoreObjectIdentifier.Table(tableName, null))!;

        // Veritabanına özel kilitleme sorgusu gönderiyoruz.
        // Bu sorgu, parametre kullanıldığı için SQL Injection'a karşı güvenlidir.
        // NOT: PostgreSQL için "FOR UPDATE", SQL Server için "WITH (UPDLOCK, ROWLOCK)".
        var sql = $"SELECT * FROM \"{tableName}\" WHERE \"{pkColumnName}\" = {{0}} FOR UPDATE";

        return await _dbSet.FromSqlRaw(sql, id).FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Bir kaydı, başka bir transaction'ın onu değiştirmesini engelleyecek şekilde kilitler (shared lock).
    /// Bu işlem bitene kadar başka transaction'lar bu kaydı okuyabilir ama değiştiremez.
    /// Genellikle tutarlı okumalar yapmak için kullanılır.
    /// ÖNEMLİ: Bu metot mutlaka bir 'BeginTransactionAsync' bloğu içinde çağrılmalıdır.
    /// </summary>
    public async Task<T?> GetByIdForReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entityType = _context.Model.FindEntityType(typeof(T))!;
        var tableName = entityType.GetTableName()!;
        var pkPropertyName = entityType.FindPrimaryKey()!.Properties.Single().Name;
        var pkColumnName = entityType.FindProperty(pkPropertyName)!.GetColumnName(StoreObjectIdentifier.Table(tableName, null))!;

        // NOT: PostgreSQL için "FOR SHARE", SQL Server için "WITH (HOLDLOCK, ROWLOCK)".
        var sql = $"SELECT * FROM \"{tableName}\" WHERE \"{pkColumnName}\" = {{0}} FOR SHARE";

        return await _dbSet.FromSqlRaw(sql, id).FirstOrDefaultAsync(cancellationToken);
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

        if (orderBy is not null)
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