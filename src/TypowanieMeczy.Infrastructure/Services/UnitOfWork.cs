using TypowanieMeczy.Domain.Interfaces;

namespace TypowanieMeczy.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
    public Task BeginTransactionAsync()
    {
        // Begin transaction logic
        return Task.CompletedTask;
    }

    public Task CommitAsync()
    {
        // Commit logic
        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        // Rollback logic
        return Task.CompletedTask;
    }
} 