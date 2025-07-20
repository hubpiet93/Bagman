using Bagman.Domain.Models;
using ErrorOr;

namespace Bagman.Domain.Repositories;

public interface IUserRepository
{
    Task<ErrorOr<User?>> GetByIdAsync(Guid id);
    Task<ErrorOr<User?>> GetByLoginAsync(string login);
    Task<ErrorOr<User?>> GetByEmailAsync(string email);
    Task<ErrorOr<User>> CreateAsync(User user);
    Task<ErrorOr<User>> UpdateAsync(User user);
}
