using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;
using Bagman.Infrastructure.Models;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Repositories;

public class EfUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    public EfUserRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<User?>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            return entity;
        }
        catch (Exception ex)
        {
            return Error.Failure("User.GetByIdError", $"Błąd podczas pobierania użytkownika: {ex.Message}");
        }
    }

    public async Task<ErrorOr<User?>> GetByLoginAsync(string login)
    {
        try
        {
            var entity = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login == login);
            return entity;
        }
        catch (Exception ex)
        {
            return Error.Failure("User.GetByLoginError", $"Błąd podczas pobierania użytkownika: {ex.Message}");
        }
    }

    public async Task<ErrorOr<User?>> GetByEmailAsync(string email)
    {
        try
        {
            var entity = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            return entity;
        }
        catch (Exception ex)
        {
            return Error.Failure("User.GetByEmailError", $"Błąd podczas pobierania użytkownika: {ex.Message}");
        }
    }

    public async Task<ErrorOr<User>> CreateAsync(User user)
    {
        try
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            return Error.Failure("User.CreateError", $"Błąd podczas tworzenia użytkownika: {ex.Message}");
        }
    }

    public async Task<ErrorOr<User>> UpdateAsync(User user)
    {
        try
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            return Error.Failure("User.UpdateError", $"Błąd podczas aktualizacji użytkownika: {ex.Message}");
        }
    }

    public async Task<ErrorOr<string?>> GetPasswordHashAsync(Guid userId)
    {
        try
        {
            // Read explicit `PasswordHash` property from User
            var token = await _db.Users.AsNoTracking().Where(u => u.Id == userId).Select(u => u.PasswordHash).FirstOrDefaultAsync();
            return token;
        }
        catch (Exception ex)
        {
            return Error.Failure("User.GetPasswordHashError", $"Błąd podczas pobierania hasha hasła: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> SetPasswordHashAsync(Guid userId, string passwordHash)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return Error.NotFound("User.NotFound", "Użytkownik nie został znaleziony");

            // Store explicitly in User.PasswordHash
            user.PasswordHash = passwordHash;
            await _db.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("User.SetPasswordHashError", $"Błąd podczas zapisu hasha hasła: {ex.Message}");
        }
    }

    public async Task<ErrorOr<RefreshToken?>> GetRefreshTokenAsync(string token)
    {
        try
        {
            var entity = await _db.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.Token == token);
            if (entity is null)
                return (RefreshToken?)null;

            return new RefreshToken {Token = entity.Token, UserId = entity.UserId, ExpiresAt = entity.ExpiresAt};
        }
        catch (Exception ex)
        {
            return Error.Failure("RefreshToken.GetError", $"Błąd podczas pobierania refresh tokena: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            var entity = new RefreshTokenEntity
            {
                Token = refreshToken.Token,
                UserId = refreshToken.UserId,
                ExpiresAt = refreshToken.ExpiresAt
            };
            _db.RefreshTokens.Add(entity);
            await _db.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("RefreshToken.AddError", $"Błąd podczas zapisu refresh tokena: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> RemoveRefreshTokenAsync(string token)
    {
        try
        {
            var entity = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (entity is null)
                return Error.NotFound("RefreshToken.NotFound", "Refresh token nie został znaleziony");

            _db.RefreshTokens.Remove(entity);
            await _db.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("RefreshToken.RemoveError", $"Błąd podczas usuwania refresh tokena: {ex.Message}");
        }
    }
}
