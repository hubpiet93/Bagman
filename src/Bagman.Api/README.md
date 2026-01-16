# Bagman API

API dla systemu typowania meczów piłki nożnej.

## Konfiguracja

### 1. Database (Azure SQL / EF Core)

1. Create an Azure SQL database or use a local SQL Server instance.
2. Configure connection string in `appsettings.json` under `ConnectionStrings:DefaultConnection`.
3. Use EF Core migrations to create the schema:

```bash
cd src/Bagman.Api
dotnet ef migrations add Initial --project ../Bagman.Infrastructure/Bagman.Infrastructure.csproj --startup-project .
dotnet ef database update --project ../Bagman.Infrastructure/Bagman.Infrastructure.csproj --startup-project .
```

The project uses EF Core (`Microsoft.EntityFrameworkCore.SqlServer`) for database access.

### 3. Uruchomienie

```bash
cd src/Bagman.Api
dotnet run
```

API będzie dostępne pod adresem `https://localhost:5001`

## Endpointy autoryzacji

### Rejestracja

```
POST /api/auth/register
Content-Type: application/json

{
  "login": "testuser",
  "password": "testpassword123",
  "email": "test@example.com"
}
```

### Logowanie

```
POST /api/auth/login
Content-Type: application/json

{
  "login": "testuser",
  "password": "testpassword123"
}
```

### Odświeżanie tokenu

```
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "your-refresh-token"
}
```

### Wylogowanie

```
POST /api/auth/logout
Authorization: Bearer your-access-token
Content-Type: application/json

{
  "refreshToken": "your-refresh-token"
}
```

## Walidacja

- Login: 3-50 znaków, tylko litery, cyfry i podkreślenia
- Hasło: minimum 10 znaków, musi zawierać wielkie i małe litery oraz cyfry
- Email: prawidłowy format email

## Bezpieczeństwo

- JWT tokeny z automatycznym odświeżaniem
- CORS skonfigurowany dla Blazor aplikacji
- Walidacja wszystkich requestów 