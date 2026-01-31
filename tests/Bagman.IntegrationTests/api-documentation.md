# prompt do AI o stworzenie dokumentacji API
Na bazie snapshotów, wygeneruj dokumentację API w formacie markdown. Dokumentacja powinna zawierać opis każdego endpointu, metodę HTTP, ścieżkę, wymagane parametry, przykładowe zapytania i odpowiedzi oraz kody statusu HTTP. Upewnij się, że dokumentacja jest czytelna i dobrze sformatowana.
Wykorzystaj wskazówkę o tym, w jaki sposób powstały snsnapshoty
- Kazdy test wykonuje różne zapytania http do endpointów API Bagman w celu odwzorowania określonego scenariusza. Snapshot zapisuje pełne nagranie wysztkich zapytań i odpowiedzi HTTP wykonanych podczas testu.
- ze snapshotów wymazane są różne dane np. Tokeny autoryzacyjne, dane użytkowników itp. aby dokumentacja nie zawierała wrażliwych informacji.

Oczekiwania co do dokumentacji
- Dokumentacja powinna być podzielona na sekcje według kontrolerów API.
- Każda sekcja powinna zawierać podsekcje dla każdego endpointu.
- Każda podsekcja powinna zawierać:
  - Opis endpointu
  - Metodę HTTP
  - Ścieżkę endpointu
  - Wymagane parametry (jeśli istnieją)
  - Przykładowe zapytania
  - Przykładowe odpowiedzi
  - Kody statusu HTTP
  - Opis typów w zapytaniach i odpowiedziach (jeśli istnieją)
- Dokumentacja powinna zawierać wyłącznie unikalne endpointy (bez duplikatów).
- Osobno opisz typy błędów ich kody oraz kiedy mogą wystąpić
- Dokumentacja powinna być w pliku .md i sformatowana w markdown.

Pamiętaj aby całą dokumentację zwrócić mi w jednym pliku tak abym mógł to w łatwt y sposób skopiować i zapisać jako plik markdown.

# Dokumentacja API Bagman

Poniższa dokumentacja została wygenerowana na podstawie snapshotów z katalogu `tests/Bagman.IntegrationTests/Controllers`. Każdy snapshot jest pełnym nagraniem żądań i odpowiedzi HTTP wykonanych podczas testów scenariuszowych. Wrażliwe dane (tokeny, loginy, e-maile) są zamaskowane jako `{Scrubbed}`.

## AuthController

### POST /api/auth/register
- Opis: Rejestruje nowego użytkownika i zwraca parę tokenów oraz dane użytkownika.
- Metoda HTTP: POST
- Ścieżka: /api/auth/register
- Wymagane parametry (body JSON):
  - Login: string
  - Password: string
  - Email: string
- Przykładowe zapytanie:
  - Body:
    ```json
    {
      "Login": "{Scrubbed}",
      "Password": "Test@12345",
      "Email": "{Scrubbed}"
    }
    ```
- Przykładowa odpowiedź 200 OK:
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
- Kody statusu HTTP:
  - 200 OK – rejestracja zakończona powodzeniem
  - 400 Bad Request – nieprawidłowe dane (np. błędny e-mail, słabe hasło, duplikat loginu)

### POST /api/auth/login
- Opis: Loguje użytkownika i zwraca parę tokenów oraz dane użytkownika.
- Metoda HTTP: POST
- Ścieżka: /api/auth/login
- Wymagane parametry (body JSON):
  - Login: string
  - Password: string
- Przykładowe zapytanie:
  - Body:
    ```json
    {
      "Login": "{Scrubbed}",
      "Password": "Test@12345"
    }
    ```
- Przykładowa odpowiedź 200 OK:
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
- Kody statusu HTTP:
  - 200 OK – logowanie zakończone powodzeniem
  - 400 Bad Request – błędne dane logowania lub nieistniejący użytkownik

### POST /api/auth/refresh
- Opis: Odświeża tokeny autoryzacyjne na podstawie refresh tokenu.
- Metoda HTTP: POST
- Ścieżka: /api/auth/refresh
- Wymagane parametry (body JSON):
  - RefreshToken: string
- Przykładowe zapytanie:
  - Body:
    ```json
    {
      "RefreshToken": "{Scrubbed}"
    }
    ```
- Przykładowa odpowiedź 200 OK:
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
- Kody statusu HTTP:
  - 200 OK – odświeżenie zakończone powodzeniem
  - 400 Bad Request – nieprawidłowy refresh token

### POST /api/auth/logout
- Opis: Wylogowuje użytkownika unieważniając refresh token.
- Metoda HTTP: POST
- Ścieżka: /api/auth/logout
- Wymagane parametry (body JSON):
  - RefreshToken: string
- Przykładowe zapytanie:
  - Body:
    ```json
    {
      "RefreshToken": "{Scrubbed}"
    }
    ```
- Przykładowa odpowiedź 200 OK:
  ```json
  {
    "message": "Wylogowano pomyślnie"
  }
  ```
- Kody statusu HTTP:
  - 200 OK – wylogowanie zakończone powodzeniem
  - 400 Bad Request – nieprawidłowy refresh token

## TablesController

### POST /api/tables
- Opis: Tworzy nową tabelę (stół) do typowania.
- Metoda HTTP: POST
- Ścieżka: /api/tables
- Wymagane parametry (body JSON):
  - UserLogin: string
  - UserPassword: string
  - TableName: string
  - TablePassword: string
  - MaxPlayers: number
  - Stake: number
- Przykładowe zapytanie:
  - Body:
    ```json
    {
      "UserLogin": "{Scrubbed}",
      "UserPassword": "Creator@12345",
      "TableName": "Betting Table Guid_1",
      "TablePassword": "BettingPass@123",
      "MaxPlayers": 10,
      "Stake": 50.0
    }
    ```
- Przykładowa odpowiedź 201 Created:
  - Nagłówki:
    - Location: `http://localhost/api/Tables/Guid_2`
  - Body:
    ```json
    {
      "id": "Guid_2",
      "name": "Betting Table Guid_1",
      "maxPlayers": 10,
      "stake": 50.0,
      "createdBy": "Guid_3",
      "createdAt": "DateTimeOffset_1",
      "isSecretMode": false
    }
    ```
- Kody statusu HTTP:
  - 201 Created – tabela utworzona
  - 400 Bad Request – nieprawidłowa nazwa stołu lub parametry

### POST /api/tables/join
- Opis: Dołącza użytkownika do istniejącej tabeli.
- Metoda HTTP: POST
- Ścieżka: /api/tables/join
- Wymagane parametry (body JSON):
  - UserLogin: string
  - UserPassword: string
  - TableName: string
  - TablePassword: string
- Przykładowe zapytanie:
  - Body:
    ```json
    {
      "UserLogin": "{Scrubbed}",
      "UserPassword": "Player@12345",
      "TableName": "Betting Table Guid_1",
      "TablePassword": "BettingPass@123"
    }
    ```
- Przykładowa odpowiedź 200 OK:
  ```json
  {
    "id": "Guid_2",
    "name": "Betting Table Guid_1",
    "maxPlayers": 10,
    "stake": 50.0,
    "createdBy": "Guid_3",
    "createdAt": "DateTimeOffset_1",
    "isSecretMode": false
  }
  ```
- Kody statusu HTTP:
  - 200 OK – dołączenie zakończone powodzeniem
  - 400 Bad Request – błędne hasło stołu lub pełny stół

### GET /api/tables
- Opis: Zwraca listę tabel, do których należy aktualny użytkownik.
- Metoda HTTP: GET
- Ścieżka: /api/tables
- Wymagane nagłówki:
  - Authorization: Bearer `{token}`
- Przykładowe zapytanie:
  - Nagłówki:
    - Authorization: `{Scrubbed}`
- Przykładowa odpowiedź 200 OK:
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
- Kody statusu HTTP:
  - 200 OK – lista zwrócona
  - 401 Unauthorized – brak ważnego tokenu

## MatchesController

### POST /api/tables/{tableId}/matches
- Opis: Tworzy mecz w wybranej tabeli.
- Metoda HTTP: POST
- Ścieżka: /api/tables/{tableId}/matches
- Wymagane nagłówki:
  - Authorization: Bearer `{token}`
- Wymagane parametry (body JSON):
  - Country1: string
  - Country2: string
  - MatchDateTime: DateTimeOffset
- Przykładowe zapytanie:
  - Nagłówki:
    - Authorization: `{Scrubbed}`
  - Body:
    ```json
    {
      "Country1": "Italy",
      "Country2": "France",
      "MatchDateTime": "DateTimeOffset_6"
    }
    ```
- Przykładowa odpowiedź 201 Created:
  - Nagłówki:
    - Location: `http://localhost/api/tables/Guid_2/matches/Guid_5`
  - Body:
    ```json
    {
      "id": "Guid_5",
      "tableId": "Guid_2",
      "country1": "Italy",
      "country2": "France",
      "matchDateTime": "DateTimeOffset_6",
      "result": null,
      "status": "scheduled",
      "started": false,
      "createdAt": "DateTimeOffset_7"
    }
    ```
- Kody statusu HTTP:
  - 201 Created – mecz utworzony
  - 400 Bad Request – nieprawidłowe dane meczu
  - 403 Forbidden – brak uprawnień admina tabeli

### GET /api/tables/{tableId}/matches/{matchId}
- Opis: Pobiera szczegóły meczu.
- Metoda HTTP: GET
- Ścieżka: /api/tables/{tableId}/matches/{matchId}
- Wymagane nagłówki:
  - Authorization: Bearer `{token}`
- Przykładowe zapytanie:
  - Nagłówki:
    - Authorization: `{Scrubbed}`
- Przykładowa odpowiedź 200 OK:
  ```json
  {
    "id": "Guid_3",
    "tableId": "Guid_1",
    "country1": "Spain",
    "country2": "Portugal",
    "matchDateTime": "DateTimeOffset_4",
    "result": null,
    "status": "scheduled",
    "started": false,
    "createdAt": "DateTimeOffset_5"
  }
  ```
- Kody statusu HTTP:
  - 200 OK – mecz zwrócony
  - 401 Unauthorized – brak ważnego tokenu
  - 404 Not Found – nie znaleziono meczu

### PUT /api/tables/{tableId}/matches/{matchId}
- Opis: Aktualizuje szczegóły meczu przed jego rozpoczęciem.
- Metoda HTTP: PUT
- Ścieżka: /api/tables/{tableId}/matches/{matchId}
- Wymagane nagłówki:
  - Authorization: Bearer `{token}`
- Wymagane parametry (body JSON):
  - Country1: string
  - Country2: string
  - MatchDateTime: DateTimeOffset
- Przykładowe zapytanie:
  - Nagłówki:
    - Authorization: `{Scrubbed}`
  - Body:
    ```json
    {
      "Country1": "Netherlands",
      "Country2": "Austria",
      "MatchDateTime": "DateTimeOffset_6"
    }
    ```
- Przykładowa odpowiedź 200 OK:
  ```json
  {
    "message": "Match updated successfully"
  }
  ```
- Kody statusu HTTP:
  - 200 OK – mecz zaktualizowany
  - 400 Bad Request – nieprawidłowe dane
  - 401 Unauthorized – brak ważnego tokenu
  - 403 Forbidden – brak uprawnień
  - 404 Not Found – nie znaleziono meczu

## BetsController

### POST /api/tables/{tableId}/matches/{matchId}/bets
- Opis: Składa typ (bet) dla wybranego meczu.
- Metoda HTTP: POST
- Ścieżka: /api/tables/{tableId}/matches/{matchId}/bets
- Wymagane nagłówki:
  - Authorization: Bearer `{token}`
- Wymagane parametry (body JSON):
  - Prediction: string (format `X:Y`)
- Przykładowe zapytanie:
  - Nagłówki:
    - Authorization: `{Scrubbed}`
  - Body:
    ```json
    {
      "Prediction": "2:1"
    }
    ```
- Przykładowa odpowiedź 201 Created:
  - Nagłówki:
    - Location: `http://localhost/api/tables/Guid_2/matches/Guid_5/bets/my`
  - Body:
    ```json
    {
      "id": "Guid_6",
      "userId": "Guid_4",
      "matchId": "Guid_5",
      "prediction": "2:1",
      "editedAt": "DateTimeOffset_8"
    }
    ```
- Kody statusu HTTP:
  - 201 Created – typ zapisany
  - 400 Bad Request – nieprawidłowy format typu
  - 401 Unauthorized – brak ważnego tokenu

### GET /api/tables/{tableId}/matches/{matchId}/bets/my
- Opis: Pobiera typ aktualnego użytkownika dla meczu.
- Metoda HTTP: GET
- Ścieżka: /api/tables/{tableId}/matches/{matchId}/bets/my
- Wymagane nagłówki:
  - Authorization: Bearer `{token}`
- Przykładowe zapytanie:
  - Nagłówki:
    - Authorization: `{Scrubbed}`
- Przykładowa odpowiedź 200 OK:
  ```json
  {
    "id": "Guid_6",
    "userId": "Guid_4",
    "matchId": "Guid_5",
    "prediction": "3:2",
    "editedAt": "DateTimeOffset_8"
  }
  ```
- Kody statusu HTTP:
  - 200 OK – typ zwrócony
  - 401 Unauthorized – brak ważnego tokenu
  - 404 Not Found – brak złożonego typu

### DELETE /api/tables/{tableId}/matches/{matchId}/bets
- Opis: Usuwa złożony typ jeśli mecz jeszcze się nie rozpoczął.
- Metoda HTTP: DELETE
- Ścieżka: /api/tables/{tableId}/matches/{matchId}/bets
- Wymagane nagłówki:
  - Authorization: Bearer `{token}`
- Przykładowe zapytanie:
  - Nagłówki:
    - Authorization: `{Scrubbed}`
- Przykładowa odpowiedź 200 OK:
  ```json
  {
    "message": "Bet deleted successfully"
  }
  ```
- Kody statusu HTTP:
  - 200 OK – typ usunięty
  - 401 Unauthorized – brak ważnego tokenu
  - 404 Not Found – brak złożonego typu

## Typy danych

- AuthResponse:
  - accessToken: string
  - refreshToken: string
  - expiresAt: DateTimeOffset
  - user:
    - id: Guid
    - login: string
    - email: string
    - createdAt: DateTimeOffset
    - isActive: bool

- TableResponse:
  - id: Guid
  - name: string
  - maxPlayers: number
  - stake: number
  - createdBy: Guid
  - createdAt: DateTimeOffset
  - isSecretMode: bool

- MatchResponse:
  - id: Guid
  - tableId: Guid
  - country1: string
  - country2: string
  - matchDateTime: DateTimeOffset
  - result: object|null
  - status: string
  - started: bool
  - createdAt: DateTimeOffset

- BetResponse:
  - id: Guid
  - userId: Guid
  - matchId: Guid
  - prediction: string
  - editedAt: DateTimeOffset
