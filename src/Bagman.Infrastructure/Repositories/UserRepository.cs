using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Models;
using ErrorOr;
using Postgrest;
using Postgrest.Exceptions;
using Client = Supabase.Client;

namespace Bagman.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly Client _supabaseClient;

    public UserRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<ErrorOr<User?>> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _supabaseClient
                .From<UserEntity>()
                .Select("*")
                .Filter("id", Constants.Operator.Equals, id.ToString())
                .Single();

            return MapToUser(response);
        }
        catch (PostgrestException ex) when (ex.Message.Contains("No rows found"))
        {
            return (User?)null;
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
            var response = await _supabaseClient
                .From<UserEntity>()
                .Select("*")
                .Filter("login", Constants.Operator.Equals, login)
                .Single();

            return MapToUser(response);
        }
        catch (PostgrestException ex) when (ex.Message.Contains("No rows found"))
        {
            return (User?)null;
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
            var response = await _supabaseClient
                .From<UserEntity>()
                .Select("*")
                .Filter("email", Constants.Operator.Equals, email)
                .Single();

            return MapToUser(response);
        }
        catch (PostgrestException ex) when (ex.Message.Contains("No rows found"))
        {
            return (User?)null;
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
            var userEntity = MapToUserEntity(user);
            var response = await _supabaseClient
                .From<UserEntity>()
                .Insert(userEntity);

            return MapToUser(response.Models.First());
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
            var userEntity = MapToUserEntity(user);
            var response = await _supabaseClient
                .From<UserEntity>()
                .Where(x => x.Id == user.Id)
                .Set(x => x.Login, userEntity.Login)
                .Set(x => x.Email, userEntity.Email)
                .Set(x => x.CreatedAt, userEntity.CreatedAt)
                .Set(x => x.IsActive, userEntity.IsActive)
                .Update();

            return MapToUser(response.Model);
        }
        catch (Exception ex)
        {
            return Error.Failure("User.UpdateError", $"Błąd podczas aktualizacji użytkownika: {ex.Message}");
        }
    }

    private static User MapToUser(UserEntity entity)
    {
        if (entity == null)
            return null!;

        return new User
        {
            Id = entity.Id,
            Login = entity.Login,
            Email = entity.Email,
            CreatedAt = entity.CreatedAt,
            IsActive = entity.IsActive
        };
    }

    private static UserEntity MapToUserEntity(User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive
        };
    }
}
