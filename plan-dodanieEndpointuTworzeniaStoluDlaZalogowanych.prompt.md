Przejmuję zadanie i przygotuję precyzyjny, praktyczny plan wdrożenia nowego endpointu POST /api/tables/create dla zalogowanych użytkowników w projekcie Bagman.

## Plan: Autoryzowany endpoint tworzenia stołu

Dodamy nowy endpoint POST /api/tables/create wymagający JWT, który pobierze userId z tokenu, utworzy stół przypisany do zalogowanego użytkownika, ustawi go jako admina, zwróci TableResponse, i zachowa dotychczasowy endpoint POST /api/tables dla rejestracji+tworzenia. Plan obejmuje zmiany w Contracts (nowy request), walidator (FluentValidation), kontroler (TablesController), serwis (ITableService + implementacja), dokumentację oraz testy integracyjne (happy path, 401, 422). Uwzględniono edge case’y (duplikat nazwy, limit graczy, brak miejsca, pusta nazwa/hasło) oraz kryteria jakości (build, lint, testy zielone) i harmonogram.

### Steps
1. Dodaj model `AuthorizedCreateTableRequest` w `src/Bagman.Contracts/Models` i aktualizuj `Bagman.Contracts.csproj`.
2. Utwórz walidator `AuthorizedCreateTableRequestValidator` w `src/Bagman.Api/Validators`, reużyj reguł z `MatchValidators.cs`/istniejących validatorów stołu.
3. Rozszerz `ITableService` w `src/Bagman.Domain/Services` o metodę `CreateForUser(userId, request)` i implementację w `src/Bagman.Infrastructure/Services`.
4. Dodaj akcję `[HttpPost("create")]` z `[Authorize]` w `src/Bagman.Api/Controllers/TablesController.cs`, pobierz `userId` z `HttpContext.User`.
5. Ustal przypisanie admina podczas tworzenia (twórca jako admin) w implementacji serwisu; uwzględnij konflikt nazwy i limit.
6. Zaktualizuj mapowanie na `TableResponse` w `src/Bagman.Contracts/Models` lub mapperze w `Infrastructure`.
7. Uzupełnij dokumentację w `docs/api-documentation*.md` o nowy endpoint, autoryzację i odpowiedzi błędów.
8. Dodaj testy integracyjne w `tests/Bagman.IntegrationTests/Controllers/Tables` dla: happy path, 401 bez tokenu, 422 błędne dane, duplikat, limit graczy.
9. Sprawdź RLS/DB ograniczenia w `db/*.sql` i odnotuj brak zmian, chyba że implementacja wymaga dodatkowych pól.
10. Kryteria jakości: pełny build, lint, testy; punkt kontrolny po każdym większym etapie.

### Szczegóły implementacyjne i uzasadnienie

- Contracts:
  - Nowy request `AuthorizedCreateTableRequest` zawiera pola: `name`, opcjonalnie `password` (jeśli model stołu przewiduje), opcjonalne parametry konfiguracyjne stołu (np. `maxPlayers`, `visibility`), zgodne z dotychczasowymi konwencjami w `Bagman.Contracts/Models`.
  - Uzasadnienie: separacja modelu dla autoryzowanego tworzenia od procesu rejestracji (+ tworzenie stołu) w istniejącym `POST /api/tables`.

- Validators (FluentValidation):
  - `AuthorizedCreateTableRequestValidator`: waliduje niepustą `name`, długość i format, `password` jeśli jest wymagane przez reguły (np. minimalna długość), opcjonalne `maxPlayers` w zakresie (np. 2–N), brak niedozwolonych znaków.
  - Edge case’y w walidacji: pusta nazwa/hasło → 422; skrajne długości; whitespace-only.
  - Reużyj wspólnych reguł z istniejących validatorów (np. z `MatchValidators.cs` jeśli są analogiczne) dla spójności.

- Controller:
  - `TablesController`: dodaj `[Authorize]` nad nową akcją `[HttpPost("create")]`.
  - Pobierz `userId` z `HttpContext.User` przez claim (np. `ClaimTypes.NameIdentifier` lub custom `"sub"/"userId"` zgodnie z aktualnym AuthController).
  - Deleguj do serwisu `ITableService.CreateForUser(userId, request)` i zwróć `TableResponse`.
  - Obsłuż błędy: duplikaty (konflikt 409), brak miejsca/limit (422 lub 409 – preferencja: 422 z komunikatem domenowym), walidacja (422), nieautoryzowany (401 automatycznie z [Authorize]).
  - Uzasadnienie: trzymamy logikę domenową w serwisie, kontroler tylko orkiestruje.

- Service:
  - `ITableService`: dodaj metodę do tworzenia stołu dla istniejącego użytkownika; nie zmieniaj istniejącej metody rejestracji+tworzenia stołu.
  - Implementacja w `Bagman.Infrastructure/Services/TablesService.cs` (lub odpowiadającym pliku jeśli użyta jest inna nazwa): 
    - Sprawdź konflikt nazwy (unikalność w repozytorium `ITableRepository`).
    - Utwórz rekord stołu z właścicielem `userId`, ustaw twórcę jako admin (np. rola `Admin` lub flagi w encji).
    - Jeśli istnieje limit graczy, sprawdź bieżącą liczbę członków; przy join od razu dodaj twórcę jako członka admin.
    - Zwróć encję zamapowaną na `TableResponse`.
  - Uzasadnienie: enkapsulacja zasad domeny (admin, limity, unikalność).

- Mapping:
  - Upewnij się, że `TableResponse` zawiera najnowsze pola, i zaktualizuj mapper jeśli brakuje danych (np. `adminUserId`, `playersCount`, `hasPassword`, `name`).
  - Uzasadnienie: zgodność z API i frontem.

- Dokumentacja:
  - `docs/api-documentation-v2.md` oraz `docs/api-documentation.md`: sekcja dla `POST /api/tables/create`:
    - Authorization: Bearer JWT, wymagany.
    - Request body: pola z opisem i walidacją.
    - Responses: 200 (TableResponse), 401, 422 (validation/limit), 409 (duplicate name).
    - Przykładowe zapytanie (bez tokenu → 401), poprawne → 200.
  - Zostaw informację, że `POST /api/tables` pozostaje bez zmian dla rejestracji+tworzenia.

- Testy integracyjne:
  - Happy path: z tokenem, poprawne dane, oczekiwany `TableResponse` z twórcą jako adminem.
  - 401: brak Bearer token → 401.
  - 422: niepoprawne dane (pusta nazwa/hasło jeśli wymagane).
  - Duplikat nazwy: spróbuj utworzyć drugi stół o tej samej nazwie → 409 (lub 422 w zależności od decyzji, preferowany 409 dla konfliktu zasobów).
  - Limit graczy/brak miejsca: ustaw maxPlayers i wypełnij stół do limitu w Arrange, potem próbuj create dla innego stołu? (Jeśli limit dotyczy globalnie per stół, test dotyczy join; dla create: waliduj maxPlayers w zakresie i business rule zgodnie z domeną; jeśli nie dotyczy create, opisz jako not applicable i pomiń w create).
  - Struktura: nowe testy w `tests/Bagman.IntegrationTests/Controllers/Tables` z Arrange/Act/Assert, reużyj `Helpers` i `TestFixtures`.

- Edge cases:
  - Duplikat nazwy stołu → 409 Conflict.
  - Limit graczy: sprawdź zasady — jeśli dotyczy join, nie blokuje create; waliduj `maxPlayers` zakresowo.
  - Brak miejsca: dotyczy join; przy create nie dotyczy, ale twórca zawsze dodany.
  - Pusta nazwa/hasło → 422 Unprocessable Entity.
  - Nazwa z niedozwolonymi znakami → 422.
  - Token bez `userId` claim → 401/403; zalecane 401.

### Further Considerations
1. Status błędów: 409 dla duplikatu nazwy vs 422 — rekomendacja: 409 dla konfliktu zasobu.
2. Claim `userId`: `ClaimTypes.NameIdentifier` czy `sub`? Rekomendacja: spójność z `AuthController`/generatorem JWT.
3. `password` opcjonalne: czy dopuszczamy stoły publiczne? Rekomendacja: opcjonalne, walidacja jeśli obecne.

### Minimalny harmonogram i punkty kontroli
- Dzień 1:
  - Contracts + Validator + Controller action szkic; Punkt kontroli: kompilacja projektu `Bagman.Api` i `Bagman.Contracts`.
- Dzień 2:
  - Service + Repo wywołania + mapping; Punkt kontroli: build całego rozwiązania, uruchomienie lokalne API.
- Dzień 3:
  - Dokumentacja + Testy integracyjne (happy path, 401, 422); Punkt kontroli: zielone testy integracyjne.
- Dzień 4:
  - Edge cases (duplikat 409, limit), refaktory; Punkt kontroli: pełny build, lint, wszystkie testy zielone.

### Kryteria jakości
- Build: brak błędów w całym rozwiązaniu (`Bagman.sln`).
- Lint/Style: zgodność ze standardami projektu (naming, SOLID, Clean Code).
- Testy: zielone dla nowych i istniejących integracyjnych.
- API: zgodne z dokumentacją; `POST /api/tables` niezmieniony.
