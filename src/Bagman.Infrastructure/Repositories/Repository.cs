using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<ErrorOr<TEntity?>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await DbSet.FindAsync(id);
            return entity;
        }
        catch (Exception ex)
        {
            return Error.Failure("Database.Error", ex.Message);
        }
    }

    public virtual void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public virtual async Task<ErrorOr<Success>> SaveChangesAsync()
    {
        try
        {
            await Context.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Database.SaveError", ex.Message);
        }
    }
}
