a# Dokumentacja techniczna – System do typowania meczów piłki nożnej  
**Blazor (client-side) + ASP.NET Core API + Azure SQL (MSSQL) + EF Core + ASP.NET Core Identity + MudBlazor**

---

## 1. Architektura systemu

### 1.1. Stack technologiczny

- **Frontend (UI):**  
  - Blazor WebAssembly (C#) – aplikacja całkowicie po stronie klienta, hostowana statycznie.
  - MudBlazor – nowoczesny, lekki framework UI, pozwala na projektowanie nietypowych nawigacji (okienka w okienku).
- **Backend (API):**  
  - ASP.NET Core (.NET 8) – REST API, pośredniczy między Blazor a Azure SQL oraz usługami autoryzacji (ASP.NET Core Identity).
  - API obsługuje autoryzację, logikę biznesową, walidacje, bezpieczeństwo.
 - **Baza danych:**
  - Azure SQL (MSSQL) – przechowuje wszystkie dane aplikacji.
  - API używa Entity Framework Core (Microsoft.EntityFrameworkCore.SqlServer) do dostępu do danych.
  - W razie potrzeby można wykonać bezpośrednie zapytania SQL przez ADO.NET lub Dapper.
- **Autoryzacja:**  
  - ASP.NET Core Identity – zarządzanie kontami (rejestracja, logowanie, sesje) z własnym providerem.
  - API wystawia i weryfikuje tokeny JWT oraz obsługuje refresh tokeny (konfiguracja TTL wedle potrzeb).
  - Blazor automatycznie odświeża JWT token używając refresh token przed wygaśnięciem.
  - Obsługa wygaśnięcia sesji: automatyczne odświeżenie gdy refresh token ważny, wylogowanie z komunikatem gdy oba tokeny wygasły.
- **DevOps/Uruchamianie lokalne:**  
  - VS Code/Visual Studio + .NET CLI
  - Konfiguracja przez appsettings.json + User Secrets (development)
  - GitHub Actions dla CI/CD
  - Wrażliwe klucze przechowywane w GitHub Secrets i ustawiane w procesie deployment
  - Dwa środowiska: development (lokalne) i production

---

## 2. Instalacja i uruchamianie lokalnie

### 2.1. Wymagania systemowe

- **.NET 8 SDK** - główny framework aplikacji
- **Node.js** (opcjonalnie, do niektórych narzędzi UI, np. budowanie stylów)
 - **Konto Azure** (https://portal.azure.com) - Azure SQL, Blob Storage i opcjonalne usługi
 - **Connection string do Azure SQL / dane dostępu** (przechowywane w Secret Manager / GitHub Secrets)
- **Docker Desktop** - wymagany dla testów integracyjnych z Testcontainers
- **Co najmniej 4GB RAM** - dla kontenerów SQL Server w testach
- **Git** - kontrola wersji

### 2.2. Kroki instalacji

1. **Klonowanie repozytorium:**
   ```bash
   git clone https://github.com/twoj-repo/typowanie-meczy.git
   cd typowanie-meczy
   ```

2. **Konfiguracja połączenia do Azure SQL i Identity:**
   - Ustaw connection string w `appsettings.json` lub Secret Managerze:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=tcp:<your-server>.database.windows.net,1433;Initial Catalog=<db>;Persist Security Info=False;User ID=<user>;Password=<password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;"
     }
     ```
   - Dodatkowe ustawienia dla Azure Blob Storage (jeśli używane) i konfiguracja JWT/refresh tokens w sekcji `Jwt`.

3. **Migracja bazy danych:**
  - Użyj EF Core Migrations. Przykładowe komendy:
    ```bash
    dotnet tool install --global dotnet-ef
    dotnet ef migrations add InitialCreate --project src/Bagman.Api
    dotnet ef database update --project src/Bagman.Api
    ```
  - Istniejące skrypty SQL w `db/` mogą pozostać jako referencja; zalecane jest przeniesienie schematu do migracji EF Core.

4. **Instalacja zależności:**
   ```bash
   dotnet restore
   ```

5. **Uruchomienie API:**
   ```bash
   dotnet run --project src/TypowanieMeczy.Api
   ```

6. **Uruchomienie Blazor WebAssembly:**
   ```bash
   dotnet run --project src/TypowanieMeczy.Web
   ```
   - UI będzie dostępne pod `https://localhost:5002`
   - API pod `https://localhost:5001` (domyślnie)

7. **Uruchomienie testów integracyjnych:**
   ```bash
   # Sprawdź czy Docker jest uruchomiony
   docker info
   
   # Uruchom wszystkie testy integracyjne
   ./tests/Bagman.IntegrationTests/run-tests.sh all
   
   # Lub ręcznie
   dotnet test tests/Bagman.IntegrationTests/Bagman.IntegrationTests.csproj
   ```

---

## 3. Architektura aplikacji

### 3.1. Struktura projektu

```
src/
├── Bagman.Api/           # ASP.NET Core API
├── Bagman.Domain/        # Modele domenowe, logika biznesowa
├── Bagman.Infrastructure/ # Repositories, EF Core DbContexts i Migrations
├── Bagman.Contracts/     # Modele odpowiedzi API (współdzielone z UI)
└── Bagman.Web/          # Blazor WebAssembly UI
```

### 3.2. Warstwy

- **UI (Blazor WebAssembly):**  
  - Renderuje widoki, komunikuje się wyłącznie z własnym .NET API przez HTTP (REST).
  - Nie kontaktuje się bezpośrednio z bazą danych — cały dostęp realizowany jest przez API.
- **API (ASP.NET Core):**  
  - **Controllers** - endpointy HTTP, walidacja requestów, mapowanie DTOs przez extension methods
  - **Services** - logika biznesowa, operacje na modelach domenowych
  - **Repositories / Data Access** - dostęp do bazy danych przez EF Core `DbContext` (Repositories mogą używać `DbContext` bezpośrednio)
  - **Domain Models** - modele domenowe z logiką biznesową
  - **Contracts** - modele odpowiedzi API współdzielone z UI
  - **Dependency Injection** - wbudowany .NET DI container (Scoped dla Services/Repositories/DbContext, Singletony dla serwisów, konfigurowalny provider Identity)
  - **CRUD pattern** - standardowe operacje Create, Read, Update, Delete
  - **Result Pattern** - wszystkie metody w Services i Domain Models zwracają Result<T> używając biblioteki ErrorOr (brak wyjątków)
  - **FluentValidation** - walidacja danych wejściowych przyjmowanych przez API
  - Realizuje autoryzację, walidację danych, obsługę uprawnień.
  - Zwroty JSON, statusy HTTP, obsługa błędów.
  - **Azure SQL / Azure services:**  
  - Dane przechowywane w Azure SQL (MSSQL). Autoryzacja realizowana przez ASP.NET Core Identity.
  - Pliki (awatar, eksporty) zalecane do przechowywania w Azure Blob Storage.
  - Realtime / powiadomienia można zrealizować przez SignalR lub Azure SignalR Service.
  - **DevOps:**  
  - Skrypty migracji bazy, seed danych, CI/CD pipeline (testy, deployment), monitoring.

---

## 4. Integracja Blazor <-> API <-> Azure SQL / Identity

- **Blazor komunikuje się WYŁĄCZNIE z własnym API przez REST.**
- **API pośredniczy w autoryzacji użytkownika:**  
  - UI przesyła dane logowania do API  
  - API korzysta z ASP.NET Core Identity (custom provider), generuje JWT i refresh tokeny, zwraca je do UI
  - Zarządzanie sesją po stronie UI (token JWT w localStorage lub sessionStorage)
- **API realizuje całą logikę dostępu do danych:**  
  - Pobieranie/przesyłanie danych (mecze, stoły, typy, pule, statystyki)
  - Walidacja uprawnień i ról (np. admin stołu)
  - Obsługa błędów i logowanie zdarzeń
- **Contracts - współdzielone modele:**  
  - Projekt Bagman.Contracts zawiera modele odpowiedzi API
  - UI korzysta z tych samych modeli co API
  - Zapewnia type safety i spójność między warstwami
- **Aktualizacje danych:**  
  - Brak realtime updates - zmiany w bazie danych są widoczne dopiero po odświeżeniu strony
  - Użytkownicy muszą ręcznie odświeżyć stronę aby zobaczyć nowe typy, wyniki meczów, itp.
  - To upraszcza architekturę i zwiększa bezpieczeństwo (jedyny punkt wejścia przez API)

---

## 5. Bezpieczeństwo

 - **Hasła min. 10 znaków, hashowane przez ASP.NET Core Identity**
- **Dostęp do API chroniony przez JWT (Bearer token)**
- **Weryfikacja uprawnień i ról w API - podejście hybrydowe:**
  - Podstawowa autoryzacja na poziomie endpointów (`[Authorize]` na kontrolerach/akcjach)
  - Szczegółowe sprawdzanie uprawnień w serwisach biznesowych (np. czy użytkownik ma dostęp do stołu, czy może edytować typ)
- **CORS skonfigurowany na API**
- **Ochrona przed typowymi atakami (XSS, CSRF, brute force) – szczegóły w README API**
- **Rate limiting dla endpointów API**
 - **Logowanie zdarzeń i błędów (np. do Azure Blob Storage lub zewnętrznego systemu logującego)**

---

## 6. Walidacja danych

### 6.1. FluentValidation

- **Biblioteka walidacji:** FluentValidation - nowoczesna biblioteka do walidacji danych w .NET
- **Lokalizacja walidatorów:** `src/Bagman.Api/Validators/` - dedykowany katalog dla walidatorów
- **Automatyczna rejestracja:** Walidatory rejestrowane automatycznie przez ASP.NET Core DI container
- **Walidacja requestów:** Wszystkie modele requestów (RegisterRequest, LoginRequest, etc.) mają dedykowane walidatory

### 6.2. Przykłady walidacji

```csharp
// Przykład walidatora dla RegisterRequest
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login jest wymagany")
            .Length(3, 50).WithMessage("Login musi mieć od 3 do 50 znaków")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Login może zawierać tylko litery, cyfry i podkreślnik");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany")
            .EmailAddress().WithMessage("Nieprawidłowy format email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Hasło jest wymagane")
            .MinimumLength(10).WithMessage("Hasło musi mieć minimum 10 znaków")
            .Matches("[A-Z]").WithMessage("Hasło musi zawierać wielką literę")
            .Matches("[a-z]").WithMessage("Hasło musi zawierać małą literę")
            .Matches("[0-9]").WithMessage("Hasło musi zawierać cyfrę");
    }
}
```

### 6.3. Obsługa błędów walidacji

- **Automatyczna walidacja:** ASP.NET Core automatycznie waliduje requesty przed przekazaniem do kontrolerów
- **Format odpowiedzi:** Błędy walidacji zwracane w standardowym formacie JSON z listą błędów
- **HTTP Status:** 400 Bad Request dla błędów walidacji
- **Integracja z ErrorOr:** Błędy walidacji mapowane na ErrorOr.ValidationError

### 6.4. Walidatory w projekcie

- `RegisterRequestValidator` - walidacja danych rejestracji
- `LoginRequestValidator` - walidacja danych logowania
- `CreateTableRequestValidator` - walidacja tworzenia stołu
- `CreateMatchRequestValidator` - walidacja dodawania meczu
- `PlaceBetRequestValidator` - walidacja typowania

### 6.5. Konfiguracja

```csharp
// Program.cs - rejestracja FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
```

---

## 7. Obsługa migracji i wersjonowania bazy danych

- **Skrypty migracji w katalogu `db/migrations` z numeracją wersji**
- **Ręczne uruchamianie migracji w kolejności**
- **Seed danych testowych**
- **Hard delete dla wszystkich encji - fizyczne usuwanie z bazy danych**
- **Instrukcja aktualizacji schematu bazy przy wdrożeniach**
- **Routing zgodny z zasadami REST - bez wersjonowania API**

---

## 8. Testowanie i jakość

### 8.1. Testy jednostkowe (xUnit) – logika API
- **Framework testowy:** xUnit - domyślny framework dla ASP.NET Core
- **Mockowanie:** NSubstitute dla mockowania warstwy dostępu do danych (np. `DbContext` lub repozytoriów)
- **Asercje:** FluentAssertions dla czytelnych i ekspresywnych asercji
- **Pokrycie:** Services, Repositories, Domain Models
- **Result Pattern:** Testowanie ErrorOr - weryfikacja sukcesu/błędów

### 8.2. Testy integracyjne – komunikacja API z Azure SQL / Identity
- **Framework:** xUnit z WebApplicationFactory
- **Kontenery:** Testcontainers (mssql) dla izolowanej bazy SQL Server
- **Pokrycie:** Integracja z ASP.NET Core Identity i Azure SQL
- **Automatyzacja:** Automatyczne uruchamianie i czyszczenie kontenerów testowych
- **Struktura testów:**
  ```
  tests/Bagman.IntegrationTests/
  ├── AuthIntegrationTests.cs              # Testy podstawowe autoryzacji
  ├── AuthErrorScenariosTests.cs           # Testy błędów i edge cases
  ├── AuthPerformanceAndSecurityTests.cs   # Testy wydajnościowe i bezpieczeństwa
  ├── AuthConfigurationTests.cs            # Testy konfiguracji
  ├── TestHelpers/
  │   └── AuthTestHelper.cs                # Helper klasa
  ├── README.md                            # Dokumentacja testów
  └── run-tests.sh                         # Skrypt uruchamiania
  ```

### 8.3. Scenariusze testów integracyjnych
- **Podstawowe operacje autoryzacji:** Rejestracja, logowanie, odświeżanie tokenów, wylogowanie
- **Walidacja danych:** Puste pola, nieprawidłowe formaty, słabe hasła, duplikaty
- **Bezpieczeństwo:** Ochrona przed SQL Injection, XSS, walidacja tokenów JWT
- **Wydajność:** Pomiar czasu odpowiedzi, testy obciążeniowe, wycieki pamięci
- **Konfiguracja:** Różne środowiska, obsługa błędów konfiguracji, CORS

### 8.4. Uruchamianie testów
```bash
# Wszystkie testy integracyjne
./tests/Bagman.IntegrationTests/run-tests.sh all

# Tylko testy podstawowe
./tests/Bagman.IntegrationTests/run-tests.sh basic

# Z raportem pokrycia
./tests/Bagman.IntegrationTests/run-tests.sh coverage

# Ręczne uruchomienie
dotnet test tests/Bagman.IntegrationTests/Bagman.IntegrationTests.csproj
```

### 8.5. Metryki jakości testów
- **Liczba testów integracyjnych:** 62 testy
- **Pokrycie scenariuszy:** >95%
- **Czas wykonania:** ~3 minuty dla całej suity
- **Stabilność:** >99% współczynnik sukcesu
- **Izolacja:** Każdy test może mieć własny kontener SQL Server (Testcontainers mssql) lub współdzielone środowisko testowe

### 8.6. Pozostałe testy
- **Testy E2E:** UI + API (np. Playwright lub Selenium)
- **Testy UI:** Testy manualne
- **Linter C#:** dotnet-format, analiza statyczna kodu
- **CI/CD pipeline:** GitHub Actions z automatycznym uruchamianiem testów

### 8.7. CI/CD Pipeline
- **GitHub Actions** - automatyczne uruchamianie testów przy każdym push
- **Workflow testów:**
  ```yaml
  # .github/workflows/tests.yml
  - name: Run Integration Tests
    run: |
      docker info
      dotnet test tests/Bagman.IntegrationTests/Bagman.IntegrationTests.csproj
      --verbosity normal
      --collect:"XPlat Code Coverage"
  ```
- **Wymagania CI/CD:**
  - Docker dostępny w runnerze
  - .NET 8 SDK
  - Co najmniej 4GB RAM dla kontenerów
- **Raporty:** Automatyczne generowanie raportów pokrycia kodu

---

## 9. Monitoring / Logging

- **Logowanie błędów w API (wbudowany ILogger, output na console)**
- **Logi audytowe (np. ważne operacje: zmiana admina, dodanie meczu, typowanie)**

---

## 10. Performance

- **Memory Cache w API** - cache w pamięci aplikacji z krótkim czasem życia (2 minuty)
- **Caching często używanych danych** - lista meczów, statystyki użytkowników

## 11. Error Handling

- **Hybrydowa obsługa błędów w Blazor** - global error boundary dla nieoczekiwanych błędów + lokalne obsługiwanie dla błędów API

## 12. UI/UX

- **MudBlazor – elastyczne, lekkie komponenty, okienka w okienku, brak typowych dashboardowych nawigacji**
- **Nawigacja w formie kart, paneli, dialogów, nie jako stały pasek**
- **MudBlazor Theme Provider – obsługa light i dark mode**
- **Responsywność przez MudBlazor Grid system**
- **Lazy loading dla komponentów z dużą ilością danych (lista meczów, statystyki)**
- **Loading spinners podczas ładowania danych z API**
- **Mobile-first, duże odstępy, zaokrąglone elementy**
- **Mockupy i wireframes – dołączone w katalogu `design/`**
- **Onboarding użytkownika, rejestracja, reset hasła, flow logowania – opisane w osobnym rozdziale**
- **WCAG 2.1 – minimalne wymagania dostępności**

---

## 13. Model danych

**Szczegółowy ERD i typy pól dołączone w katalogu `db/ERD.png` oraz `db/schema.sql`.**

Przykład encji:

- `users`  
  - id (uuid, PK), login (varchar, unique), password_hash, email (unique), created_at, is_active
- `tables`  
  - id (uuid, PK), name (varchar), password_hash, max_players (int), stake (decimal), created_by (uuid, FK), created_at
- `table_members`  
  - user_id (FK), table_id (FK), is_admin (bool), joined_at
- `matches`  
  - id (uuid, PK), table_id (FK), country_1, country_2, datetime, result, status (enum), started (bool)
- `bets`  
  - id (uuid, PK), user_id (FK), match_id (FK), prediction (varchar), edited_at
- `pools`  
  - id (uuid, PK), match_id (FK), amount (decimal), status (enum: won, rollover, expired), winners[]
- `stats`  
  - user_id (FK), table_id (FK), matches_played, bets_placed, pools_won, total_won

---

## 14. Rozszerzenia i wersja 2.0

- Tryb „tajemniczego typera" – flaga w tabeli `tables`, obsługa w UI i API
 - Powiadomienia – integracja przez SignalR / Azure SignalR Service lub zewnętrzne API (np. SendGrid, Twilio)
- Wersja charytatywna – tabela `charity_history`
- Mobile – PWA (Blazor WebAssembly natywnie wspiera), dedykowany layout mobile
 - Komentarze/czat – SignalR / Azure SignalR Service przez API
- Integracja z zewnętrznym API – np. [API-Football](https://www.api-football.com/), obsługa przez backend API

**Roadmapa jest opisana w katalogu `roadmap.md` z priorytetami i szacunkami czasowymi.**

---

## 15. Monitoring, backup i disaster recovery

- **Backup bazy danych – automatyczne backupy Azure SQL lub eksport przez narzędzia zewnętrzne**
- **Instrukcja odtwarzania backupu**
- **Monitorowanie dostępności API (np. UptimeRobot, HealthChecks w .NET)**
- **Procedury awaryjne – opisane w katalogu `ops/`**

---

## 16. FAQ dla developerów

### Ogólne pytania
- **Jak debugować typowe problemy z Azure SQL/Blazor/API**
- **Jak dodać nową migrację bazy**
- **Jak dodać nowy endpoint API**
- **Jak stylizować MudBlazor zgodnie z wymaganiami UI**

### Testowanie
- **Jak dodać testy do projektu**
- **Jak uruchomić testy integracyjne lokalnie**
- **Jak debugować testy z Testcontainers**
- **Jak dodać nowe scenariusze testowe**

### Troubleshooting testów
```bash
# Problem: Kontener SQL Server nie startuje
docker info
./tests/Bagman.IntegrationTests/run-tests.sh cleanup

# Problem: Testy są wolne
export TESTCONTAINERS_REUSE_ENABLE=true
./tests/Bagman.IntegrationTests/run-tests.sh parallel

# Problem: Błędy pamięci
docker system prune
# Zwiększ pamięć dla Docker Desktop
```

---

## 17. Linki i zasoby

### Dokumentacja technologii
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
- [Azure SQL Database docs](https://learn.microsoft.com/azure/azure-sql/)
- [MudBlazor Docs](https://mudblazor.com/getting-started/installation)
- [Blazor WebAssembly Docs](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-8.0)
- [ASP.NET Core API Docs](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-8.0)
- [ErrorOr Library](https://github.com/amantinband/error-or) - Result Pattern dla .NET
- [FluentValidation Docs](https://docs.fluentvalidation.net/) - Walidacja danych w .NET

### Testowanie
- [xUnit Docs](https://xunit.net/) - Framework testowy
- [Testcontainers .NET](https://dotnet.testcontainers.org/) - Kontenery dla testów
- [FluentAssertions](https://fluentassertions.com/) - Asercje dla testów
- [WebApplicationFactory](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0) - Testowanie ASP.NET Core
- [NSubstitute](https://nsubstitute.github.io/) - Mocking framework

---

**W razie pytań dotyczących wdrożenia lub kodu – zgłaszaj uwagi i propozycje na repozytorium projektu!**

---

# Pytania do Ciebie (w celu dopełnienia dokumentacji)

1. **Czy API będzie publiczne, czy tylko dla frontendu? (Warto doprecyzować politykę CORS i uprawnienia dostępu)**
2. **Czy przewidujesz wersjonowanie API od początku? (np. /api/v1/)**
3. **Czy chcesz obsługiwać upload plików (np. avatarów, eksportów) przez API/Azure Blob Storage?**
4. **Czy potrzebujesz integracji z zewnętrznymi systemami powiadomień (email, SMS)?**
5. **Jakie są preferencje co do hostingu: chmura, VPS, lokalnie?**
6. **Czy w projekcie będą wymagane logi audytowe (np. kto, kiedy zmienił typ, dodał mecz itp.)?**
7. **Czy chcesz, żeby UI był dostępny bez logowania (np. jako demo)?**
8. **Czy jest wymóg wsparcia dla języka angielskiego (multilanguage)?**
9. **Czy są jakieś limity na liczbę stołów, graczy, meczów?**

---

Odpowiedz na powyższe, a doprecyzuję dokumentację według Twoich preferencji! 