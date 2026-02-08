using ErrorOr;

namespace Bagman.Domain.Repositories;

/// <summary>
///     Generic repository interface for aggregate roots
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    Task<ErrorOr<TEntity?>> GetByIdAsync(Guid id);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<ErrorOr<Success>> SaveChangesAsync();
}
