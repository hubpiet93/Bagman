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
    - [GET /api/admin/event-types](#get-apiadminevent-types)
    - [POST /api/admin/event-types](#post-apiadminevent-types)
    - [PUT /api/admin/event-types/{id}](#put-apiadminevent-typesid)
    - [POST /api/admin/event-types/{id}/deactivate](#post-apiadminevent-typesiddeactivate)
3. [AdminMatchesController](#adminmatchescontroller)
    - [GET /api/admin/event-types/{eventTypeId}/matches](#get-apiadminevent-typeseventtypeidmatches)
    - [POST /api/admin/event-types/{eventTypeId}/matches](#post-apiadminevent-typeseventtypeidmatches)
    - [PUT /api/admin/event-types/{eventTypeId}/matches/{matchId}](#put-apiadminevent-typeseventtypeidmatchesmatchid)
    - [DELETE /api/admin/event-types/{eventTypeId}/matches/{matchId}](#delete-apiadminevent-typeseventtypeidmatchesmatchid)
    - [PUT /api/admin/matches/{matchId}/result](#put-apiadminmatchesmatchidresult)
4. [TablesController](#tablescontroller)
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
    - [PATCH /api/tables/{tableId}](#patch-apitablestableid)
5. [MatchesController](#matchescontroller)
    - [GET /api/tables/{tableId}/matches/{matchId}](#get-apitablestableidmatchesmatchid)
6. [BetsController](#betscontroller)
    - [POST /api/tables/{tableId}/matches/{matchId}/bets](#post-apitablestableidmatchesmatchidbets)
    - [GET /api/tables/{tableId}/matches/{matchId}/bets/my](#get-apitablestableidmatchesmatchidbetsmy)
    - [DELETE /api/tables/{tableId}/matches/{matchId}/bets](#delete-apitablestableidmatchesmatchidbets)
7. [Typy modeli](#typy-modeli)
    - [Auth](#auth--modele)
    - [EventTypes](#eventtypes--modele)
    - [AdminMatches](#adminmatches--modele)
    - [Tables](#tables--modele)
    - [Matches](#matches--modele)
    - [Bets](#bets--modele)
8. [Błędy i kody statusu](#błędy-i-kody-statusu)
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
    "isActive": true,
    "isSuperAdmin": false
  }
}
```

#### Odpowiedź 400 – nieprawidłowy email

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

#### Odpowiedź 400 – słabe hasło

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

#### Odpowiedź 400 – login już istnieje

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

| Kod | Opis |
|-----|------|
| `200 OK` | Rejestracja zakończona sukcesem |
| `400 Bad Request` | Błąd walidacji lub login już zajęty |

---

### POST /api/auth/login

Logowanie użytkownika i zwrócenie tokenów uwierzytelniających.

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
  "Login": "testuser",
  "Password": "Test@12345"
}
```

#### Odpowiedź 200 OK

```json
{
  "accessToken": "{Scrubbed}",
  "refreshToken": "{Scrubbed}",
  "expiresAt": "DateTimeOffset_3",
  "user": {
    "id": "Guid_1",
    "login": "{Scrubbed}",
    "email": "{Scrubbed}",
    "createdAt": "DateTimeOffset_4",
    "isActive": true,
    "isSuperAdmin": false
  }
}
```

#### Odpowiedź 400 – nieprawidłowe dane logowania

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

> Ta sama odpowiedź jest zwracana zarówno gdy hasło jest nieprawidłowe, jak i gdy użytkownik nie istnieje (ze względów bezpieczeństwa).

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Logowanie zakończone sukcesem |
| `400 Bad Request` | Nieprawidłowy login lub hasło |

---

### POST /api/auth/refresh

Odświeżenie tokenów dostępu przy użyciu ważnego refresh tokena.

- **Metoda:** `POST`
- **Ścieżka:** `/api/auth/refresh`
- **Autoryzacja:** niewymagana

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
  "expiresAt": "DateTimeOffset_3",
  "user": {
    "id": "Guid_1",
    "login": "{Scrubbed}",
    "email": "{Scrubbed}",
    "createdAt": "DateTimeOffset_4",
    "isActive": true,
    "isSuperAdmin": false
  }
}
```

#### Odpowiedź 400 – nieważny refresh token

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

| Kod | Opis |
|-----|------|
| `200 OK` | Tokeny odświeżone pomyślnie |
| `400 Bad Request` | Refresh token nieważny lub nieznany |

---

### POST /api/auth/logout

Wylogowanie użytkownika – unieważnienie refresh tokena.

- **Metoda:** `POST`
- **Ścieżka:** `/api/auth/logout`
- **Autoryzacja:** niewymagana

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

#### Odpowiedź 400 – nieważny refresh token

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

| Kod | Opis |
|-----|------|
| `200 OK` | Wylogowanie zakończone sukcesem |
| `400 Bad Request` | Refresh token nieznany |

---

## EventTypesController

### GET /api/event-types

Pobiera listę aktywnych typów wydarzeń. Endpoint publiczny – nie wymaga autoryzacji.

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

| Kod | Opis |
|-----|------|
| `200 OK` | Lista aktywnych typów wydarzeń |

---

### GET /api/admin/event-types

[SuperAdminOnly] Pobiera listę wszystkich typów wydarzeń, włącznie z nieaktywnymi. Wymaga zalogowania jako SuperAdmin.

- **Metoda:** `GET`
- **Ścieżka:** `/api/admin/event-types`
- **Autoryzacja:** Bearer token (SuperAdmin)

#### Przykładowe żądanie

```http
GET /api/admin/event-types HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "eventTypes": [
    {
      "id": "Guid_3",
      "code": "TEST_DEFAULT",
      "name": "Default Test Event",
      "startDate": "DateTimeOffset_6",
      "isActive": false
    },
    {
      "id": "Guid_2",
      "code": "INACTIVE_FOR_ALL_TEST",
      "name": "Inactive Event For All Test",
      "startDate": "DateTimeOffset_7",
      "isActive": true
    }
  ]
}
```

#### Odpowiedź 401 – brak tokenu

```json
null
```
*(pusta odpowiedź z nagłówkiem `WWW-Authenticate: Bearer`)*

#### Odpowiedź 403 – zwykły użytkownik

```json
null
```
*(pusta odpowiedź)*

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Lista wszystkich typów wydarzeń (w tym nieaktywnych) |
| `401 Unauthorized` | Brak tokenu |
| `403 Forbidden` | Użytkownik nie jest SuperAdminem |

---

### POST /api/admin/event-types

[SuperAdminOnly] Tworzy nowy typ wydarzenia.

- **Metoda:** `POST`
- **Ścieżka:** `/api/admin/event-types`
- **Autoryzacja:** Bearer token (SuperAdmin)

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
  "StartDate": "DateTimeOffset_5"
}
```

#### Odpowiedź 201 Created

```json
{
  "id": "Guid_2",
  "code": "LIGA_MISTRZOW_2026",
  "name": "Liga Mistrzów 2025/2026",
  "startDate": "DateTimeOffset_5",
  "isActive": true
}
```

Nagłówek `Location`: `http://localhost/api/event-types?id=Guid_2`

#### Odpowiedź 403 – zwykły użytkownik

```json
null
```
*(pusta odpowiedź)*

#### Odpowiedź 409 – kod już istnieje

```json
{
  "errors": [
    {
      "code": "EventType.CodeExists",
      "description": "Kod wydarzenia już istnieje"
    }
  ]
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `201 Created` | Typ wydarzenia utworzony |
| `403 Forbidden` | Użytkownik nie jest SuperAdminem |
| `409 Conflict` | Kod już istnieje |

---

### PUT /api/admin/event-types/{id}

[SuperAdminOnly] Aktualizuje istniejący typ wydarzenia.

- **Metoda:** `PUT`
- **Ścieżka:** `/api/admin/event-types/{id}`
- **Autoryzacja:** Bearer token (SuperAdmin)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `id` | GUID | Identyfikator typu wydarzenia |

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
  "StartDate": "DateTimeOffset_5"
}
```

#### Odpowiedź 200 OK

```json
{
  "id": "Guid_2",
  "code": "TEST_DEFAULT",
  "name": "Updated Test Event",
  "startDate": "DateTimeOffset_5",
  "isActive": true
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Typ wydarzenia zaktualizowany |
| `403 Forbidden` | Użytkownik nie jest SuperAdminem |

---

### POST /api/admin/event-types/{id}/deactivate

[SuperAdminOnly] Dezaktywuje typ wydarzenia.

- **Metoda:** `POST`
- **Ścieżka:** `/api/admin/event-types/{id}/deactivate`
- **Autoryzacja:** Bearer token (SuperAdmin)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `id` | GUID | Identyfikator typu wydarzenia |

#### Przykładowe żądanie

```http
POST /api/admin/event-types/Guid_2/deactivate HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Typ wydarzenia został dezaktywowany"
}
```

#### Odpowiedź 403 – zwykły użytkownik

```json
null
```
*(pusta odpowiedź)*

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Typ wydarzenia dezaktywowany |
| `403 Forbidden` | Użytkownik nie jest SuperAdminem |

---

## AdminMatchesController

### GET /api/admin/event-types/{eventTypeId}/matches

[SuperAdminOnly] Pobiera listę meczów dla danego typu wydarzenia. Odpowiedź zawiera pole `result` (wynik meczu lub `null`).

- **Metoda:** `GET`
- **Ścieżka:** `/api/admin/event-types/{eventTypeId}/matches`
- **Autoryzacja:** Bearer token (SuperAdmin)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `eventTypeId` | GUID | Identyfikator typu wydarzenia |

#### Przykładowe żądanie

```http
GET /api/admin/event-types/Guid_2/matches HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK – mecz bez wyniku

```json
[
  {
    "id": "Guid_3",
    "eventTypeId": "Guid_2",
    "country1": "Spain",
    "country2": "Portugal",
    "matchDateTime": "DateTimeOffset_7",
    "result": null,
    "status": "scheduled",
    "started": false,
    "createdAt": "DateTimeOffset_8"
  }
]
```

#### Odpowiedź 200 OK – mecz z wynikiem (finished)

```json
[
  {
    "id": "Guid_3",
    "eventTypeId": "Guid_2",
    "country1": "Brazil",
    "country2": "Argentina",
    "matchDateTime": "DateTimeOffset_5",
    "result": "3:0",
    "status": "finished",
    "started": true,
    "createdAt": "DateTimeOffset_6"
  }
]
```

#### Odpowiedź 200 OK – mieszana lista (z wynikiem i bez)

```json
[
  {
    "id": "Guid_4",
    "eventTypeId": "Guid_2",
    "country1": "France",
    "country2": "Germany",
    "matchDateTime": "DateTimeOffset_6",
    "result": "2:1",
    "status": "finished",
    "started": true,
    "createdAt": "DateTimeOffset_7"
  },
  {
    "id": "Guid_3",
    "eventTypeId": "Guid_2",
    "country1": "Brazil",
    "country2": "Argentina",
    "matchDateTime": "DateTimeOffset_8",
    "result": null,
    "status": "scheduled",
    "started": false,
    "createdAt": "DateTimeOffset_9"
  }
]
```

#### Odpowiedź 403 – zwykły użytkownik

```json
null
```
*(pusta odpowiedź)*

#### Odpowiedź 404 – typ wydarzenia nie istnieje

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Lista meczów (może być pusta) |
| `403 Forbidden` | Użytkownik nie jest SuperAdminem |
| `404 Not Found` | Typ wydarzenia nie istnieje |

---

### POST /api/admin/event-types/{eventTypeId}/matches

[SuperAdminOnly] Tworzy nowy mecz w ramach danego typu wydarzenia.

- **Metoda:** `POST`
- **Ścieżka:** `/api/admin/event-types/{eventTypeId}/matches`
- **Autoryzacja:** Bearer token (SuperAdmin)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `eventTypeId` | GUID | Identyfikator typu wydarzenia |

#### Body (request)

```json
{
  "Country1": "string",
  "Country2": "string",
  "MatchDateTime": "DateTimeOffset"
}
```

Walidacje obserwowane w snapshotach:
- `MatchDateTime`: musi być w przyszłości (`"Data meczu musi być w przyszłości"`)

#### Przykładowe żądanie

```http
POST /api/admin/event-types/Guid_2/matches HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Country1": "France",
  "Country2": "Germany",
  "MatchDateTime": "DateTimeOffset_5"
}
```

#### Odpowiedź 201 Created

```json
{
  "id": "Guid_3",
  "eventTypeId": "Guid_2",
  "country1": "France",
  "country2": "Germany",
  "matchDateTime": "DateTimeOffset_5",
  "status": "scheduled",
  "createdAt": "DateTimeOffset_6"
}
```

Nagłówek `Location`: `/api/admin/event-types/Guid_2/matches/Guid_3`

#### Odpowiedź 400 – data meczu w przeszłości

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

#### Odpowiedź 403 – zwykły użytkownik

```json
null
```
*(pusta odpowiedź)*

#### Kody statusu

| Kod | Opis |
|-----|------|
| `201 Created` | Mecz utworzony |
| `400 Bad Request` | Błąd walidacji (np. data w przeszłości) |
| `403 Forbidden` | Użytkownik nie jest SuperAdminem |

---

### PUT /api/admin/event-types/{eventTypeId}/matches/{matchId}

[SuperAdminOnly] Aktualizuje dane meczu. Nie można edytować meczu, który już się rozpoczął.

- **Metoda:** `PUT`
- **Ścieżka:** `/api/admin/event-types/{eventTypeId}/matches/{matchId}`
- **Autoryzacja:** Bearer token (SuperAdmin)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `eventTypeId` | GUID | Identyfikator typu wydarzenia |
| `matchId` | GUID | Identyfikator meczu |

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
PUT /api/admin/event-types/Guid_2/matches/Guid_3 HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Country1": "Poland",
  "Country2": "Czech Republic",
  "MatchDateTime": "DateTimeOffset_6"
}
```

#### Odpowiedź 200 OK

```json
{
  "id": "Guid_3",
  "eventTypeId": "Guid_2",
  "country1": "Poland",
  "country2": "Czech Republic",
  "matchDateTime": "DateTimeOffset_6",
  "result": null,
  "status": "scheduled",
  "started": false,
  "createdAt": "DateTimeOffset_7"
}
```

#### Odpowiedź 400 – mecz już się rozpoczął

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "Match.AlreadyStarted",
      "description": "Nie można edytować meczu, który już się rozpoczął"
    }
  ]
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Mecz zaktualizowany |
| `400 Bad Request` | Mecz już się rozpoczął |
| `403 Forbidden` | Użytkownik nie jest SuperAdminem |

---

### DELETE /api/admin/event-types/{eventTypeId}/matches/{matchId}

[SuperAdminOnly] Usuwa mecz. Nie można usunąć meczu, który już się rozpoczął.

- **Metoda:** `DELETE`
- **Ścieżka:** `/api/admin/event-types/{eventTypeId}/matches/{matchId}`
- **Autoryzacja:** Bearer token (SuperAdmin)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `eventTypeId` | GUID | Identyfikator typu wydarzenia |
| `matchId` | GUID | Identyfikator meczu |

#### Przykładowe żądanie

```http
DELETE /api/admin/event-types/Guid_2/matches/Guid_3 HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Mecz został usunięty"
}
```

#### Odpowiedź 400 – mecz już się rozpoczął

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "Match.AlreadyStarted",
      "description": "Nie można usunąć meczu, który już się rozpoczął"
    }
  ]
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Mecz usunięty |
| `400 Bad Request` | Mecz już się rozpoczął |
| `403 Forbidden` | Użytkownik nie jest SuperAdminem |

---

### PUT /api/admin/matches/{matchId}/result

[SuperAdminOnly] Ustawia wynik meczu. Mecz musi być już rozpoczęty (data w przeszłości).

- **Metoda:** `PUT`
- **Ścieżka:** `/api/admin/matches/{matchId}/result`
- **Autoryzacja:** Bearer token (SuperAdmin)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `matchId` | GUID | Identyfikator meczu |

#### Body (request)

```json
{
  "Result": "string"
}
```

Format wyniku: `"X:Y"` gdzie X i Y to liczby całkowite (np. `"2:1"`).

#### Przykładowe żądanie

```http
PUT /api/admin/matches/Guid_2/result HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Result": "2:1"
}
```

#### Odpowiedź 200 OK

```json
{
  "id": "Guid_2",
  "result": "2:1",
  "status": "finished"
}
```

#### Odpowiedź 400 – nieprawidłowy format wyniku

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "Score.InvalidFormat",
      "description": "Score must be in format '2:1'"
    }
  ]
}
```

#### Odpowiedź 400 – mecz jeszcze się nie rozpoczął

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "Match.NotStarted",
      "description": "Nie można ustawić wyniku przed rozpoczęciem meczu"
    }
  ]
}
```

#### Odpowiedź 403 – zwykły użytkownik

```json
null
```
*(pusta odpowiedź)*

#### Odpowiedź 404 – mecz nie istnieje

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Wynik ustawiony, mecz ma status `finished` |
| `400 Bad Request` | Nieprawidłowy format lub mecz nie rozpoczęty |
| `403 Forbidden` | Użytkownik nie jest SuperAdminem |
| `404 Not Found` | Mecz nie istnieje |

---

## TablesController

### POST /api/tables

Tworzy nowy stół i automatycznie dołącza użytkownika do niego. To jest **starszy endpoint** (legacy) – przyjmuje dane logowania w ciele żądania. Nowym odpowiednikiem jest `POST /api/tables/create`.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables`
- **Autoryzacja:** niewymagana (dane logowania w body)

#### Body (request)

```json
{
  "UserLogin": "string",
  "UserPassword": "string",
  "TableName": "string",
  "TablePassword": "string",
  "MaxPlayers": "integer",
  "Stake": "decimal",
  "EventTypeId": "GUID"
}
```

Walidacje:
- `TableName`: wymagana (błąd: `"Nazwa stołu jest wymagana"`)
- `Stake`: nie może być ujemna (błąd: `"Stawka nie może być ujemna"`)

#### Przykładowe żądanie

```http
POST /api/tables HTTP/1.1
Content-Type: application/json

{
  "UserLogin": "{Scrubbed}",
  "UserPassword": "Creator@12345",
  "TableName": "Test Betting Table Guid_1",
  "TablePassword": "TablePass@123",
  "MaxPlayers": 10,
  "Stake": 50.0,
  "EventTypeId": "Guid_2"
}
```

#### Odpowiedź 201 Created

```json
{
  "id": "Guid_3",
  "name": "Test Betting Table Guid_1",
  "maxPlayers": 10,
  "stake": 50.0,
  "createdBy": "Guid_4",
  "createdAt": "DateTimeOffset_1",
  "isSecretMode": false
}
```

Nagłówek `Location`: `http://localhost/api/Tables/Guid_3`

#### Odpowiedź 400 – brak nazwy stołu

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

#### Odpowiedź 400 – ujemna stawka

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

| Kod | Opis |
|-----|------|
| `201 Created` | Stół utworzony |
| `400 Bad Request` | Błąd walidacji |

---

### POST /api/tables/create

Tworzy nowy stół dla zalogowanego użytkownika. Nowy endpoint wymagający Bearer tokena.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/create`
- **Autoryzacja:** Bearer token (wymagane)

#### Body (request)

```json
{
  "TableName": "string",
  "TablePassword": "string",
  "MaxPlayers": "integer",
  "Stake": "decimal",
  "EventTypeId": "GUID"
}
```

#### Przykładowe żądanie

```http
POST /api/tables/create HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "TableName": "Authorized Table Guid_2",
  "TablePassword": "AuthTablePass@123",
  "MaxPlayers": 10,
  "Stake": 25.0,
  "EventTypeId": "Guid_3"
}
```

#### Odpowiedź 201 Created

```json
{
  "id": "Guid_4",
  "name": "Authorized Table Guid_2",
  "maxPlayers": 10,
  "stake": 25.0,
  "createdBy": "Guid_1",
  "createdAt": "DateTimeOffset_3",
  "isSecretMode": false
}
```

Nagłówek `Location`: `http://localhost/api/Tables/Guid_4`

#### Odpowiedź 400 – brak nazwy stołu

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

#### Odpowiedź 401 – brak tokenu

```json
null
```
*(pusta odpowiedź z nagłówkiem `WWW-Authenticate: Bearer`)*

#### Odpowiedź 409 – nazwa stołu już istnieje

```json
{
  "code": "Table.DuplicateName",
  "message": "Stół o podanej nazwie już istnieje"
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `201 Created` | Stół utworzony |
| `400 Bad Request` | Błąd walidacji |
| `401 Unauthorized` | Brak tokenu |
| `409 Conflict` | Nazwa stołu już istnieje |

---

### POST /api/tables/join

Dołącza użytkownika do istniejącego stołu. To jest **starszy endpoint** (legacy) – przyjmuje dane logowania w ciele żądania. Nowym odpowiednikiem jest `POST /api/tables/{tableId}/join`.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/join`
- **Autoryzacja:** niewymagana (dane logowania w body)

#### Body (request)

```json
{
  "UserLogin": "string",
  "UserPassword": "string",
  "TableName": "string",
  "TablePassword": "string"
}
```

#### Przykładowe żądanie

```http
POST /api/tables/join HTTP/1.1
Content-Type: application/json

{
  "UserLogin": "{Scrubbed}",
  "UserPassword": "Joiner@12345",
  "TableName": "Joinable Table Guid_1",
  "TablePassword": "TablePass@123"
}
```

#### Odpowiedź 200 OK

```json
{
  "id": "Guid_3",
  "name": "Joinable Table Guid_1",
  "maxPlayers": 10,
  "stake": 50.0,
  "createdBy": "Guid_4",
  "createdAt": "DateTimeOffset_2",
  "isSecretMode": false
}
```

#### Odpowiedź 400 – błędne hasło

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

#### Odpowiedź 400 – stół pełny

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

| Kod | Opis |
|-----|------|
| `200 OK` | Dołączono do stołu |
| `400 Bad Request` | Błędne hasło lub stół pełny |

---

### POST /api/tables/{tableId}/join

Dołącza zalogowanego użytkownika do stołu. Nowy endpoint wymagający Bearer tokena.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/{tableId}/join`
- **Autoryzacja:** Bearer token (wymagane)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |

#### Body (request)

```json
{
  "Password": "string"
}
```

Walidacje:
- `Password`: wymagane (błąd: `"Hasło stołu jest wymagane"`)

#### Przykładowe żądanie

```http
POST /api/tables/Guid_3/join HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Password": "TablePass@123"
}
```

#### Odpowiedź 200 OK

```json
{
  "tableId": "Guid_3",
  "tableName": "Authorized Join Table Guid_1",
  "maxPlayers": 10,
  "stake": 50.0,
  "tableCreatedAt": "DateTimeOffset_4",
  "userId": "Guid_5",
  "userLogin": "{Scrubbed}",
  "isAdmin": false,
  "joinedAt": "DateTimeOffset_5",
  "currentMemberCount": 2
}
```

#### Odpowiedź 400 – puste hasło

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Password": [
      "Hasło stołu jest wymagane"
    ]
  },
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 400 – błędne hasło

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

#### Odpowiedź 400 – stół pełny

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

#### Odpowiedź 401 – brak tokenu

```json
null
```
*(pusta odpowiedź z nagłówkiem `WWW-Authenticate: Bearer`)*

#### Odpowiedź 404 – stół nie istnieje

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Odpowiedź 409 – użytkownik już jest członkiem

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

| Kod | Opis |
|-----|------|
| `200 OK` | Dołączono do stołu |
| `400 Bad Request` | Błąd walidacji, błędne hasło lub stół pełny |
| `401 Unauthorized` | Brak tokenu |
| `404 Not Found` | Stół nie istnieje |
| `409 Conflict` | Użytkownik już jest członkiem stołu |

---

### GET /api/tables

Pobiera listę stołów, do których należy zalogowany użytkownik.

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables`
- **Autoryzacja:** Bearer token (wymagane)

#### Przykładowe żądanie

```http
GET /api/tables HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
[
  {
    "id": "Guid_4",
    "name": "Authorized Table Guid_2",
    "maxPlayers": 10,
    "stake": 25.0,
    "createdBy": "Guid_1",
    "createdAt": "DateTimeOffset_3",
    "isSecretMode": false
  },
  {
    "id": "Guid_6",
    "name": "Authorized Table Guid_5",
    "maxPlayers": 10,
    "stake": 25.0,
    "createdBy": "Guid_1",
    "createdAt": "DateTimeOffset_4",
    "isSecretMode": false
  }
]
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Lista stołów użytkownika |

---

### GET /api/tables/{tableId}

Pobiera szczegóły stołu wraz z listą członków.

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables/{tableId}`
- **Autoryzacja:** Bearer token (wymagane)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |

#### Przykładowe żądanie

```http
GET /api/tables/Guid_4 HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "id": "Guid_4",
  "name": "Details Table Guid_2",
  "maxPlayers": 10,
  "stake": 50.0,
  "createdAt": "DateTimeOffset_4",
  "members": [
    {
      "userId": "Guid_1",
      "login": "{Scrubbed}",
      "isAdmin": true,
      "joinedAt": "DateTimeOffset_5"
    }
  ]
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Szczegóły stołu z listą członków |

---

### GET /api/tables/{tableId}/dashboard

Pobiera dashboard stołu dla zalogowanego użytkownika. Zwraca informacje o stole, członkach, meczach, zakładach, tablicy wyników (leaderboard) i pulach (pools).

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables/{tableId}/dashboard`
- **Autoryzacja:** Bearer token (wymagane, użytkownik musi być członkiem stołu)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |

#### Przykładowe żądanie

```http
GET /api/tables/Guid_5/dashboard HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK – podstawowe dane (bez meczów)

```json
{
  "table": {
    "id": "Guid_5",
    "name": "Dashboard Table Guid_3",
    "maxPlayers": 10,
    "stake": 50.0,
    "createdAt": "DateTimeOffset_6"
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
      "joinedAt": "DateTimeOffset_8"
    }
  ]
}
```

#### Odpowiedź 200 OK – pełne dane (z meczami, zakładami, leaderboardem)

```json
{
  "table": {
    "id": "Guid_6",
    "name": "Full Data Table Guid_4",
    "maxPlayers": 10,
    "stake": 50.0,
    "createdAt": "DateTimeOffset_8"
  },
  "members": [
    {
      "userId": "Guid_1",
      "login": "{Scrubbed}",
      "isAdmin": true,
      "joinedAt": "DateTimeOffset_8"
    },
    {
      "userId": "Guid_2",
      "login": "{Scrubbed}",
      "isAdmin": false,
      "joinedAt": "DateTimeOffset_20"
    },
    {
      "userId": "Guid_3",
      "login": "{Scrubbed}",
      "isAdmin": false,
      "joinedAt": "DateTimeOffset_21"
    }
  ],
  "matches": [
    {
      "id": "Guid_8",
      "country1": "Poland",
      "country2": "Spain",
      "matchDateTime": "DateTimeOffset_22",
      "result": null,
      "isStarted": false
    },
    {
      "id": "Guid_9",
      "country1": "Germany",
      "country2": "France",
      "matchDateTime": "DateTimeOffset_23",
      "result": null,
      "isStarted": false
    }
  ],
  "bets": [
    {
      "id": "Guid_10",
      "userId": "Guid_1",
      "matchId": "Guid_8",
      "prediction": "1:0",
      "editedAt": "DateTimeOffset_24"
    },
    {
      "id": "Guid_11",
      "userId": "Guid_2",
      "matchId": "Guid_8",
      "prediction": "2:1",
      "editedAt": "DateTimeOffset_25"
    },
    {
      "id": "Guid_12",
      "userId": "Guid_3",
      "matchId": "Guid_9",
      "prediction": "X",
      "editedAt": "DateTimeOffset_26"
    }
  ],
  "leaderboard": [
    {
      "position": 1,
      "userId": "Guid_1",
      "login": "{Scrubbed}",
      "points": 0,
      "exactHits": 0,
      "winnerHits": 0,
      "totalBets": 0,
      "accuracy": 0
    },
    {
      "position": 2,
      "userId": "Guid_2",
      "login": "{Scrubbed}",
      "points": 0,
      "exactHits": 0,
      "winnerHits": 0,
      "totalBets": 0,
      "accuracy": 0
    },
    {
      "position": 3,
      "userId": "Guid_3",
      "login": "{Scrubbed}",
      "points": 0,
      "exactHits": 0,
      "winnerHits": 0,
      "totalBets": 0,
      "accuracy": 0
    }
  ]
}
```

#### Odpowiedź 200 OK – z pulą aktywną

Gdy dla meczu istnieje pula, w odpowiedzi pojawia się dodatkowe pole `pools`:

```json
{
  "table": { "..." : "..." },
  "members": ["..."],
  "matches": [
    {
      "id": "Guid_6",
      "country1": "Poland",
      "country2": "Spain",
      "matchDateTime": "DateTimeOffset_12",
      "result": null,
      "isStarted": false
    }
  ],
  "pools": [
    {
      "id": "Guid_7",
      "matchId": "Guid_6",
      "amount": 200.0,
      "status": "active"
    }
  ]
}
```

#### Odpowiedź 200 OK – z pulą wygraną (status `won`)

```json
{
  "table": { "..." : "..." },
  "members": ["..."],
  "matches": [
    {
      "id": "Guid_6",
      "country1": "Germany",
      "country2": "France",
      "matchDateTime": "DateTimeOffset_9",
      "result": "2:1",
      "isStarted": true
    }
  ],
  "pools": [
    {
      "id": "Guid_7",
      "matchId": "Guid_6",
      "amount": 200.0,
      "status": "won",
      "winners": [
        "Guid_1"
      ]
    }
  ]
}
```

Statusy pul obserwowane w snapshotach: `"active"`, `"expired"`, `"won"`, `"rollover"`.

#### Opis systemu punktowego (leaderboard)

| Typ trafienia | Punkty | Opis |
|---------------|--------|------|
| Dokładny wynik (`exactHits`) | 3 pkt | Przewidziano dokładny wynik meczu |
| Trafiony zwycięzca (`winnerHits`) | 1 pkt | Przewidziano właściwego zwycięzcę (lub remis `X`) |
| Pudło | 0 pkt | Brak trafienia |

Pole `accuracy` informuje o ogólnej skuteczności gracza.

#### Odpowiedź 401 – brak tokenu

```json
null
```
*(pusta odpowiedź z nagłówkiem `WWW-Authenticate: Bearer`)*

#### Odpowiedź 403 – nie jest członkiem stołu

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

#### Odpowiedź 404 – stół nie istnieje

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Dashboard stołu |
| `401 Unauthorized` | Brak tokenu |
| `403 Forbidden` | Użytkownik nie jest członkiem stołu |
| `404 Not Found` | Stół nie istnieje |

---

### DELETE /api/tables/{tableId}/members

Opuszczenie stołu przez zalogowanego użytkownika.

- **Metoda:** `DELETE`
- **Ścieżka:** `/api/tables/{tableId}/members`
- **Autoryzacja:** Bearer token (wymagane)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |

#### Przykładowe żądanie

```http
DELETE /api/tables/Guid_3/members HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Successfully left table"
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Pomyślnie opuszczono stół |

---

### POST /api/tables/{tableId}/admins

Nadaje uprawnienia administratora wybranemu użytkownikowi stołu. Może wykonać tylko twórca (kreator) stołu.

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/{tableId}/admins`
- **Autoryzacja:** Bearer token (wymagane, kreator stołu)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |

#### Body (request)

```json
{
  "UserId": "GUID"
}
```

#### Przykładowe żądanie

```http
POST /api/tables/Guid_5/admins HTTP/1.1
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

| Kod | Opis |
|-----|------|
| `200 OK` | Uprawnienia admina nadane |

---

### DELETE /api/tables/{tableId}/admins/{userId}

Odbiera uprawnienia administratora od wybranego użytkownika stołu. Może wykonać tylko twórca (kreator) stołu.

- **Metoda:** `DELETE`
- **Ścieżka:** `/api/tables/{tableId}/admins/{userId}`
- **Autoryzacja:** Bearer token (wymagane, kreator stołu)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |
| `userId` | GUID | Identyfikator użytkownika, któremu odbieramy uprawnienia |

#### Przykładowe żądanie

```http
DELETE /api/tables/Guid_5/admins/Guid_2 HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Admin role revoked"
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Uprawnienia admina odebrane |

---

### PATCH /api/tables/{tableId}

Aktualizuje parametry stołu. Można zmienić dowolny podzbiór pól — pola pominięte w żądaniu pozostają bez zmian. Może wykonać tylko admin stołu.

- **Metoda:** `PATCH`
- **Ścieżka:** `/api/tables/{tableId}`
- **Autoryzacja:** Bearer token (wymagane, admin stołu)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |

#### Ciało żądania (wszystkie pola opcjonalne)

```json
{
  "name": "string | null",
  "password": "string | null",
  "maxPlayers": "integer | null",
  "stake": "decimal | null"
}
```

| Pole | Typ | Opis |
|------|-----|------|
| `name` | string? | Nowa nazwa stołu (maks. 100 znaków). Pominięcie = bez zmiany |
| `password` | string? | Nowe hasło stołu (będzie zahashowane). Pominięcie = bez zmiany |
| `maxPlayers` | integer? | Nowa maks. liczba graczy (min. 1, nie może być mniejsza niż aktualna liczba członków). Pominięcie = bez zmiany |
| `stake` | decimal? | Nowa stawka (wartość nieujemna). Pominięcie = bez zmiany |

#### Przykładowe żądanie – zmiana tylko nazwy

```http
PATCH /api/tables/Guid_1 HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "name": "Nowa nazwa stołu"
}
```

#### Przykładowe żądanie – zmiana wszystkich pól

```http
PATCH /api/tables/Guid_1 HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "name": "Nowa nazwa",
  "password": "NoweHaslo@123",
  "maxPlayers": 20,
  "stake": 100.00
}
```

#### Odpowiedź 200 OK

```json
{
  "id": "Guid_1",
  "name": "Nowa nazwa stołu",
  "maxPlayers": 10,
  "stake": 50.00,
  "createdBy": "Guid_2",
  "createdAt": "DateTimeOffset_1",
  "isSecretMode": false
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Parametry stołu zaktualizowane |
| `400 Bad Request` | Błąd walidacji (np. pusta nazwa, ujemna stawka, `maxPlayers` poniżej liczby członków) |
| `401 Unauthorized` | Brak tokenu |
| `403 Forbidden` | Użytkownik nie jest adminem stołu (`Table.NotAdmin`) |
| `404 Not Found` | Stół nie istnieje (`Table.NotFound`) |

---

## MatchesController

### GET /api/tables/{tableId}/matches/{matchId}

Pobiera szczegóły meczu w kontekście danego stołu. Pole `started` wskazuje, czy mecz już się rozpoczął (data w przeszłości).

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}`
- **Autoryzacja:** Bearer token (wymagane)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |
| `matchId` | GUID | Identyfikator meczu |

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
  "matchDateTime": "DateTimeOffset_10",
  "result": null,
  "status": "scheduled",
  "started": true,
  "createdAt": "DateTimeOffset_11"
}
```

> Pole `started` jest `true` gdy `matchDateTime` jest w przeszłości, a `false` gdy data jest w przyszłości. Nie zależy od statusu meczu.

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Szczegóły meczu |

---

## BetsController

### POST /api/tables/{tableId}/matches/{matchId}/bets

Składa lub aktualizuje zakład na wynik meczu. Jeśli zakład dla danego użytkownika i meczu już istnieje, zostanie zaktualizowany (zwraca `201 Created` przy aktualizacji).

- **Metoda:** `POST`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}/bets`
- **Autoryzacja:** Bearer token (wymagane)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |
| `matchId` | GUID | Identyfikator meczu |

#### Body (request)

```json
{
  "Prediction": "string"
}
```

Format typowania:
- `"X:Y"` – przewidywany wynik, np. `"2:1"`, `"0:0"`, `"3:2"`
- `"X"` – remis (draw)

Walidacje:
- Maksymalna długość: 10 znaków
- Musi być w formacie `"wynik1:wynik2"` lub `"X"` (remis)

#### Przykładowe żądanie

```http
POST /api/tables/Guid_4/matches/Guid_7/bets HTTP/1.1
Authorization: Bearer {Scrubbed}
Content-Type: application/json

{
  "Prediction": "2:1"
}
```

#### Odpowiedź 201 Created – nowy zakład

```json
{
  "id": "Guid_8",
  "userId": "Guid_5",
  "matchId": "Guid_7",
  "prediction": "2:1",
  "editedAt": "DateTimeOffset_14"
}
```

Nagłówek `Location`: `http://localhost/api/tables/Guid_4/matches/Guid_7/bets/my`

#### Odpowiedź 201 Created – zakład z remisem

```json
{
  "id": "Guid_8",
  "userId": "Guid_5",
  "matchId": "Guid_7",
  "prediction": "X",
  "editedAt": "DateTimeOffset_14"
}
```

#### Odpowiedź 201 Created – zaktualizowany zakład

Przy aktualizacji istniejącego zakładu API zwraca `201 Created` z nową wartością:

```json
{
  "id": "Guid_8",
  "userId": "Guid_5",
  "matchId": "Guid_7",
  "prediction": "2:1",
  "editedAt": "DateTimeOffset_15"
}
```

#### Odpowiedź 400 – nieprawidłowy format typowania

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

#### Odpowiedź 404 – mecz lub stół nie istnieje

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `201 Created` | Zakład złożony lub zaktualizowany |
| `400 Bad Request` | Nieprawidłowy format typowania |
| `404 Not Found` | Mecz lub stół nie istnieje |

---

### GET /api/tables/{tableId}/matches/{matchId}/bets/my

Pobiera zakład zalogowanego użytkownika dla danego meczu.

- **Metoda:** `GET`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}/bets/my`
- **Autoryzacja:** Bearer token (wymagane)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |
| `matchId` | GUID | Identyfikator meczu |

#### Przykładowe żądanie

```http
GET /api/tables/Guid_4/matches/Guid_7/bets/my HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "id": "Guid_8",
  "userId": "Guid_5",
  "matchId": "Guid_7",
  "prediction": "3:2",
  "editedAt": "DateTimeOffset_15"
}
```

#### Odpowiedź 404 – brak zakładu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Zakład użytkownika |
| `404 Not Found` | Użytkownik nie ma zakładu na ten mecz |

---

### DELETE /api/tables/{tableId}/matches/{matchId}/bets

Usuwa zakład zalogowanego użytkownika dla danego meczu. Możliwe tylko przed rozpoczęciem meczu.

- **Metoda:** `DELETE`
- **Ścieżka:** `/api/tables/{tableId}/matches/{matchId}/bets`
- **Autoryzacja:** Bearer token (wymagane)

#### Parametry ścieżki

| Parametr | Typ | Opis |
|----------|-----|------|
| `tableId` | GUID | Identyfikator stołu |
| `matchId` | GUID | Identyfikator meczu |

#### Przykładowe żądanie

```http
DELETE /api/tables/Guid_4/matches/Guid_7/bets HTTP/1.1
Authorization: Bearer {Scrubbed}
```

#### Odpowiedź 200 OK

```json
{
  "message": "Bet deleted successfully"
}
```

#### Odpowiedź 404 – brak zakładu do usunięcia

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

#### Kody statusu

| Kod | Opis |
|-----|------|
| `200 OK` | Zakład usunięty |
| `404 Not Found` | Brak zakładu do usunięcia |

---

## Typy modeli

### Auth – modele

#### AuthResponse

Odpowiedź zwracana po rejestracji, logowaniu i odświeżeniu tokenów.

```json
{
  "accessToken": "string",
  "refreshToken": "string",
  "expiresAt": "DateTimeOffset",
  "user": {
    "id": "GUID",
    "login": "string",
    "email": "string",
    "createdAt": "DateTimeOffset",
    "isActive": "boolean",
    "isSuperAdmin": "boolean"
  }
}
```

#### UserResponse

| Pole | Typ | Opis |
|------|-----|------|
| `id` | GUID | Unikalny identyfikator użytkownika |
| `login` | string | Login użytkownika |
| `email` | string | Adres email |
| `createdAt` | DateTimeOffset | Data rejestracji |
| `isActive` | boolean | Czy konto jest aktywne |
| `isSuperAdmin` | boolean | Czy użytkownik posiada uprawnienia SuperAdmina |

#### RegisterRequest

| Pole | Typ | Wymagane | Opis |
|------|-----|----------|------|
| `Login` | string | tak | Login użytkownika |
| `Password` | string | tak | Hasło (min. 10 znaków, wielka litera, cyfra, znak specjalny) |
| `Email` | string | tak | Adres email (poprawny format) |

#### LoginRequest

| Pole | Typ | Wymagane | Opis |
|------|-----|----------|------|
| `Login` | string | tak | Login użytkownika |
| `Password` | string | tak | Hasło |

#### RefreshRequest / LogoutRequest

| Pole | Typ | Wymagane | Opis |
|------|-----|----------|------|
| `RefreshToken` | string | tak | Refresh token |

---

### EventTypes – modele

#### EventTypeResponse

```json
{
  "id": "GUID",
  "code": "string",
  "name": "string",
  "startDate": "DateTimeOffset",
  "isActive": "boolean"
}
```

| Pole | Typ | Opis |
|------|-----|------|
| `id` | GUID | Unikalny identyfikator |
| `code` | string | Unikalny kod (np. `"LIGA_MISTRZOW_2026"`) |
| `name` | string | Nazwa wyświetlana |
| `startDate` | DateTimeOffset | Data rozpoczęcia |
| `isActive` | boolean | Czy aktywny (widoczny publicznie) |

#### CreateEventTypeRequest

| Pole | Typ | Wymagane | Opis |
|------|-----|----------|------|
| `Code` | string | tak | Unikalny kod wydarzenia |
| `Name` | string | tak | Nazwa wyświetlana |
| `StartDate` | DateTimeOffset | tak | Data rozpoczęcia |

#### UpdateEventTypeRequest

| Pole | Typ | Wymagane | Opis |
|------|-----|----------|------|
| `Name` | string | tak | Nowa nazwa wyświetlana |
| `StartDate` | DateTimeOffset | tak | Nowa data rozpoczęcia |

---

### AdminMatches – modele

#### MatchResponse (admin)

Odpowiedź z endpointów administracyjnych (GET lista, POST tworzenie):

```json
{
  "id": "GUID",
  "eventTypeId": "GUID",
  "country1": "string",
  "country2": "string",
  "matchDateTime": "DateTimeOffset",
  "status": "string",
  "createdAt": "DateTimeOffset"
}
```

Odpowiedź z endpointów GET (lista) oraz PUT (aktualizacja) zawiera dodatkowo pola `result` i `started`:

```json
{
  "id": "GUID",
  "eventTypeId": "GUID",
  "country1": "string",
  "country2": "string",
  "matchDateTime": "DateTimeOffset",
  "result": "string | null",
  "status": "string",
  "started": "boolean",
  "createdAt": "DateTimeOffset"
}
```

#### SetMatchResultRequest

| Pole | Typ | Wymagane | Opis |
|------|-----|----------|------|
| `Result` | string | tak | Wynik meczu w formacie `"X:Y"` (np. `"2:1"`) |

#### SetMatchResultResponse

```json
{
  "id": "GUID",
  "result": "string",
  "status": "finished"
}
```

#### Statusy meczów

| Status | Opis |
|--------|------|
| `scheduled` | Zaplanowany |
| `finished` | Zakończony (wynik ustawiony) |

---

### Tables – modele

#### UpdateTableRequest (edycja stołu)

Wszystkie pola są opcjonalne. Pola pominięte lub `null` są ignorowane.

```json
{
  "name": "string | null",
  "password": "string | null",
  "maxPlayers": "integer | null",
  "stake": "decimal | null"
}
```

#### TableResponse (tworzenie / legacy join / edycja)

```json
{
  "id": "GUID",
  "name": "string",
  "maxPlayers": "integer",
  "stake": "decimal",
  "createdBy": "GUID",
  "createdAt": "DateTimeOffset",
  "isSecretMode": "boolean"
}
```

#### TableDetailsResponse (GET szczegóły)

```json
{
  "id": "GUID",
  "name": "string",
  "maxPlayers": "integer",
  "stake": "decimal",
  "createdAt": "DateTimeOffset",
  "members": [
    {
      "userId": "GUID",
      "login": "string",
      "isAdmin": "boolean",
      "joinedAt": "DateTimeOffset"
    }
  ]
}
```

#### JoinTableAuthorizedResponse

```json
{
  "tableId": "GUID",
  "tableName": "string",
  "maxPlayers": "integer",
  "stake": "decimal",
  "tableCreatedAt": "DateTimeOffset",
  "userId": "GUID",
  "userLogin": "string",
  "isAdmin": "boolean",
  "joinedAt": "DateTimeOffset",
  "currentMemberCount": "integer"
}
```

#### DashboardResponse

```json
{
  "table": {
    "id": "GUID",
    "name": "string",
    "maxPlayers": "integer",
    "stake": "decimal",
    "createdAt": "DateTimeOffset"
  },
  "members": [
    {
      "userId": "GUID",
      "login": "string",
      "isAdmin": "boolean",
      "joinedAt": "DateTimeOffset"
    }
  ],
  "matches": [
    {
      "id": "GUID",
      "country1": "string",
      "country2": "string",
      "matchDateTime": "DateTimeOffset",
      "result": "string | null",
      "isStarted": "boolean"
    }
  ],
  "bets": [
    {
      "id": "GUID",
      "userId": "GUID",
      "matchId": "GUID",
      "prediction": "string",
      "editedAt": "DateTimeOffset"
    }
  ],
  "leaderboard": [
    {
      "position": "integer",
      "userId": "GUID",
      "login": "string",
      "points": "integer",
      "exactHits": "integer",
      "winnerHits": "integer",
      "totalBets": "integer",
      "accuracy": "decimal"
    }
  ],
  "pools": [
    {
      "id": "GUID",
      "matchId": "GUID",
      "amount": "decimal",
      "status": "string",
      "winners": ["GUID"]
    }
  ]
}
```

> Pola `matches`, `bets`, `leaderboard` i `pools` są opcjonalne – pojawiają się tylko gdy są dostępne dane.

#### Statusy pul (pools)

| Status | Opis |
|--------|------|
| `active` | Pula aktywna (mecz nie zakończony) |
| `expired` | Pula wygasła |
| `won` | Pula wygrana – pole `winners` zawiera GUID zwycięzców |
| `rollover` | Pula przeniesiona (brak zwycięzców) |

---

### Matches – modele

#### MatchResponse (publiczny)

```json
{
  "id": "GUID",
  "eventTypeId": "GUID",
  "country1": "string",
  "country2": "string",
  "matchDateTime": "DateTimeOffset",
  "result": "string | null",
  "status": "string",
  "started": "boolean",
  "createdAt": "DateTimeOffset"
}
```

| Pole | Typ | Opis |
|------|-----|------|
| `id` | GUID | Identyfikator meczu |
| `eventTypeId` | GUID | Identyfikator typu wydarzenia |
| `country1` | string | Drużyna/kraj 1 |
| `country2` | string | Drużyna/kraj 2 |
| `matchDateTime` | DateTimeOffset | Data i godzina meczu |
| `result` | string\|null | Wynik w formacie `"X:Y"` lub `null` jeśli nie zakończony |
| `status` | string | Status meczu (`scheduled`, `finished`) |
| `started` | boolean | `true` gdy `matchDateTime` minął |
| `createdAt` | DateTimeOffset | Data utworzenia rekordu |

---

### Bets – modele

#### BetResponse

```json
{
  "id": "GUID",
  "userId": "GUID",
  "matchId": "GUID",
  "prediction": "string",
  "editedAt": "DateTimeOffset"
}
```

| Pole | Typ | Opis |
|------|-----|------|
| `id` | GUID | Identyfikator zakładu |
| `userId` | GUID | Identyfikator użytkownika |
| `matchId` | GUID | Identyfikator meczu |
| `prediction` | string | Typowanie w formacie `"X:Y"` lub `"X"` (remis) |
| `editedAt` | DateTimeOffset | Data ostatniej edycji |

#### PlaceBetRequest

| Pole | Typ | Wymagane | Opis |
|------|-----|----------|------|
| `Prediction` | string | tak | Typowanie: `"X:Y"` (wynik) lub `"X"` (remis), max 10 znaków |

---

## Błędy i kody statusu

### Walidacja 400 – RFC 9110

Błędy walidacji modelu zwracane są w standardowym formacie ASP.NET:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "NazwaPola": [
      "Komunikat błędu"
    ]
  },
  "traceId": "{Scrubbed}"
}
```

### Błędy domenowe (tablice `errors`)

Błędy biznesowe mogą przyjmować dwa formaty:

**Format z `errorCode`:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "{Scrubbed}",
  "errors": [
    {
      "errorCode": "Namespace.ErrorCode",
      "description": "Opis błędu po polsku"
    }
  ]
}
```

**Format z `code`:**
```json
{
  "errors": [
    {
      "code": "Namespace.ErrorCode",
      "description": "Opis błędu po polsku"
    }
  ]
}
```

### 401 Unauthorized

Zwracana gdy brak tokenu Bearer lub token jest nieważny:

```json
null
```

*(Pusta odpowiedź z nagłówkiem `WWW-Authenticate: Bearer`)*

### 403 Forbidden

Zwracana gdy użytkownik nie ma uprawnień do zasobu (np. nie jest SuperAdminem lub nie należy do stołu):

```json
null
```

*(Pusta odpowiedź, lub w przypadku dashboardu:)*
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

### 404 Not Found

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "{Scrubbed}"
}
```

### 409 Conflict

Zwracana przy próbie stworzenia zasobu, który już istnieje. Format zależy od endpointu:

**EventType (kod już istnieje):**
```json
{
  "errors": [
    {
      "code": "EventType.CodeExists",
      "description": "Kod wydarzenia już istnieje"
    }
  ]
}
```

**Table (nazwa stołu już istnieje):**
```json
{
  "code": "Table.DuplicateName",
  "message": "Stół o podanej nazwie już istnieje"
}
```

**Table (użytkownik już jest członkiem):**
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

---

### Zestawienie kodów statusu

| Kod | Nazwa | Kiedy |
|-----|-------|-------|
| `200 OK` | OK | Sukces (GET, DELETE, POST dla login/logout) |
| `201 Created` | Created | Zasób został utworzony (POST tworzący zasób, POST zakładu) |
| `400 Bad Request` | Bad Request | Błąd walidacji lub reguły biznesowej |
| `401 Unauthorized` | Unauthorized | Brak lub nieważny token Bearer |
| `403 Forbidden` | Forbidden | Brak uprawnień do zasobu |
| `404 Not Found` | Not Found | Zasób nie istnieje |
| `409 Conflict` | Conflict | Konflikt (duplikat kodu, nazwy lub członkostwa) |

### Zestawienie kodów błędów domenowych

| Kod błędu | Opis | HTTP |
|-----------|------|------|
| `User.AlreadyExists` | Użytkownik o podanym loginie już istnieje | 400 |
| `Auth.InvalidCredentials` | Nieprawidłowy login lub hasło | 400 |
| `Auth.RefreshNotFound` | Refresh token nie został znaleziony | 400 |
| `RefreshToken.NotFound` | Refresh token nie został znaleziony | 400 |
| `EventType.CodeExists` | Kod wydarzenia już istnieje | 409 |
| `Match.AlreadyStarted` | Mecz już się rozpoczął | 400 |
| `Match.NotStarted` | Mecz jeszcze się nie rozpoczął | 400 |
| `Score.InvalidFormat` | Nieprawidłowy format wyniku | 400 |
| `Table.InvalidPassword` | Nieprawidłowe hasło do stołu | 400 |
| `Table.Full` | Stół jest pełny | 400 |
| `Table.DuplicateName` | Stół o podanej nazwie już istnieje | 409 |
| `Table.AlreadyMember` | Użytkownik jest już członkiem tego stołu | 409 |
| `Table.AccessDenied` | Brak dostępu do stołu | 403 |
