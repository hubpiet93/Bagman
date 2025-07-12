using TypowanieMeczy.Domain.Entities;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id);
    Task<User?> GetByLoginAsync(Login login);
    Task<User?> GetByEmailAsync(Email email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> ExistsAsync(UserId id);
    Task<bool> ExistsByLoginAsync(Login login);
    Task<bool> ExistsByEmailAsync(Email email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(UserId id);
    Task<IEnumerable<User>> GetByTableIdAsync(TableId tableId);
    Task<IEnumerable<User>> GetAdminsByTableIdAsync(TableId tableId);
} 