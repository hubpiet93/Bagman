# Bagman API – Dokumentacja na podstawie testów integracyjnych

> Dokumentacja została wygenerowana na podstawie snapshotów testów integracyjnych.
> Każdy test wykonuje sekwencję wywołań HTTP do API Bagman, a snapshot zawiera pełny zapis wszystkich żądań i odpowiedzi.
> Dane wrażliwe (tokeny, loginy, maile, identyfikatory użytkowników, daty) są w snapshotach zanonimizowane (`{Scrubbed}`, `Guid_X`, `DateTimeOffset_X`).
> Adres api to: `https://bagman-production.up.railway.app`
---

## Spis treści

1. [AuthController](#authcontroller)
    - [POST /api/auth/register](#post-apiauthregister)
    - [POST /api/auth/login](#post-apiauthlogin)
    - [POST /api/auth/refresh](#post-apiauthrefresh)
    - [POST /api/auth/logout](#post-apiauthlogout)
2. [EventTypesController](#eventtypescontroller)
    - [GET /api/event-types](#get-apievent-types)
    - [POST /api/admin/event-types](#post-apiadminevent-types)
    - [PUT /api/admin/event-types/{id}](#put-apiadminevent-typesid)
    - [POST /api/admin/event-types/{id}/deactivate](#post-apiadminevent-typesiddeactivate)
    - [POST /api/admin/event-types/{eventTypeId}/matches](#post-apiadminevent-typeseventtypeidmatches)
3. [TablesController](#tablescontroller)
    - [POST /api/tables](#post-apitables)
    - [POST /api/tables/create](#post-apitablescreate)
    - [POST /api/tables/join](#post-apitablesjoin)
    - [POST /api/tables/{tableId}/join](#post-apitablestableidjoin)
    - [GET /api/tables](#get-apitables)
    - [GET /api/tables/{tableId}](#get-apitablestableid)
    - [GET /api/tables/{tableId}/dashboard](#get-apitablestableiddashboard)
    - [DELETE /api/tables/{tableId}/members](#delete-apitablestableidmembers)
    - [POST /api/tables/{tableId}/admins](#post-apitablestableidadmins)
    - [DELETE /api/tables/{tableId}/admins/{userId}](#delete-apitablestableidadminsuserid)
4. [MatchesController](#matchescontroller)
    - [GET /api/tables/{tableId}/matches/{matchId}](#get-apitablestableidmatchesmatchid)
5. [BetsController](#betscontroller)
    - [POST /api/tables/{tableId}/matches/{matchId}/bets](#post-apitablestableidmatchesmatchidbets)
    - [GET /api/tables/{tableId}/matches/{matchId}/bets/my](#get-apitablestableidmatchesmatchidbetsmy)
    - [DELETE /api/tables/{tableId}/matches/{matchId}/bets](#delete-apitablestableidmatchesmatchidbets)
6. [Typy modeli](#typy-modeli)
    - [Auth](#auth--modele)
    - [EventTypes](#eventtypes--modele)
    - [Tables](#tables--modele)
    - [Matches](#matches--modele)
    - [Bets](#bets--modele)
7. [Błędy i kody statusu](#błędy-i-kody-statusu)
    - [Walidacja 400 – RFC 9110](#walidacja-400--rfc-9110)
    - [Błędy domenowe (tablice `errors`)](#błędy-domenowe-tablice-errors)
    - [401 Unauthorized](#401-unauthorized)
    - [403 Forbidden](#403-forbidden)
    - [404 Not Found](#404-not-found)
    - [409 Conflict](#409-conflict)
    - [Zestawienie kodów statusu](#zestawienie-kodów-statusu)

---

## AuthController

### POST /api/auth/register

Rejestracja nowego użytkownika i zwrócenie tokenów uwierzytelniających.

- **Metoda:** `POST`
- **Ścieżka:** `/api/auth/register`
- **Autoryzacja:** niewymagana

#### Body (request)

```json
{
  "Login": "string",
  "Password": "string",
  "Email": "string"
}
```

Walidacje obserwowane w snapshotach:

- `Email`:
    - musi mieć poprawny format adresu email
- `Password`:
    - minimalna długość: 10 znaków
    - musi zawierać wielką literę
    - musi zawierać cyfrę
    - musi zawierać znak specjalny

#### Przykładowe żądanie

```http
POST /api/auth/register HTTP/1.1
Content-Type: application/json

{
  "Login": "testuser",
  "Password": "Test@12345",
  "Email": "test@example.com"
}
```

#### Odpowiedź 200 OK

```json
{
  "accessToken": "{Scrubbed}",
  "refreshToken": "{Scrubbed}",
  "expiresAt": "DateTimeOffset_1",
  "user": {
    "id": "Guid_1",
    "login": "{Scrubbed}",
    "email": "{Scrubbed}",
    "createdAt": "DateTimeOffset_2",
    "isActive": true
  }
}
```

#### Odpowiedź 400 BadRequest – niepoprawny email

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": "{Scrubbed}"
  },
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 400 BadRequest – słabe hasło

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Password": [
      "Hasło musi mieć minimum 10 znaków",
      "Hasło musi zawierać wielką literę",
      "Hasło musi zawierać cyfrę",
      "Hasło musi zawierać znak specjalny"
    ]
  },
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 400 BadRequest – duplikat loginu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "User.AlreadyExists",
      "description": "Użytkownik o podanym loginie już istnieje"
    }
  ]
}
```

#### Kody statusu

- `200 OK` – poprawna rejestracja
- `400 BadRequest` – błędy walidacyjne lub duplikat użytkownika

---

### POST /api/auth/login

Logowanie użytkownika i zwrócenie tokenów.

- **Metoda:** `POST`
- **Ścieżka:** `/api/auth/login`
- **Autoryzacja:** niewymagana

#### Body (request)

```json
{
  "Login": "string",
  "Password": "string"
}
```

#### Przykładowe żądanie

```http
POST /api/auth/login HTTP/1.1
Content-Type: application/json

{
  "Login": "loginuser",
  "Password": "Test@12345"
}
```

#### Odpowiedź 200 OK

Struktura taka jak w `/api/auth/register`:

```json
{
  "accessToken": "{Scrubbed}",
  "refreshToken": "{Scrubbed}",
  "expiresAt": "DateTimeOffset_3",
  "user": {
    "id": "Guid_1",
    "login": "{Scrubbed}",
    "email": "{Scrubbed}",
    "createdAt": "DateTimeOffset_2",
    "isActive": true
  }
}
```

#### Odpowiedź 400 BadRequest – błędne dane logowania

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "Auth.InvalidCredentials",
      "description": "Nieprawidłowy login lub hasło"
    }
  ]
}
```

#### Kody statusu

- `200 OK` – poprawne logowanie
- `400 BadRequest` – błędne dane logowania

---

### POST /api/auth/refresh

Odświeżenie tokenów na podstawie ważnego refresh tokena.

- **Metoda:** `POST`
- **Ścieżka:** `/api/auth/refresh`
- **Autoryzacja:** niewymagana (refresh token w body)

#### Body (request)

```json
{
  "RefreshToken": "string"
}
```

#### Przykładowe żądanie

```http
POST /api/auth/refresh HTTP/1.1
Content-Type: application/json

{
  "RefreshToken": "{Scrubbed}"
}
```

#### Odpowiedź 200 OK

```json
{
  "accessToken": "{Scrubbed}",
  "refreshToken": "{Scrubbed}",
  "expiresAt": "DateTimeOffset_4",
  "user": {
    "id": "Guid_1",
    "login": "{Scrubbed}",
    "email": "{Scrubbed}",
    "createdAt": "DateTimeOffset_2",
    "isActive": true
  }
}
```

#### Odpowiedź 400 BadRequest – refresh token nie istnieje

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "Auth.RefreshNotFound",
      "description": "Refresh token nie został znaleziony"
    }
  ]
}
```

#### Kody statusu

- `200 OK` – poprawne odświeżenie
- `400 BadRequest` – błędny/nieistniejący refresh token

---

### POST /api/auth/logout

Wylogowanie na podstawie refresh tokena.

- **Metoda:** `POST`
- **Ścieżka:** `/api/auth/logout`
- **Autoryzacja:** niewymagana (refresh token w body)

#### Body (request)

```json
{
  "RefreshToken": "string"
}
```

#### Przykładowe żądanie

```http
POST /api/auth/logout HTTP/1.1
Content-Type: application/json

{
  "RefreshToken": "{Scrubbed}"
}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Wylogowano pomyślnie"
}
```

#### Odpowiedź 400 BadRequest – nieprawidłowy refresh token

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "RefreshToken.NotFound",
      "description": "Refresh token nie został znaleziony"
    }
  ]
}
```

#### Kody statusu

- `200 OK` – wylogowanie zakończone powodzeniem
- `400 BadRequest` – nieprawidłowy refresh token

---

## EventTypesController

### GET /api/event-types

Pobranie listy aktywnych typów wydarzeń. Endpoint publiczny.

- **Metoda:** `GET`
- **Ścieżka:** `/api/event-types`
- **Autoryzacja:** niewymagana

#### Przykładowe żądanie

```http
GET /api/event-types HTTP/1.1
```

#### Odpowiedź 200 OK

```json
{
  "eventTypes": [
    {
      "id": "Guid_1",
      "code": "TEST_DEFAULT",
      "name": "Default Test Event",
      "startDate": "DateTimeOffset_1",
      "isActive": true
    }
  ]
}
```

#### Kody statusu

- `200 OK` – lista typów wydarzeń

---

### POST /api/admin/event-types

Utworzenie nowego typu wydarzenia. Dostępne tylko dla SuperAdmin.

- **Metoda:** `POST`
- **Ścieżka:** `/api/admin/event-types`
- **Autoryzacja:** wymagana (`Bearer` + rola SuperAdmin)

#### Nagłówki

```http
Authorization: Bearer {accessToken}
Content-Type: application/json
```

#### Body (request)

```json
{
  "Code": "string",
  "Name": "string",
  "StartDate": "DateTimeOffset"
}
```

#### Przykładowe żądanie

```http
POST /api/admin/event-types HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Code": "LIGA_MISTRZOW_2026",
  "Name": "Liga Mistrzów 2025/2026",
  "StartDate": "2026-09-01T00:00:00Z"
}
```

#### Odpowiedź 201 Created

Nagłówki:
- `Location: http://localhost/api/event-types?id=Guid_2`

```json
{
  "id": "Guid_2",
  "code": "LIGA_MISTRZOW_2026",
  "name": "Liga Mistrzów 2025/2026",
  "startDate": "DateTimeOffset_4",
  "isActive": true
}
```

#### Odpowiedź 403 Forbidden – brak roli SuperAdmin

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.4",
  "title": "Forbidden",
  "status": 403,
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 409 Conflict – duplikat kodu

```json
{
  "code": "EventType.CodeExists",
  "message": "Typ wydarzenia o podanym kodzie już istnieje"
}
```

#### Kody statusu

- `201 Created` – typ wydarzenia utworzony
- `403 Forbidden` – brak roli SuperAdmin
- `409 Conflict` – duplikat kodu wydarzenia

---

### PUT /api/admin/event-types/{id}

Aktualizacja typu wydarzenia. Dostępne tylko dla SuperAdmin.

- **Metoda:** `PUT`
- **Ścieżka:** `/api/admin/event-types/{id}`
- **Autoryzacja:** wymagana (`Bearer` + rola SuperAdmin)

#### Parametry ścieżki

- `id` – identyfikator typu wydarzenia (GUID)

#### Nagłówki

```http
Authorization: Bearer {accessToken}
Content-Type: application/json
```

#### Body (request)

```json
{
  "Name": "string",
  "StartDate": "DateTimeOffset"
}
```

#### Przykładowe żądanie

```http
PUT /api/admin/event-types/Guid_2 HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Name": "Updated Test Event",
  "StartDate": "2026-10-01T00:00:00Z"
}
```

#### Odpowiedź 200 OK

```json
{
  "id": "Guid_2",
  "code": "TEST_DEFAULT",
  "name": "Updated Test Event",
  "startDate": "DateTimeOffset_4",
  "isActive": true
}
```

#### Kody statusu

- `200 OK` – typ wydarzenia zaktualizowany
- `403 Forbidden` – brak roli SuperAdmin
- `404 Not Found` – typ wydarzenia nie istnieje

---

### POST /api/admin/event-types/{id}/deactivate

Dezaktywacja typu wydarzenia. Dostępne tylko dla SuperAdmin.

- **Metoda:** `POST`
- **Ścieżka:** `/api/admin/event-types/{id}/deactivate`
- **Autoryzacja:** wymagana (`Bearer` + rola SuperAdmin)

#### Parametry ścieżki

- `id` – identyfikator typu wydarzenia (GUID)

#### Nagłówki

```http
Authorization: Bearer {accessToken}
```

#### Przykładowe żądanie

```http
POST /api/admin/event-types/Guid_2/deactivate HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Typ wydarzenia został dezaktywowany"
}
```

#### Odpowiedź 403 Forbidden – brak roli SuperAdmin

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.4",
  "title": "Forbidden",
  "status": 403,
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

- `200 OK` – typ wydarzenia dezaktywowany
- `403 Forbidden` – brak roli SuperAdmin
- `404 Not Found` – typ wydarzenia nie istnieje

---

### POST /api/admin/event-types/{eventTypeId}/matches

Utworzenie meczu w ramach typu wydarzenia. Dostępne tylko dla SuperAdmin.

- **Metoda:** `POST`
- **Ścieżka:** `/api/admin/event-types/{eventTypeId}/matches`
- **Autoryzacja:** wymagana (`Bearer` + rola SuperAdmin)

#### Parametry ścieżki

- `eventTypeId` – identyfikator typu wydarzenia (GUID)

#### Nagłówki

```http
Authorization: Bearer {accessToken}
Content-Type: application/json
```

#### Body (request)

```json
{
  "Country1": "string",
  "Country2": "string",
  "MatchDateTime": "DateTimeOffset"
}
```

#### Przykładowe żądanie

```http
POST /api/admin/event-types/Guid_5/matches HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Country1": "Poland",
  "Country2": "Spain",
  "MatchDateTime": "2026-06-01T18:00:00Z"
}
```

#### Odpowiedź 201 Created

Nagłówki:
- `Location: /api/admin/event-types/Guid_5/matches/Guid_8`

```json
{
  "id": "Guid_8",
  "eventTypeId": "Guid_5",
  "country1": "Poland",
  "country2": "Spain",
  "matchDateTime": "DateTimeOffset_11",
  "status": "scheduled",
  "createdAt": "DateTimeOffset_12"
}
```

#### Kody statusu

- `201 Created` – mecz utworzony
- `400 BadRequest` – błędy walidacyjne
- `403 Forbidden` – brak roli SuperAdmin
- `404 Not Found` – typ wydarzenia nie istnieje

---

## TablesController

### POST /api/tables

Tworzenie nowego stołu do typowania meczów. Endpoint równocześnie rejestruje/loguje użytkownika–twórcę stołu.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables`
- **Autoryzacja:** niewymagana (login/hasło w body)

#### Body (request)

```json
{
  "UserLogin": "string",
  "UserPassword": "string",
  "TableName": "string",
  "TablePassword": "string",
  "MaxPlayers": 10,
  "Stake": 50.0,
  "EventTypeId": "Guid"
}
```

Walidacje zaobserwowane:

- `TableName`:
    - nie może być pusty (`""`)
- `Stake`:
    - nie może być ujemny
- `EventTypeId`:
    - musi wskazywać na istniejący typ wydarzenia

#### Przykładowe żądanie – poprawne

```http
POST /api/tables HTTP/1.1
Content-Type: application/json

{
  "UserLogin": "creator_user_1234abcd",
  "UserPassword": "Creator@12345",
  "TableName": "Test Betting Table",
  "TablePassword": "TablePass@123",
  "MaxPlayers": 10,
  "Stake": 50.0,
  "EventTypeId": "Guid_2"
}
```

#### Odpowiedź 201 Created

Nagłówki:
- `Location: http://localhost/api/Tables/Guid_3`

```json
{
  "id": "Guid_3",
  "name": "Test Betting Table",
  "maxPlayers": 10,
  "stake": 50.0,
  "createdBy": "Guid_1",
  "createdAt": "DateTimeOffset_1",
  "isSecretMode": false
}
```

#### Odpowiedź 400 BadRequest – pusta nazwa stołu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "TableName": [
      "Nazwa stołu jest wymagana"
    ]
  },
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 400 BadRequest – ujemna stawka

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Stake": [
      "Stawka nie może być ujemna"
    ]
  },
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

- `201 Created` – stół utworzony
- `400 BadRequest` – błędy walidacyjne

---

### POST /api/tables/create

Tworzenie nowego stołu przez zalogowanego użytkownika.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/create`
- **Autoryzacja:** wymagana (`Bearer`)

#### Nagłówki

```http
Authorization: Bearer {accessToken}
Content-Type: application/json
```

#### Body (request)

```json
{
  "TableName": "string",
  "TablePassword": "string",
  "MaxPlayers": 10,
  "Stake": 50.0,
  "EventTypeId": "Guid"
}
```

Walidacje:
- `TableName`: nie może być pusty, maksymalnie 100 znaków
- `TablePassword`: wymagane, maksymalnie 255 znaków
- `MaxPlayers`: > 0 i ≤ 1000
- `Stake`: ≥ 0

#### Przykładowe żądanie – poprawne

```http
POST /api/tables/create HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "TableName": "Authorized Table Guid_1",
  "TablePassword": "AuthTablePass@123",
  "MaxPlayers": 10,
  "Stake": 25.0,
  "EventTypeId": "Guid_2"
}
```

#### Odpowiedź 201 Created

```json
{
  "id": "Guid_3",
  "name": "Authorized Table Guid_1",
  "maxPlayers": 10,
  "stake": 25.0,
  "createdBy": "Guid_1",
  "createdAt": "DateTimeOffset_1",
  "isSecretMode": false
}
```

#### Odpowiedź 401 Unauthorized – brak lub nieważny token

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 409 Conflict – duplikat nazwy stołu

```json
{
  "code": "Table.DuplicateName",
  "message": "Stół o podanej nazwie już istnieje"
}
```

#### Kody statusu

- `201 Created` – stół utworzony
- `401 Unauthorized` – brak/niepoprawny token
- `409 Conflict` – duplikat nazwy stołu
- `400 BadRequest` – błędy walidacyjne

---

### POST /api/tables/join

Dołączenie do istniejącego stołu. Endpoint równocześnie loguje użytkownika.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/join`
- **Autoryzacja:** niewymagana (login/hasło w body)

#### Body (request)

```json
{
  "UserLogin": "string",
  "UserPassword": "string",
  "TableName": "string",
  "TablePassword": "string"
}
```

#### Przykładowe żądanie – poprawne

```http
POST /api/tables/join HTTP/1.1
Content-Type: application/json

{
  "UserLogin": "joiner_1234abcd",
  "UserPassword": "Joiner@12345",
  "TableName": "Joinable Table Guid_1",
  "TablePassword": "JoinPass@123"
}
```

#### Odpowiedź 200 OK – poprawne dołączenie

```json
{
  "id": "Guid_2",
  "name": "Joinable Table Guid_1",
  "maxPlayers": 5,
  "stake": 100.0,
  "createdBy": "Guid_3",
  "createdAt": "DateTimeOffset_1",
  "isSecretMode": false
}
```

#### Odpowiedź 400 BadRequest – błędne hasło do stołu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "Table.InvalidPassword",
      "description": "Nieprawidłowe hasło do stołu"
    }
  ]
}
```

#### Odpowiedź 400 BadRequest – stół pełny

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "Table.Full",
      "description": "Stół jest pełny"
    }
  ]
}
```

#### Kody statusu

- `200 OK` – dołączenie zakończone powodzeniem
- `400 BadRequest` – błędne hasło lub stół pełny

---

### POST /api/tables/{tableId}/join

Dołączenie do istniejącego stołu przez zalogowanego użytkownika.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/{tableId}/join`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId` – identyfikator stołu (GUID)

#### Nagłówki

```http
Authorization: Bearer {accessToken}
Content-Type: application/json
```

#### Body (request)

```json
{
  "Password": "string"
}
```

#### Przykładowe żądanie – poprawne

```http
POST /api/tables/Guid_3/join HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Password": "TablePass@123"
}
```

#### Odpowiedź 200 OK – poprawne dołączenie

```json
{
  "tableId": "Guid_3",
  "tableName": "Authorized Join Table Guid_1",
  "maxPlayers": 10,
  "stake": 50.0,
  "tableCreatedAt": "DateTimeOffset_1",
  "userId": "Guid_5",
  "userLogin": "{Scrubbed}",
  "isAdmin": false,
  "joinedAt": "DateTimeOffset_4",
  "currentMemberCount": 2
}
```

#### Odpowiedź 400 BadRequest – błędne hasło do stołu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "code": "Table.InvalidPassword",
      "description": "Nieprawidłowe hasło do stołu"
    }
  ]
}
```

#### Odpowiedź 400 BadRequest – stół pełny

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "code": "Table.Full",
      "description": "Stół jest pełny"
    }
  ]
}
```

#### Odpowiedź 400 BadRequest – brak hasła

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Password": [
      "Hasło jest wymagane"
    ]
  },
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 401 Unauthorized – brak tokenu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 404 Not Found – stół nie istnieje

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 409 Conflict – użytkownik już jest członkiem

```json
{
  "errors": [
    {
      "code": "Table.AlreadyMember",
      "description": "Użytkownik jest już członkiem tego stołu"
    }
  ]
}
```

#### Kody statusu

- `200 OK` – dołączenie zakończone powodzeniem
- `400 BadRequest` – błędne hasło, stół pełny lub brak hasła
- `401 Unauthorized` – brak tokenu
- `404 Not Found` – stół nie istnieje
- `409 Conflict` – użytkownik już jest członkiem

---

### GET /api/tables

Pobranie listy stołów, których użytkownik jest członkiem.

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables`
- **Autoryzacja:** wymagana (`Bearer`)

#### Nagłówki

```http
Authorization: Bearer {accessToken}
```

#### Przykładowe żądanie

```http
GET /api/tables HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
[
  {
    "id": "Guid_3",
    "name": "Table One Guid_2",
    "maxPlayers": 5,
    "stake": 50.0,
    "createdBy": "Guid_1",
    "createdAt": "DateTimeOffset_3",
    "isSecretMode": false
  },
  {
    "id": "Guid_5",
    "name": "Table Two Guid_4",
    "maxPlayers": 10,
    "stake": 100.0,
    "createdBy": "Guid_1",
    "createdAt": "DateTimeOffset_4",
    "isSecretMode": false
  }
]
```

#### Kody statusu

- `200 OK` – lista stołów użytkownika
- `401 Unauthorized` – brak/niepoprawny token

---

### GET /api/tables/{tableId}

Pobranie szczegółów stołu i listy członków.

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables/{tableId}`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId` – identyfikator stołu (GUID)

#### Przykładowe żądanie

```http
GET /api/tables/Guid_3 HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "id": "Guid_3",
  "name": "Details Table Guid_2",
  "maxPlayers": 5,
  "stake": 50.0,
  "createdAt": "DateTimeOffset_3",
  "members": [
    {
      "userId": "Guid_1",
      "login": "{Scrubbed}",
      "isAdmin": true,
      "joinedAt": "DateTimeOffset_4"
    },
    {
      "userId": "Guid_4",
      "login": "{Scrubbed}",
      "isAdmin": false,
      "joinedAt": "DateTimeOffset_5"
    }
  ]
}
```

#### Kody statusu

- `200 OK` – szczegóły stołu
- `401 Unauthorized` – brak autoryzacji
- `404 NotFound` – stół nie istnieje

---

### GET /api/tables/{tableId}/dashboard

Pobranie pełnego dashboardu stołu z informacjami o członkach, meczach i zakładach.

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables/{tableId}/dashboard`
- **Autoryzacja:** wymagana (`Bearer`, użytkownik musi być członkiem stołu)

#### Parametry ścieżki

- `tableId` – identyfikator stołu (GUID)

#### Nagłówki

```http
Authorization: Bearer {accessToken}
```

#### Przykładowe żądanie

```http
GET /api/tables/Guid_6/dashboard HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "table": {
    "id": "Guid_6",
    "name": "Full Data Table Guid_4",
    "maxPlayers": 10,
    "stake": 50.0,
    "createdAt": "DateTimeOffset_7"
  },
  "members": [
    {
      "userId": "Guid_1",
      "login": "{Scrubbed}",
      "isAdmin": true,
      "joinedAt": "DateTimeOffset_7"
    },
    {
      "userId": "Guid_2",
      "login": "{Scrubbed}",
      "isAdmin": false,
      "joinedAt": "DateTimeOffset_18"
    },
    {
      "userId": "Guid_3",
      "login": "{Scrubbed}",
      "isAdmin": false,
      "joinedAt": "DateTimeOffset_19"
    }
  ],
  "matches": [
    {
      "id": "Guid_8",
      "country1": "Poland",
      "country2": "Spain",
      "matchDateTime": "DateTimeOffset_11",
      "result": "2:1",
      "isStarted": true
    },
    {
      "id": "Guid_9",
      "country1": "Germany",
      "country2": "France",
      "matchDateTime": "DateTimeOffset_13",
      "result": null,
      "isStarted": false
    }
  ],
  "bets": [
    {
      "id": "Guid_10",
      "userId": "Guid_1",
      "matchId": "Guid_8",
      "prediction": "2:1",
      "editedAt": "DateTimeOffset_15"
    },
    {
      "id": "Guid_11",
      "userId": "Guid_2",
      "matchId": "Guid_8",
      "prediction": "1:0",
      "editedAt": "DateTimeOffset_16"
    },
    {
      "id": "Guid_12",
      "userId": "Guid_3",
      "matchId": "Guid_9",
      "prediction": "X",
      "editedAt": "DateTimeOffset_17"
    }
  ],
  "pools": [],
  "stats": [],
  "leaderboard": [
    {
      "position": 1,
      "userId": "Guid_1",
      "login": "{Scrubbed}",
      "points": 3,
      "exactHits": 1,
      "winnerHits": 0,
      "totalBets": 1,
      "accuracy": 100.0
    },
    {
      "position": 2,
      "userId": "Guid_2",
      "login": "{Scrubbed}",
      "points": 1,
      "exactHits": 0,
      "winnerHits": 1,
      "totalBets": 1,
      "accuracy": 100.0
    }
  ]
}
```

#### System punktacji leaderboard

Leaderboard pokazuje ranking członków stołu na podstawie trafności typów.

**Punktacja:**

| Typ trafienia | Punkty | Opis |
|---------------|--------|------|
| **Exact Hit** | 3 | Dokładne trafienie wyniku (np. przewidywanie "2:1" przy wyniku "2:1") |
| **Winner Hit** | 1 | Trafienie zwycięzcy/remisu, ale z innym wynikiem (np. "2:0" przy wyniku "1:0") |
| **Miss** | 0 | Nietrafiony typ (np. przewidywanie "2:0" przy wyniku "0:1") |

**Typy specjalne:**

- Przewidywanie `"X"` oznacza remis i pasuje do każdego remisowego wyniku (0:0, 1:1, 2:2, itd.) jako **Winner Hit**

**Sortowanie rankingu:**

1. Punkty (malejąco)
2. Liczba Exact Hits (malejąco)
3. Procent trafności (malejąco)

**Obliczanie Accuracy:**

```
accuracy = (exactHits + winnerHits) / totalBets × 100
```

#### Odpowiedź 401 Unauthorized – brak tokenu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 403 Forbidden – użytkownik nie jest członkiem stołu

```json
{
  "errors": [
    {
      "code": "Table.AccessDenied",
      "description": "Nie masz dostępu do tego stołu"
    }
  ]
}
```

#### Odpowiedź 404 Not Found – stół nie istnieje

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

- `200 OK` – dashboard zwrócony
- `401 Unauthorized` – brak tokenu
- `403 Forbidden` – użytkownik nie jest członkiem stołu
- `404 Not Found` – stół nie istnieje

---

### DELETE /api/tables/{tableId}/members

Opuszczenie stołu przez zalogowanego użytkownika (członka stołu).

- **Metoda:** `DELETE`
- **Ścieżka:** `/api/tables/{tableId}/members`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId` – identyfikator stołu

#### Przykładowe żądanie

```http
DELETE /api/tables/Guid_2/members HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Successfully left table"
}
```

#### Kody statusu

- `200 OK` – użytkownik opuścił stół
- `401 Unauthorized` – brak autoryzacji
- `404 NotFound` – użytkownik nie jest członkiem stołu lub stół nie istnieje

---

### POST /api/tables/{tableId}/admins

Nadanie roli administratora innemu użytkownikowi stołu. Wymagane uprawnienia twórcy stołu.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/{tableId}/admins`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId` – identyfikator stołu

#### Body (request)

```json
{
  "UserId": "Guid"
}
```

#### Przykładowe żądanie

```http
POST /api/tables/Guid_4/admins HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "UserId": "Guid_2"
}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Admin role granted"
}
```

#### Kody statusu

- `200 OK` – rola nadana
- `401 Unauthorized` – brak autoryzacji
- `403 Forbidden` – użytkownik nie jest uprawniony do zarządzania adminami
- `404 NotFound` – stół lub użytkownik nie istnieje

---

### DELETE /api/tables/{tableId}/admins/{userId}

Odebranie roli administratora użytkownikowi stołu.

- **Metoda:** `DELETE`
- **Ścieżka:** `/api/tables/{tableId}/admins/{userId}`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId` – identyfikator stołu
- `userId` – identyfikator użytkownika, któremu odbieramy rolę

#### Przykładowe żądanie

```http
DELETE /api/tables/Guid_4/admins/Guid_2 HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Admin role revoked"
}
```

#### Kody statusu

- `200 OK` – rola odebrana
- `401 Unauthorized`
- `403 Forbidden`
- `404 NotFound`

---

## MatchesController

### GET /api/tables/{tableId}/matches/{matchId}

Pobranie szczegółów meczu.

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId` – identyfikator stołu
- `matchId` – identyfikator meczu

#### Przykładowe żądanie

```http
GET /api/tables/Guid_4/matches/Guid_6 HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "id": "Guid_6",
  "eventTypeId": "Guid_3",
  "country1": "Spain",
  "country2": "Portugal",
  "matchDateTime": "DateTimeOffset_7",
  "result": null,
  "status": "scheduled",
  "started": true,
  "createdAt": "DateTimeOffset_8"
}
```

> Pole `started` jest dynamiczne: `false` dla przyszłych meczów, `true` po przekroczeniu czasu meczu.

#### Kody statusu

- `200 OK`
- `401 Unauthorized`
- `404 NotFound` – mecz nie istnieje

---

## BetsController

### POST /api/tables/{tableId}/matches/{matchId}/bets

Utworzenie lub aktualizacja typu (zakładu) użytkownika na mecz.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}/bets`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId`
- `matchId`

#### Body (request)

```json
{
  "Prediction": "string"
}
```

Dozwolone formaty:

- wynik meczu: `"gole1:gole2"` np. `"2:1"`
- remis: `"X"`

Zabronione:

- inne stringi, np. `"invalid prediction format"`

#### Przykładowe żądanie – poprawny wynik

```http
POST /api/tables/Guid_2/matches/Guid_5/bets HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Prediction": "2:1"
}
```

#### Odpowiedź 201 Created – nowy zakład

Nagłówki:
- `Location: http://localhost/api/tables/Guid_2/matches/Guid_5/bets/my`

```json
{
  "id": "Guid_6",
  "userId": "Guid_4",
  "matchId": "Guid_5",
  "prediction": "2:1",
  "editedAt": "DateTimeOffset_8"
}
```

#### Odpowiedź 201 Created – aktualizacja istniejącego zakładu

Kolejne wywołanie zastępuje typ:

```json
{
  "id": "Guid_6",
  "userId": "Guid_4",
  "matchId": "Guid_5",
  "prediction": "3:0",
  "editedAt": "DateTimeOffset_9"
}
```

#### Odpowiedź 201 Created – remis

```json
{
  "id": "Guid_6",
  "userId": "Guid_4",
  "matchId": "Guid_5",
  "prediction": "X",
  "editedAt": "DateTimeOffset_8"
}
```

#### Odpowiedź 400 BadRequest – niepoprawny format typu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Prediction": [
      "Typowanie nie może mieć więcej niż 10 znaków",
      "Typowanie musi być w formacie 'wynik1:wynik2' lub 'X' (remis)"
    ]
  },
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

- `201 Created` – zakład utworzony lub zaktualizowany
- `400 BadRequest` – błędny format `Prediction`
- `401 Unauthorized` – brak tokena
- `404 NotFound` – mecz/ stół nie istnieje

---

### GET /api/tables/{tableId}/matches/{matchId}/bets/my

Pobranie zakładu aktualnie zalogowanego użytkownika na dany mecz.

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}/bets/my`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId`
- `matchId`

#### Przykładowe żądanie

```http
GET /api/tables/Guid_2/matches/Guid_5/bets/my HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK – zakład istnieje

```json
{
  "id": "Guid_6",
  "userId": "Guid_4",
  "matchId": "Guid_5",
  "prediction": "3:2",
  "editedAt": "DateTimeOffset_8"
}
```

#### Odpowiedź 404 NotFound – brak zakładu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

- `200 OK`
- `404 NotFound` – użytkownik nie ma zakładu
- `401 Unauthorized`

---

### DELETE /api/tables/{tableId}/matches/{matchId}/bets

Usunięcie zakładu użytkownika przed rozpoczęciem meczu.

- **Metoda:** `DELETE`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}/bets`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId`
- `matchId`

#### Przykładowe żądanie

```http
DELETE /api/tables/Guid_2/matches/Guid_5/bets HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK – zakład usunięty

```json
{
  "message": "Bet deleted successfully"
}
```

#### Odpowiedź 404 NotFound – brak zakładu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

- `200 OK`
- `404 NotFound` – brak zakładu
- `401 Unauthorized`

---

## Typy modeli

Poniżej zestawienie modeli request/response na podstawie snapshotów.

### Auth – modele

#### RegisterRequest

```json
{
  "Login": "string",
  "Password": "string",
  "Email": "string"
}
```

#### LoginRequest

```json
{
  "Login": "string",
  "Password": "string"
}
```

#### RefreshRequest

```json
{
  "RefreshToken": "string"
}
```

#### LogoutRequest

```json
{
  "RefreshToken": "string"
}
```

#### AuthResponse

```json
{
  "accessToken": "string",
  "refreshToken": "string",
  "expiresAt": "DateTimeOffset",
  "user": {
    "id": "Guid",
    "login": "string",
    "email": "string",
    "createdAt": "DateTimeOffset",
    "isActive": true
  }
}
```

---

### EventTypes – modele

#### CreateEventTypeRequest

```json
{
  "Code": "string",
  "Name": "string",
  "StartDate": "DateTimeOffset"
}
```

#### UpdateEventTypeRequest

```json
{
  "Name": "string",
  "StartDate": "DateTimeOffset"
}
```

#### EventTypeResponse

```json
{
  "id": "Guid",
  "code": "string",
  "name": "string",
  "startDate": "DateTimeOffset",
  "isActive": true
}
```

#### EventTypesListResponse

```json
{
  "eventTypes": [
    {
      "id": "Guid",
      "code": "string",
      "name": "string",
      "startDate": "DateTimeOffset",
      "isActive": true
    }
  ]
}
```

---

### Tables – modele

#### CreateTableRequest (anonimowy)

```json
{
  "UserLogin": "string",
  "UserPassword": "string",
  "TableName": "string",
  "TablePassword": "string",
  "MaxPlayers": 10,
  "Stake": 50.0,
  "EventTypeId": "Guid"
}
```

#### CreateTableAuthorizedRequest

```json
{
  "TableName": "string",
  "TablePassword": "string",
  "MaxPlayers": 10,
  "Stake": 50.0,
  "EventTypeId": "Guid"
}
```

#### JoinTableRequest (anonimowy)

```json
{
  "UserLogin": "string",
  "UserPassword": "string",
  "TableName": "string",
  "TablePassword": "string"
}
```

#### JoinTableAuthorizedRequest

```json
{
  "Password": "string"
}
```

#### TableResponse

```json
{
  "id": "Guid",
  "name": "string",
  "maxPlayers": 10,
  "stake": 50.0,
  "createdBy": "Guid",
  "createdAt": "DateTimeOffset",
  "isSecretMode": false
}
```

#### JoinTableAuthorizedResponse

```json
{
  "tableId": "Guid",
  "tableName": "string",
  "maxPlayers": 10,
  "stake": 50.0,
  "tableCreatedAt": "DateTimeOffset",
  "userId": "Guid",
  "userLogin": "string",
  "isAdmin": false,
  "joinedAt": "DateTimeOffset",
  "currentMemberCount": 2
}
```

#### TableDetailsResponse

```json
{
  "id": "Guid",
  "name": "string",
  "maxPlayers": 5,
  "stake": 50.0,
  "createdAt": "DateTimeOffset",
  "members": [
    {
      "userId": "Guid",
      "login": "string",
      "isAdmin": true,
      "joinedAt": "DateTimeOffset"
    }
  ]
}
```

#### TableDashboardResponse

```json
{
  "table": {
    "id": "Guid",
    "name": "string",
    "maxPlayers": 10,
    "stake": 50.0,
    "createdAt": "DateTimeOffset"
  },
  "members": [
    {
      "userId": "Guid",
      "login": "string",
      "isAdmin": true,
      "joinedAt": "DateTimeOffset"
    }
  ],
  "matches": [
    {
      "id": "Guid",
      "country1": "string",
      "country2": "string",
      "matchDateTime": "DateTimeOffset",
      "result": "string | null",
      "isStarted": false
    }
  ],
  "bets": [
    {
      "id": "Guid",
      "userId": "Guid",
      "matchId": "Guid",
      "prediction": "string",
      "editedAt": "DateTimeOffset"
    }
  ],
  "pools": [
    {
      "id": "Guid",
      "matchId": "Guid",
      "amount": 100.0,
      "status": "string",
      "winners": ["Guid"]
    }
  ],
  "stats": [
    {
      "userId": "Guid",
      "matchesPlayed": 5,
      "betsPlaced": 10,
      "poolsWon": 2,
      "totalWon": 150.0
    }
  ],
  "leaderboard": [
    {
      "position": 1,
      "userId": "Guid",
      "login": "string",
      "points": 7,
      "exactHits": 2,
      "winnerHits": 1,
      "totalBets": 5,
      "accuracy": 60.0
    }
  ]
}
```

#### LeaderboardEntry

| Pole | Typ | Opis |
|------|-----|------|
| `position` | int | Pozycja w rankingu (1-based) |
| `userId` | Guid | Identyfikator użytkownika |
| `login` | string | Login użytkownika |
| `points` | int | Suma punktów (3×exactHits + 1×winnerHits) |
| `exactHits` | int | Liczba dokładnych trafień wyniku |
| `winnerHits` | int | Liczba trafionych zwycięzców/remisów |
| `totalBets` | int | Liczba zakładów na zakończone mecze |
| `accuracy` | double | Procent trafności (0-100) |

#### GrantAdminRequest

```json
{
  "UserId": "Guid"
}
```

---

### Matches – modele

#### CreateMatchRequest

```json
{
  "Country1": "string",
  "Country2": "string",
  "MatchDateTime": "DateTimeOffset"
}
```

#### MatchResponse (z tworzenia przez admin)

```json
{
  "id": "Guid",
  "eventTypeId": "Guid",
  "country1": "string",
  "country2": "string",
  "matchDateTime": "DateTimeOffset",
  "status": "scheduled",
  "createdAt": "DateTimeOffset"
}
```

#### MatchResponse (z pobierania przez członka stołu)

```json
{
  "id": "Guid",
  "eventTypeId": "Guid",
  "country1": "string",
  "country2": "string",
  "matchDateTime": "DateTimeOffset",
  "result": null,
  "status": "scheduled",
  "started": false,
  "createdAt": "DateTimeOffset"
}
```

> Pole `started` jest dynamiczne: `false` dla przyszłych meczów, `true` po przekroczeniu czasu meczu.

#### MatchDashboardItem

```json
{
  "id": "Guid",
  "country1": "string",
  "country2": "string",
  "matchDateTime": "DateTimeOffset",
  "result": null,
  "isStarted": false
}
```

---

### Bets – modele

#### PlaceBetRequest

```json
{
  "Prediction": "string"
}
```

Dozwolone wartości:

- `"X"` – remis
- `"{gole1}:{gole2}"` np. `"1:0"`, `"3:2"`

#### BetResponse

```json
{
  "id": "Guid",
  "userId": "Guid",
  "matchId": "Guid",
  "prediction": "string",
  "editedAt": "DateTimeOffset"
}
```

---

## Błędy i kody statusu

### Walidacja 400 – RFC 9110

Część błędów walidacyjnych ma standardowy format z dokumentu RFC 9110:

#### Struktura

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "FieldName": [
      "Komunikat błędu"
    ]
  },
  "traceId": "{Scrubbed}"
}
```

Przykłady:

- Rejestracja – błędny email (`FieldName: "Email"`)
- Rejestracja – słabe hasło (`FieldName: "Password"`)
- Tworzenie stołu – pusta nazwa (`FieldName: "TableName"`)
- Tworzenie stołu – ujemna stawka (`FieldName: "Stake"`)
- Dołączanie do stołu – brak hasła (`FieldName: "Password"`)
- Typowanie – zły format (`FieldName: "Prediction"`)

---

### Błędy domenowe (tablice `errors`)

Część błędów domenowych ma strukturę:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "Some.Code",
      "description": "Opis błędu"
    }
  ]
}
```

Zaobserwowane kody:

| Kod błędu | Opis |
|-----------|------|
| `User.AlreadyExists` | Próba rejestracji użytkownika o istniejącym loginie |
| `Auth.InvalidCredentials` | Błędny login lub hasło przy logowaniu |
| `Auth.RefreshNotFound` | Refresh token nie został znaleziony |
| `RefreshToken.NotFound` | Refresh token nie został znaleziony (przy logout) |
| `Table.Full` | Próba dołączenia do pełnego stołu |
| `Table.InvalidPassword` | Nieprawidłowe hasło do stołu |
| `Table.AlreadyMember` | Użytkownik jest już członkiem stołu |
| `Table.AccessDenied` | Użytkownik nie ma dostępu do stołu |
| `Table.DuplicateName` | Stół o podanej nazwie już istnieje |
| `Table.NotAdmin` | Brak uprawnień administratora stołu |
| `EventType.CodeExists` | Typ wydarzenia o podanym kodzie już istnieje |

---

### 401 Unauthorized

Używany, gdy brak lub niepoprawny token autoryzacyjny:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "{Scrubbed}"
}
```

Nagłówki mogą zawierać: `WWW-Authenticate: Bearer`

---

### 403 Forbidden

Używany, gdy użytkownik nie ma odpowiednich uprawnień:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.4",
  "title": "Forbidden",
  "status": 403,
  "traceId": "{Scrubbed}"
}
```

Lub z błędem domenowym:

```json
{
  "errors": [
    {
      "code": "Table.NotAdmin",
      "description": "Nie masz uprawnień do wykonania tej czynności"
    }
  ]
}
```

```json
{
  "errors": [
    {
      "code": "Table.AccessDenied",
      "description": "Nie masz dostępu do tego stołu"
    }
  ]
}
```

---

### 404 Not Found

Standardowy format dla brakujących zasobów:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

Przykłady:

- `GET /bets/my` – brak zakładu dla użytkownika
- `DELETE /bets` – próba usunięcia nieistniejącego zakładu
- `POST /tables/{tableId}/join` – stół nie istnieje
- `GET /tables/{tableId}/dashboard` – stół nie istnieje

---

### 409 Conflict

Używany przy konfliktach stanu:

```json
{
  "code": "Table.DuplicateName",
  "message": "Stół o podanej nazwie już istnieje"
}
```

```json
{
  "errors": [
    {
      "code": "Table.AlreadyMember",
      "description": "Użytkownik jest już członkiem tego stołu"
    }
  ]
}
```

```json
{
  "code": "EventType.CodeExists",
  "message": "Typ wydarzenia o podanym kodzie już istnieje"
}
```

---

### Zestawienie kodów statusu

| Kod | Znaczenie                                              | Przykładowe endpointy                                |
|-----|--------------------------------------------------------|------------------------------------------------------|
| 200 | OK                                                     | `GET /api/tables`, `GET /matches/{id}`, `logout`, `login` |
| 201 | Created                                                | `POST /api/tables`, `POST /api/admin/event-types`, `POST /bets` |
| 400 | Bad Request – walidacja lub błędy domenowe             | rejestracja, tworzenie stołu, place bet, join |
| 401 | Unauthorized – brak/niepoprawny token                  | wszystkie endpointy wymagające Bearer token |
| 403 | Forbidden – brak uprawnień                             | admin operations, dashboard bez członkostwa |
| 404 | Not Found – zasób nie istnieje                         | brak zakładu, brak stołu, brak meczu |
| 409 | Conflict – konflikt stanu                              | duplikat nazwy stołu, już członek stołu |

---

> Wszystkie przykłady danych (`Guid_X`, `DateTimeOffset_X`, `{Scrubbed}`) są zanonimizowane zgodnie z konfiguracją snapshotów i nie reprezentują rzeczywistych wartości w systemie produkcyjnym.
