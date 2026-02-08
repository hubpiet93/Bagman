namespace Bagman.Domain.Services;

public interface IPasswordHasher
{
    /// <summary>
    ///     Hashes a password using PBKDF2 algorithm
    /// </summary>
    /// <param name="password">Plain text password to hash</param>
    /// <returns>Hashed password in format: iterations.salt.key (all base64)</returns>
    string HashPassword(string password);

    /// <summary>
    ///     Verifies a password against a stored hash
    /// </summary>
    /// <param name="storedHash">The stored password hash</param>
    /// <param name="password">The plain text password to verify</param>
    /// <returns>True if password matches the hash, false otherwise</returns>
    bool VerifyPassword(string storedHash, string password);
}
