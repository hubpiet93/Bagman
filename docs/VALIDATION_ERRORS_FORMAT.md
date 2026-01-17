# Nowy Format Błędów Walidacji

## Zmiana
Zmieniamy format odpowiedzi API dla błędów walidacji z domyślnego formatu ProblemDetails na niestandardowy format bardziej przyjazny dla frontendów.

## Stary Format (Deprecated)
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "RefreshToken": [
      "Refresh token jest wymagany"
    ]
  },
  "traceId": "00-0505e41214f80d6276bddc92f0f7c345-ee2c6f5b71d1cfc9-00"
}
```

## Nowy Format (Aktualny)
```json
{
  "errors": [
    {
      "errorCode": "Required",
      "errorMessage": "Refresh token jest wymagany",
      "errorSource": "RefreshToken"
    }
  ]
}
```

## Dostępne Kody Błędów (ValidationErrorCode)

| Kod | Opis |
|-----|------|
| `Required` | Wymagane pole |
| `InvalidFormat` | Nieprawidłowy format |
| `MinLength` | Minimum długości nie spełnione |
| `MaxLength` | Maximum długości przekroczone |
| `InvalidEmail` | Nieprawidłowy email |
| `InvalidUrl` | Nieprawidłowy URL |
| `RangeError` | Wartość poza dozwolonym zakresem |
| `PatternMismatch` | Nie pasuje do wzoru |
| `DuplicateValue` | Zduplikowana wartość |
| `InvalidCredentials` | Nieprawidłowe poświadczenia |
| `UserNotFound` | Użytkownik nie znaleziony |
| `TokenExpired` | Token wygasł |
| `TokenInvalid` | Token nieprawidłowy |
| `UnauthorizedAccess` | Brak dostępu |
| `ConflictError` | Konflikt (np. email już istnieje) |
| `ServerError` | Błąd serwera |
| `Other` | Inny błąd |

## Implementacja

### Modele (/src/Bagman.Contracts/Models/)

#### ValidationErrorCode.cs
Enum zawierający wszystkie możliwe kody błędów walidacji.

#### ValidationErrorResponse.cs
Model odpowiedzi API zawierający listę błędów.

### Middleware (/src/Bagman.Api/Middleware/)

#### ValidationExceptionMiddleware.cs
Middleware przechwytuje `ValidationException` z FluentValidation i mapuje błędy na nowy format. Automatycznie mapuje kody walidatorów FluentValidation na odpowiadające im `ValidationErrorCode`.

## Przykład Użycia

Kiedy użytkownik wysyła nieprawidłowe dane:

```csharp
POST /api/auth/register
{
  "email": "invalid-email",
  "password": "123"
}
```

Odpowiedź:
```json
HTTP/1.1 400 Bad Request

{
  "errors": [
    {
      "errorCode": "InvalidEmail",
      "errorMessage": "'Email' nie jest prawidłowym adresem email.",
      "errorSource": "Email"
    },
    {
      "errorCode": "MinLength",
      "errorMessage": "'Password' musi być dłuższe lub równe '8' znaków.",
      "errorSource": "Password"
    }
  ]
}
```

## Dla Frontend Developerów

Teraz możesz łatwo:
1. Iterować po tablicy `errors`
2. Wyświetlić błędy obok konkretnych pól za pomocą `errorSource`
3. Grupować i stylizować błędy na podstawie `errorCode`

```javascript
response.errors.forEach(error => {
  const fieldElement = document.querySelector(`[name="${error.errorSource}"]`);
  showErrorMessage(fieldElement, error.errorMessage);
});
```
