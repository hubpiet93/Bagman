# Integration Tests dla AuthController z TestContainers

## Przegląd

W projekcie zostały zaimplementowane kompleksowe testy integracyjne dla `AuthController` z wykorzystaniem:
- **TestContainers** - do zarządzania bazą danych PostgreSQL
- **Verify** - do snapshot testów weryfikujących strukturę odpowiedzi API

Testy weryfikują wszystkie akcje kontrolera:
- **Register** - rejestracja nowego użytkownika
- **Login** - logowanie użytkownika
- **Refresh** - odświeżanie tokenu dostępu
- **Logout** - wylogowanie użytkownika

## Struktura Testów

### 1. PostgresFixture (`TestFixtures/PostgresFixture.cs`)

Fixture zarządzający cyklem życia kontenera PostgreSQL:

```csharp
public class PostgresFixture : IAsyncLifetime
{
    // Tworzy i uruchamia PostgreSQL container
    // Usuwa container po zakończeniu testów
}
```

**Cechy:**
- Implementuje `IAsyncLifetime` do zarządzania inicjalizacją i czyszczeniem zasobów
- Tworzy nową instancję bazy danych dla każdego zestawu testów
- Zwraca connection string dla komponentów testowych

### 2. AuthTestWebApplicationFactory (`TestFixtures/AuthTestWebApplicationFactory.cs`)

Fabryka do tworzenia instancji aplikacji testowej:

```csharp
public class AuthTestWebApplicationFactory : WebApplicationFactory<Program>
{
    // Konfiguruje WebHost z testową bazą danych
    // Zastępuje DbContext na tę ze stringiem połączenia z TestContainersem
}
```

**Cechy:**
- Rozszerza `WebApplicationFactory<Program>`
- Konfiguruje `ApplicationDbContext` z testową bazą danych
- Zapewnia izolację testów poprzez dedykowane bazy danych

### 3. AuthControllerTests (`Controllers/AuthControllerTests.cs`)

Główna klasa testowa zawierająca wszystkie testy:

```csharp
public class AuthControllerTests : IAsyncLifetime
{
    // Implementuje 11 testów przypadków
    // Używa Verify.Xunit dla snapshot testów
}
```

## Snapshot Testing z Verify

Projekt wykorzystuje bibliotekę **Verify** do snapshot testów. Snapshot testing to podejście polegające na:

1. **Capturowaniu stanu** - Weryfikator przechwyca bieżący stan (np. struktura JSON)
2. **Porównywaniu ze snapshote** - Porównuje to ze wcześniej zarejestrowanym snapshote
3. **Zatwierdzeniu zmian** - Deweloper zatwierdzit/odrzuca zmiany

### Zalety Snapshot Testing

- **Automatyczne weryfikowanie** - Nie trzeba ręcznie pisać asercji dla każdego pola
- **Czytelne dyffu** - Widać dokładnie co się zmieniło
- **Mniej kodu** - Zamiast wielu `.Should().Be()` jeden `await Verify()`
- **Bezpieczeństwo refaktoryzacji** - Każda zmiana w API jest od razu widoczna

### Struktura Snapshot Testów

Każdy test weryfikuje kluczowe aspekty odpowiedzi:

```csharp
await Verify(new
{
    response.StatusCode,
    User = new
    {
        authResponse!.User.Login,
        authResponse.User.Email,
        authResponse.User.IsActive,
        HasAccessToken = !string.IsNullOrEmpty(authResponse.AccessToken),
        HasRefreshToken = !string.IsNullOrEmpty(authResponse.RefreshToken),
        ExpiresAtGreaterThanNow = authResponse.ExpiresAt > DateTime.UtcNow
    }
});
```

**Zaleta:** Weryfikuje strukturę bez twardych zaleceń do konkretnych wartości (np. token może być inny)

## Pokrycie Testami

### Register Action
1. **Register_WithValidRequest_ReturnsOkWithAuthResponse** - Rejestracja z prawidłowymi danymi
2. **Register_WithDuplicateLogin_ReturnsBadRequest** - Duplikat loginu
3. **Register_WithInvalidEmail_ReturnsBadRequest** - Nieprawidłowy email
4. **Register_WithWeakPassword_ReturnsBadRequest** - Słabe hasło

### Login Action
1. **Login_WithValidCredentials_ReturnsOkWithAuthResponse** - Logowanie z prawidłowymi poświadczeniami
2. **Login_WithInvalidPassword_ReturnsBadRequest** - Nieprawidłowe hasło
3. **Login_WithNonexistentUser_ReturnsBadRequest** - Użytkownik nie istnieje

### Refresh Action
1. **Refresh_WithValidRefreshToken_ReturnsOkWithNewTokens** - Odświeżenie z prawidłowym tokenem
2. **Refresh_WithInvalidRefreshToken_ReturnsBadRequest** - Nieprawidłowy token

### Logout Action
1. **Logout_WithValidRefreshToken_ReturnsOk** - Wylogowanie z prawidłowym tokenem
2. **Logout_WithInvalidRefreshToken_ReturnsBadRequest** - Nieprawidłowy token

### End-to-End
1. **FullAuthenticationFlow_RegisterLoginRefreshLogout_Succeeds** - Pełny przepływ autentykacji

## Uruchamianie Testów

### Wymagania
- Docker (do uruchamiania kontenera PostgreSQL)
- .NET 10.0 SDK

### Polecenia

```bash
# Uruchomienie wszystkich testów integracyjnych
dotnet test tests/Bagman.IntegrationTests/Bagman.IntegrationTests.csproj

# Uruchomienie testów AuthController
dotnet test tests/Bagman.IntegrationTests/Bagman.IntegrationTests.csproj --filter "NamespaceName=Bagman.IntegrationTests.Controllers"

# Uruchomienie konkretnego testu
dotnet test tests/Bagman.IntegrationTests/Bagman.IntegrationTests.csproj --filter "AuthControllerTests.Register_WithValidRequest_ReturnsOkWithAuthResponse"

# Uruchomienie z verbose output
dotnet test tests/Bagman.IntegrationTests/Bagman.IntegrationTests.csproj -v d

# Zatwierdzenie/aktualizacja snapshote
dotnet test tests/Bagman.IntegrationTests/Bagman.IntegrationTests.csproj -- --verify-mode AcceptTest
```

### Snapshots

Snapshot testy zapisują swoją oczekiwaną zawartość w plikach `.verified.txt` w katalogu `__snapshots__`. 

Po uruchomieniu testów po raz pierwszy:
1. Testy mogą się nie powieść (brak snapshote)
2. Zostaną utworzone pliki `.received.txt`
3. Developer porównuje i zatwierdza zmiany
4. Pliki `.received.txt` są usuwane, snapshots `.verified.txt` są zatwierdzone

## Zależności

Projekt testów korzysta z:

| Pakiet | Wersja | Przeznaczenie |
|--------|--------|---------------|
| Testcontainers | 4.10.0 | Zarządzanie kontenerami Docker |
| Testcontainers.PostgreSql | 4.10.0 | Kontener PostgreSQL |
| Microsoft.AspNetCore.Mvc.Testing | 10.0.1 | WebApplicationFactory |
| xunit | 2.9.0 | Framework testowy |
| Verify.Xunit | 26.0.0 | Snapshot testing |
| FluentAssertions | 6.12.0 | Assertion library (do logiki nie-snapshot) |
| Moq | 4.20.70 | Mocking framework |

## Architektura Testów

```
┌─────────────────────────────────────────────────┐
│        AuthControllerTests                      │
│  (Test class implementing IAsyncLifetime)       │
└────────────────┬────────────────────────────────┘
                 │
     ┌───────────┴────────────┐
     │                        │
┌────▼──────────────┐   ┌─────▼──────────────────┐
│ PostgresFixture   │   │ AuthTestWebApplication │
│                   │   │ Factory               │
│ - StartAsync()    │   │                       │
│ - DisposeAsync()  │   │ - ConfigureWebHost()  │
└────┬──────────────┘   └─────┬──────────────────┘
     │                        │
     └───────────┬────────────┘
                 │
         ┌───────▼──────────┐
         │ PostgreSQL       │
         │ TestContainer    │
         │ (Docker)         │
         └──────────────────┘
```

## Best Practices Zastosowane

1. **Izolacja Bazy Danych** - Każdy test ma czystą bazę danych dzięki TestContainers
2. **Fixture Pattern** - Użycie `IAsyncLifetime` do zarządzania zasobami
3. **Snapshot Testing** - Weryfikacja struktury API bez twardych walidacji
4. **Semantic Testing** - Weryfikujemy stan logiczny (np. `HasAccessToken`) zamiast dokładnej wartości
5. **Nazewnictwo** - Jasne nazwy testów opisujące scenariusz
6. **Arrange-Act-Assert** - Konsystentna struktura testów
7. **Testy E2E** - Pełny przepływ uwierzytelniania

## Notes

- Testy wymagają uruchomionego Docker Daemon
- Cada test tworzy nową instancję kontenera PostgreSQL
- Testy działają niezależnie od siebie dzięki izolacji bazy danych
- Testcontainers automatycznie czyści zasoby po zakończeniu testów

## Dalsze Rozwijanie

Możliwe rozszerzenia:

1. **Testy wydajności** - Mierzenie czasu odpowiedzi
2. **Testy bezpieczeństwa** - SQL Injection, XSS, CSRF
3. **Testy współbieżności** - Równoczesne żądania
4. **Testy Redis** - Jeśli będzie cache layer
5. **Testy bardziej szczegółowe** - Edge case'i w walidacji
6. **Testy role-based access control** - Kiedy zostanie dodana autoryzacja
7. **Performance baselines** - Snapshot timing dla weryfikacji wydajności

## Zarządzanie Snapshots

### Akceptacja nowych/zmodyfikowanych snapshots

```bash
# Interactive mode - wybierz które snapshots zatwierdzić
dotnet test tests/Bagman.IntegrationTests -- --verify-mode Interactive

# Accept all changes
dotnet test tests/Bagman.IntegrationTests -- --verify-mode AcceptTest
```

### Oczyszczanie snapshots

```bash
# Usuń wszystkie .received.txt pliki
find tests/Bagman.IntegrationTests -name "*.received.*" -delete
```

### Debugging snapshots

W IDE (Rider, Visual Studio) można porównać `.verified.txt` z `.received.txt` wizualnie
