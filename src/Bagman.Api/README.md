# Bagman API

API dla systemu typowania meczów piłki nożnej.

## Konfiguracja

### 1. Supabase

1. Utwórz projekt w [Supabase](https://supabase.com)
2. Skopiuj URL i klucz anonimowy z ustawień projektu
3. Zaktualizuj `appsettings.json` lub `appsettings.Development.json`:

```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "AnonKey": "your-anon-key"
  }
}
```

### 2. Baza danych

1. Uruchom skrypt `db/schema.sql` w swojej bazie Supabase
2. Skrypt utworzy wszystkie potrzebne tabele i indeksy

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

- Hasła są hashowane przez Supabase Auth
- JWT tokeny z automatycznym odświeżaniem
- CORS skonfigurowany dla Blazor aplikacji
- Walidacja wszystkich requestów 