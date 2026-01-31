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
2. [TablesController](#tablescontroller)
    - [POST /api/tables](#post-apitables)
    - [POST /api/tables/join](#post-apitablesjoin)
    - [POST /api/tables/create](#post-apitablescreate)
    - [GET /api/tables](#get-apitables)
    - [GET /api/tables/{tableId}](#get-apitablestableid)
    - [DELETE /api/tables/{tableId}/members](#delete-apitablestableidmembers)
    - [POST /api/tables/{tableId}/admins](#post-apitablestableidadmins)
    - [DELETE /api/tables/{tableId}/admins/{userId}](#delete-apitablestableidadminsuserid)
3. [MatchesController](#matchescontroller)
    - [POST /api/tables/{tableId}/matches](#post-apitablestableidmatches)
    - [GET /api/tables/{tableId}/matches/{matchId}](#get-apitablestableidmatchesmatchid)
    - [PUT /api/tables/{tableId}/matches/{matchId}](#put-apitablestableidmatchesmatchid)
    - [DELETE /api/tables/{tableId}/matches/{matchId}](#delete-apitablestableidmatchesmatchid)
    - [PUT /api/tables/{tableId}/matches/{matchId}/result](#put-apitablestableidmatchesmatchidresult)
4. [BetsController](#betscontroller)
    - [POST /api/tables/{tableId}/matches/{matchId}/bets](#post-apitablestableidmatchesmatchidbets)
    - [GET /api/tables/{tableId}/matches/{matchId}/bets/my](#get-apitablestableidmatchesmatchidbetsmy)
    - [DELETE /api/tables/{tableId}/matches/{matchId}/bets](#delete-apitablestableidmatchesmatchidbets)
5. [Typy modeli](#typy-modeli)
    - [Auth](#auth-modele)
    - [Tables](#tables-modele)
    - [Matches](#matches-modele)
    - [Bets](#bets-modele)
6. [Błędy i kody statusu](#błędy-i-kody-statusu)
    - [Walidacja 400 – RFC 9110](#walidacja-400--rfc-9110)
    - [Błędy domenowe (tablice `errors`)](#błędy-domenowe-tablice-errors)
    - [404 Not Found](#404-not-found)
    - [403 Forbidden](#403-forbidden)
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

#### Odpowiedź 400 BadRequest – błędne dane logowania (zły login lub hasło, użytkownik nie istnieje)

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

> Uwaga: w snapshotach widoczny jest przypadek, w którym wylogowanie z nieprawidłowym refresh tokenem również zwraca `200 OK` z tą samą wiadomością. Możliwe, że API celowo nie rozróżnia tego przypadku z powodów bezpieczeństwa.

#### Kody statusu

- `200 OK` – wylogowanie (również przy potencjalnie nieistniejącym refresh tokenie)

---

## TablesController

### POST /api/tables

Tworzenie nowego stołu do typowania meczów. Endpoint równocześnie rejestruje użytkownika–twórcę stołu.

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
  "Stake": 50.0
}
```

Walidacje zaobserwowane:

- `TableName`:
    - nie może być pusty (`""`)
- `Stake`:
    - nie może być ujemny

#### Przykładowe żądanie – poprawne

```http
POST /api/tables HTTP/1.1
Content-Type: application/json

{
  "UserLogin": "creator_user_1234abcd",
  "UserPassword": "Creator@12345",
  "TableName": "Test Betting Table 4f6e...",
  "TablePassword": "TablePass@123",
  "MaxPlayers": 10,
  "Stake": 50.0
}
```

#### Odpowiedź 201 Created

```json
{
  "id": "Guid_2",
  "name": "Test Betting Table Guid_1",
  "maxPlayers": 10,
  "stake": 50.0,
  "createdBy": "Guid_3",
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

### POST /api/tables/join

Dołączenie do istniejącego stołu.

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

> Uwaga: w jednym snapshotcie błędne hasło do stołu zwraca `200 OK` z odpowiedzią stołu – wygląda na to, że test lub implementacja może być niespójna. W poprawnej implementacji należałoby oczekiwać `400` z błędem domenowym.

#### Kody statusu

- `200 OK` – dołączenie (również w przypadkach testowo niespójnych)
- `400 BadRequest` – stół pełny lub inne błędy domenowe

---

### POST /api/tables/create

Tworzenie nowego stołu przez zalogowanego użytkownika.

- Metoda: `POST`
- Ścieżka: `/api/tables/create`
- Autoryzacja: wymagana (Bearer JWT)

#### Headers

```
Authorization: Bearer {token}
Content-Type: application/json
```

#### Body (request)

```json
{
  "TableName": "string",
  "TablePassword": "string",
  "MaxPlayers": 10,
  "Stake": 50.0
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
  "Stake": 25.0
}
```

#### Odpowiedź 201 Created

```json
{
  "id": "Guid_2",
  "name": "Authorized Table Guid_1",
  "maxPlayers": 10,
  "stake": 25.0,
  "createdBy": "Guid_3",
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

#### Odpowiedź 400 BadRequest – błędy walidacyjne

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "TableName": "{Scrubbed}",
    "TablePassword": "{Scrubbed}",
    "MaxPlayers": "{Scrubbed}",
    "Stake": "{Scrubbed}"
  },
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

- `201 Created` – stół utworzony
- `401 Unauthorized` – brak/niepoprawny token
- `409 Conflict` – duplikat nazwy stołu
- `400 BadRequest` – błędy walidacyjne

---

### GET /api/tables

Pobranie listy stołów, których użytkownik jest członkiem.

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables`
- **Autoryzacja:** wymagana (`Bearer` w nagłówku)

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
- `401 Unauthorized` – jeśli brak/niepoprawny token (wnioskowane z konwencji, brak w snapshotach)

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
- `404 NotFound` – stół nie istnieje (wnioskowane, brak w snapshotach)

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
- `404 NotFound` – użytkownik nie jest członkiem stołu lub stół nie istnieje (wnioskowane)

---

### POST /api/tables/{tableId}/admins

Nadanie roli administratora innemu użytkownikowi stołu. Wymagane uprawnienia twórcy stołu lub administratora.

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
- `403 Forbidden` – użytkownik nie jest uprawniony do zarządzania adminami (wnioskowane)
- `404 NotFound` – stół lub użytkownik nie istnieje (wnioskowane)

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

### POST /api/tables/{tableId}/matches

Tworzenie meczu w ramach stołu. Dostępne tylko dla administratorów stołu.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/{tableId}/matches`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId` – identyfikator stołu

#### Body (request)

```json
{
  "Country1": "string",
  "Country2": "string",
  "MatchDateTime": "DateTimeOffset"
}
```

Walidacje:

- `Country1` i `Country2` muszą być różne
- `MatchDateTime` musi być w przyszłości

#### Przykładowe żądanie – poprawne

```http
POST /api/tables/Guid_1/matches HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Country1": "Poland",
  "Country2": "Germany",
  "MatchDateTime": "2026-05-01T18:00:00Z"
}
```

#### Odpowiedź 201 Created

```json
{
  "id": "Guid_3",
  "tableId": "Guid_1",
  "country1": "Poland",
  "country2": "Germany",
  "matchDateTime": "DateTimeOffset_4",
  "result": null,
  "status": "scheduled",
  "started": false,
  "createdAt": "DateTimeOffset_5"
}
```

#### Odpowiedź 400 BadRequest – te same drużyny

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "": [
      "Drużyny muszą być różne"
    ]
  },
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 400 BadRequest – data w przeszłości

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "MatchDateTime": [
      "Data meczu musi być w przyszłości"
    ]
  },
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 403 Forbidden – brak roli admina

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

> Odpowiedź 403 nie zawiera pól `type/title/status` – to błąd domenowy.

#### Kody statusu

- `201 Created` – mecz utworzony
- `400 BadRequest` – błędy walidacyjne
- `403 Forbidden` – użytkownik nie jest adminem stołu
- `401 Unauthorized` – brak tokena (wnioskowane)

---

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
GET /api/tables/Guid_1/matches/Guid_3 HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

Przyszły mecz (`started: false`):

```json
{
  "id": "Guid_3",
  "tableId": "Guid_1",
  "country1": "Poland",
  "country2": "Germany",
  "matchDateTime": "DateTimeOffset_4",
  "result": null,
  "status": "scheduled",
  "started": false,
  "createdAt": "DateTimeOffset_5"
}
```

Mecz po czasie (`started: true`):

```json
{
  "id": "Guid_3",
  "tableId": "Guid_1",
  "country1": "France",
  "country2": "Italy",
  "matchDateTime": "DateTimeOffset_4",
  "result": null,
  "status": "scheduled",
  "started": true,
  "createdAt": "DateTimeOffset_5"
}
```

#### Kody statusu

- `200 OK`
- `401 Unauthorized`
- `404 NotFound` – mecz nie istnieje (wnioskowane)

---

### PUT /api/tables/{tableId}/matches/{matchId}

Aktualizacja danych meczu przed jego rozpoczęciem.

- **Metoda:** `PUT`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId`
- `matchId`

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
PUT /api/tables/Guid_1/matches/Guid_3 HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Country1": "Netherlands",
  "Country2": "Austria",
  "MatchDateTime": "2026-06-01T18:00:00Z"
}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Match updated successfully"
}
```

#### Kody statusu

- `200 OK` – mecz zaktualizowany
- `400 BadRequest` – naruszenie walidacji (wnioskowane)
- `403 Forbidden` – brak uprawnień admina (wnioskowane)
- `404 NotFound` – mecz/ stół nie istnieje (wnioskowane)

---

### DELETE /api/tables/{tableId}/matches/{matchId}

Usunięcie meczu przed jego rozpoczęciem.

- **Metoda:** `DELETE`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId`
- `matchId`

#### Przykładowe żądanie

```http
DELETE /api/tables/Guid_1/matches/Guid_3 HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Match deleted successfully"
}
```

#### Kody statusu

- `200 OK`
- `403 Forbidden` – brak uprawnień (wnioskowane)
- `404 NotFound`

---

### PUT /api/tables/{tableId}/matches/{matchId}/result

Ustawienie wyniku meczu. Dostępne tylko dla administratorów stołu.

- **Metoda:** `PUT`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}/result`
- **Autoryzacja:** wymagana (`Bearer`)

#### Parametry ścieżki

- `tableId`
- `matchId`

#### Body (request)

```json
{
  "Score1": "string",
  "Score2": "string"
}
```

> Z snapshotów wynika, że wyniki są przekazywane jako stringi liczbowe (`"2"`, `"1"`).

#### Przykładowe żądanie

```http
PUT /api/tables/Guid_1/matches/Guid_3/result HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Score1": "2",
  "Score2": "1"
}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Match result set successfully"
}
```

#### Kody statusu

- `200 OK`
- `400 BadRequest` – błędy walidacji wyniku (wnioskowane)
- `403 Forbidden` – brak roli admina (wnioskowane)
- `404 NotFound` – mecz/ stół nie istnieje (wnioskowane)

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

Kolejne wywołanie zastępuje typ, ale zwraca ten sam `id`:

```json
{
  "id": "Guid_6",
  "userId": "Guid_4",
  "matchId": "Guid_5",
  "prediction": "2:1",
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
- `401 Unauthorized` – brak tokena (wnioskowane)
- `404 NotFound` – mecz/ stół nie istnieje (wnioskowane)

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

### Tables – modele

#### CreateTableRequest

```json
{
  "UserLogin": "string",
  "UserPassword": "string",
  "TableName": "string",
  "TablePassword": "string",
  "MaxPlayers": 10,
  "Stake": 50.0
}
```

#### JoinTableRequest

```json
{
  "UserLogin": "string",
  "UserPassword": "string",
  "TableName": "string",
  "TablePassword": "string"
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

#### TableDetailsResponse (wnioskowany)

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

#### UpdateMatchRequest

```json
{
  "Country1": "string",
  "Country2": "string",
  "MatchDateTime": "DateTimeOffset"
}
```

#### MatchResponse

```json
{
  "id": "Guid",
  "tableId": "Guid",
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

#### SetMatchResultRequest

```json
{
  "Score1": "string",
  "Score2": "string"
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
- Tworzenie meczu – data w przeszłości (`FieldName: "MatchDateTime"`)
- Typowanie – zły format (`FieldName: "Prediction"`)

Dodatkowo w jednym przypadku:

```json
"errors": {
  "": [
    "Drużyny muszą być różne"
  ]
}
```

– błąd przypisany do całego modelu (brak konkretnego pola).

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

- `User.AlreadyExists` – próba rejestracji użytkownika o istniejącym loginie
- `Auth.InvalidCredentials` – błędny login lub hasło przy logowaniu
- `Auth.RefreshNotFound` – refresh token nie został znaleziony
- `Table.Full` – próba dołączenia do pełnego stołu

Błąd 403 `Table.NotAdmin`:

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

– brak standardowych pól walidacyjnych (`type/title/status`).

---

### 404 Not Found

Standardowy format dla brakujących zasobów (zakład, itp.):

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

---

### 403 Forbidden

Używany, gdy użytkownik nie ma odpowiednich uprawnień:

- Tworzenie meczu przez użytkownika niebędącego adminem stołu:

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

---

### Zestawienie kodów statusu

| Kod | Znaczenie                                              | Przykładowe endpointy                                |
|-----|--------------------------------------------------------|------------------------------------------------------|
| 200 | OK                                                     | `GET /api/tables`, `GET /matches/{id}`, `logout`    |
| 201 | Created                                                | `POST /api/tables`, `POST /matches`, `POST /bets`   |
| 400 | Bad Request – walidacja lub błędy domenowe             | rejestracja, tworzenie stołu/meczu, place bet       |
| 403 | Forbidden – brak uprawnień                             | tworzenie meczu bez roli admina                     |
| 404 | Not Found – zasób lub stan nie istnieje                | brak zakładu, brak meczu/stołu (wnioskowane)        |
| 401 | Unauthorized – brak/niepoprawny token (wnioskowane)    | wszystkie endpointy wymagające Bearer token          |

---

> Wszystkie przykłady danych (`Guid_X`, `DateTimeOffset_X`, `{Scrubbed}`) są zanonimizowane zgodnie z konfiguracją snapshotów i nie reprezentują rzeczywistych wartości w systemie produkcyjnym.