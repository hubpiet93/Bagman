namespace Bagman.Contracts.Models;

public enum ValidationErrorCode
{
    Required,           // Wymagane pole
    InvalidFormat,      // Nieprawidłowy format
    MinLength,          // Minimum długości nie spełnione
    MaxLength,          // Maximum długości przekroczone
    InvalidEmail,       // Nieprawidłowy email
    InvalidUrl,         // Nieprawidłowy URL
    RangeError,         // Wartość poza dozwolonym zakresem
    PatternMismatch,    // Nie pasuje do wzoru
    DuplicateValue,     // Zduplikowana wartość
    InvalidCredentials, // Nieprawidłowe poświadczenia
    UserNotFound,       // Użytkownik nie znaleziony
    TokenExpired,       // Token wygasł
    TokenInvalid,       // Token nieprawidłowy
    UnauthorizedAccess, // Brak dostępu
    ConflictError,      // Konflikt (np. email już istnieje)
    ServerError,        // Błąd serwera
    Other               // Inny błąd
}
