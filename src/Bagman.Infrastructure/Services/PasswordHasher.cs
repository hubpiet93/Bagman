using System.Security.Cryptography;
using Bagman.Domain.Services;

namespace Bagman.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(KeySize);

        // store as: iterations.salt.key (all base64)
        return string.Join('.', Iterations.ToString(), Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }

    public bool VerifyPassword(string storedHash, string password)
    {
        try
        {
            var parts = storedHash.Split('.', 3);
            if (parts.Length != 3)
                return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var attempted = pbkdf2.GetBytes(key.Length);

            return CryptographicOperations.FixedTimeEquals(attempted, key);
        }
        catch
        {
            return false;
        }
    }
}
