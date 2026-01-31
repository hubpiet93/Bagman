# Bagman – Wymagania dla Frontendu (wersja 0.1)

System: prywatne obstawianie wyników meczów piłkarskich w gronie znajomych (turnieje typu EURO / MŚ).  
Ten dokument opisuje **wymagania funkcjonalne i widokowe dla frontendu**, oparte na:

- dostępnych endpointach API (Auth, Tables, Matches, Bets),
- zasadach gry (pula, typowanie, role),
- wcześniejszym dokumencie funkcjonalnym.

---

## 1. Główne moduły frontendu

Frontend powinien dostarczać co najmniej następujące obszary:

1. **Autoryzacja i wybór stołu**
    - Rejestracja / logowanie.
    - Dołączanie do stołu (scenariusz „join + załóż konto”).
    - Wybór stołu, jeśli użytkownik należy do wielu stołów.
    - Wylogowanie.

2. **Panel stołu**
    - Podgląd informacji o stole i jego uczestnikach.
    - Lista meczów z możliwością obstawiania (dla gracza).
    - Akcje admina: zarządzanie meczami, adminami, trybem stołu (w przyszłości).

3. **Widok meczu**
    - Szczegóły meczu, status, godzina rozpoczęcia.
    - Formularz typowania / edycji typu (przed startem meczu).
    - Podgląd typów innych (w wersji 1.0 – jawne, potem tryb „tajemniczy”).

4. **Statystyki i historia**
    - Statystyki użytkownika.
    - Historia meczów i wyników przy danym stole (kto trafił, jak podzielono pulę – logicznie, nawet jeśli API dopiero to dostanie).

---

## 2. Autoryzacja i wybór stołu

### 2.1. Widok: Strona startowa / Landing

**Cel:** Umożliwić użytkownikowi:

- zalogowanie się,
- rejestrację i założenie stołu,
- dołączenie do istniejącego stołu.

#### Wymagane elementy UI

- Przyciski / zakładki:
    - „Zaloguj się”
    - „Załóż stół” (rejestracja + stworzenie stołu)
    - „Dołącz do stołu”

---

### 2.2. Widok: Rejestracja + założenie stołu

**Flow:** Użytkownik zakłada konto i od razu tworzy stół.

#### Formularz

Pola (klient powinien walidować jak najwięcej przed wysłaniem do API):

- Login:
    - wymagany,
    - unikalny (błąd „użytkownik już istnieje” trzeba pokazać na loginie).
- Hasło użytkownika:
    - min. 10 znaków,
    - musi zawierać:
        - wielką literę,
        - cyfrę,
        - znak specjalny.
- Nazwa stołu:
    - wymagane,
    - tekst (np. max 100 znaków – do ustalenia).
- Hasło stołu:
    - min. 10 znaków (zalecenie spójne z hasłem użytkownika; API teraz waliduje min. długość, resztę frontend może dodać).
- Maksymalna liczba graczy:
    - liczba całkowita > 0,
    - UI może zawęzić np. do [2–50].
- Stawka za mecz (w zł):
    - liczba ≥ 0,
    - format np. z dwoma miejscami po przecinku.

#### Wywołania API

1. `POST /api/tables`

   Body:

   ```json
   {
     "UserLogin": "<login>",
     "UserPassword": "<password>",
     "TableName": "<nazwa stołu>",
     "TablePassword": "<hasło stołu>",
     "MaxPlayers": <int>,
     "Stake": <decimal>
   }
   ```

   Odpowiedź 201 zawiera dane stołu.

2. Następnie frontend powinien zalogować użytkownika (opcje):
    - A: Bezpośrednio użyć `POST /api/auth/login` z `Login` / `Password`.
    - B: Jeśli backend kiedyś będzie zwracał tokeny z `POST /api/tables`, można to uprościć – ale z aktualnych snapshotów wynika, że trzeba wywołać login osobno.

#### Obsługa błędów

- 400 – walidacja:
    - Wyświetlić treści z `errors.FieldName[]` przy odpowiednich polach.
- 400 – `User.AlreadyExists`:
    - Komunikat w polu Login: „Użytkownik o podanym loginie już istnieje”.
- 400 – błędne hasło stołu / inne błędy domenowe:
    - Wyświetlić komunikat globalny lub przy odpowiednim polu (wymaga interpretacji `errors`).

---

### 2.3. Widok: Logowanie

**Cel:** Zalogować istniejącego użytkownika i pozwolić mu wybrać stół.

#### Formularz logowania

Pola:

- Login,
- Hasło.

#### API

- `POST /api/auth/login`

  Body:

  ```json
  {
    "Login": "<login>",
    "Password": "<password>"
  }
  ```

  Odpowiedź:

    - accessToken, refreshToken, user (id, login, email).

#### Dalszy krok – wybór stołu

Po poprawnym logowaniu frontend powinien:

1. pobrać stoły użytkownika:

    - `GET /api/tables` z nagłówkiem:

      ```http
      Authorization: Bearer <accessToken>
      ```

2. jeśli:
    - brak stołów → ekran „nie należysz do żadnego stołu” + CTA „Załóż stół” / „Dołącz do stołu”;
    - jeden stół → można automatycznie wejść na panel stołu;
    - wiele stołów → wyświetlić listę do wyboru.

---

### 2.4. Widok: „Dołącz do stołu”

**Cel:** Połączenie rejestracji/logowania z dołączeniem do konkretnego stołu.

#### Formularz

Pola:

- Login (nowy lub istniejący):
    - jeśli login istnieje → traktujemy jako logowanie,
    - jeśli nie istnieje → traktujemy jako rejestrację.
- Hasło użytkownika.
- Nazwa stołu.
- Hasło stołu.

#### API – sugerowany flow

Backend nie ma dedykowanego endpointu „join + register”, więc frontend musi złożyć kilka wywołań:

1. Sprawdzić, czy login istnieje (API tego nie pokazuje – **do ustalenia**).

   Prostsza implementacja frontu (bez introspekcji):

    - próbujemy zawsze:

        1. `POST /api/auth/login`
            - jeśli się uda → przechodzimy do punktu 2 (join).
            - jeśli zwróci `Auth.InvalidCredentials`:
                - traktujemy to jako sygnał:
                    - jeśli użytkownik świadomie zakłada nowe konto → pokaż przycisk „Załóż konto i dołącz do stołu”,
                    - jeśli nie → komunikat „Nieprawidłowy login lub hasło”.

        2. `POST /api/auth/register` (gdy użytkownik wybierze opcję „Załóż konto”).
        3. `POST /api/tables/join`.

2. Dołączenie do stołu:

    - `POST /api/tables/join`

      Body:

      ```json
      {
        "UserLogin": "<login>",
        "UserPassword": "<password>",
        "TableName": "<nazwa stołu>",
        "TablePassword": "<hasło stołu>"
      }
      ```

    - Po sukcesie warto zalogować użytkownika (jeśli jeszcze nie jest zalogowany).

#### Obsługa błędów

- `Table.Full` – komunikat: „Stół jest pełny”.
- Niewłaściwe hasło stołu / konta:
    - błędy walidacyjne (400/401) zwrócone przez Auth lub Tables.
- UX: zapewnić jasny feedback, czy problem jest z loginem/hasłem konta czy stołem.

---

### 2.5. Wylogowanie i przełączanie stołu

#### Wylogowanie

- Widoczne jako:
    - przycisk „Wyloguj” w nawigacji.
- API:
    - `POST /api/auth/logout` z `RefreshToken` (token można trzymać w pamięci / localStorage):
        - backend i tak zwykle zwraca `200 OK`, nawet dla nieistniejących refresh tokenów.
- Frontend:
    - po wylogowaniu:
        - usuwa tokeny z pamięci,
        - redirect na stronę startową.

#### Przełączanie stołu

- W obrębie zalogowanej sesji:
    - cały czas mamy `accessToken` powiązany z użytkownikiem, nie ze stołem.
- UX:
    - w górnej belce dropdown „Aktualny stół: [nazwa]” → po kliknięciu lista stołów:
        - dane z `GET /api/tables`.
    - wybór stołu zmienia kontekst:
        - panel stołu,
        - lista meczów,
        - zakłady, historia etc.

---

## 3. Panel stołu

### 3.1. Widok: Dashboard stołu

**Cel:** Jeden „centralny” ekran pokazujący:

- informacje o stole,
- uczestników,
- zbiorczy status (np. obecna pula – do wprowadzenia po stronie backendu),
- listę najbliższych meczów do obstawienia.

#### Dane potrzebne z API

- `GET /api/tables/{tableId}`

  Zwraca:

  ```json
  {
    "id": "Guid",
    "name": "string",
    "maxPlayers": 5,
    "stake": 50.0,
    "createdAt": "DateTimeOffset_3",
    "members": [
      {
        "userId": "Guid_1",
        "login": "string",
        "isAdmin": true,
        "joinedAt": "DateTimeOffset_4"
      }
    ]
  }
  ```

- `GET /api/tables/{tableId}/matches` – takiego endpointu nie ma w snapshotach, ale frontend będzie go potrzebował, więc:
    - rekomendacja: zaplanować go w backendzie.
    - na razie front może bazować na tym, że dla większości operacji używamy `MatchesController` z parametrami `tableId`.

#### Elementy UI

- Nagłówek stołu:
    - nazwa,
    - liczba graczy / maksymalna (np. „5 / 10 graczy”),
    - stawka za mecz,
    - informacja, czy aktualny użytkownik jest adminem.
- Lista uczestników:
    - login,
    - ikona admina (np. gwiazdka / korona).
- Sekcja „Najbliższe mecze”:
    - data i godzina,
    - pary krajów (z flagami – po stronie frontu / integracji z listą państw),
    - status (np. „do obstawienia”, „w trakcie”, „zakończony”),
    - dla przyszłych meczów:
        - przycisk „Obstaw” lub „Edytuj typ” (jeśli użytkownik już obstawił).

---

### 3.2. Widok: Zarządzanie administratorami (tylko admin)

**Cel:** Admin stołu może nadać lub odebrać rolę admina.

#### Elementy UI

- Lista graczy w stole:
    - login,
    - status: admin / zwykły,
    - przyciski:
        - „Nadaj admina” (dla zwykłych),
        - „Odbierz admina” (dla adminów – z wyjątkiem twórcy? Do ustalenia).

#### API

- Nadaj admina:

    - `POST /api/tables/{tableId}/admins`

      Body:

      ```json
      {
        "UserId": "<Guid użytkownika>"
      }
      ```

- Odbierz admina:

    - `DELETE /api/tables/{tableId}/admins/{userId}`

#### Obsługa błędów

- `403 Forbidden` z `Table.NotAdmin` → pokazać komunikat „Nie masz uprawnień do wykonania tej czynności”.

---

## 4. Zarządzanie meczami (admin)

### 4.1. Widok: Lista meczów (dla admina)

**Cel:** Lista wszystkich meczów przy danym stole z możliwością:

- dodania meczu,
- edycji (przed startem),
- usunięcia (przed startem),
- wpisania wyniku (po zakończeniu).

#### Elementy UI

- Tabela:
    - kolumny:
        - data/godzina,
        - kraj1,
        - kraj2,
        - status (scheduled/started/finished – do rozszerzenia),
        - wynik (jeśli istnieje),
        - akcje (edytuj, usuń, wpisz wynik).
- Przyciski:
    - „Dodaj mecz”.

---

### 4.2. Widok: Formularz dodawania meczu

#### Formularz

Pola:

- Kraj gospodarzy (`Country1`),
- Kraj gości (`Country2`):
    - UI: autocomplete z listy państw (frontend powinien zarządzać listą; backend waliduje),
- Data i godzina meczu (`MatchDateTime`):
    - picker daty + czasu,
    - walidacja na froncie: data przyszła.

#### API

- `POST /api/tables/{tableId}/matches`

  Body:

  ```json
  {
    "Country1": "Poland",
    "Country2": "Germany",
    "MatchDateTime": "2026-05-01T18:00:00Z"
  }
  ```

#### Obsługa błędów

- 400 – `Drużyny muszą być różne` → komunikat przy wyborze krajów.
- 400 – `Data meczu musi być w przyszłości` → komunikat przy dacie.

---

### 4.3. Widok: Edycja/Usuwanie meczu

#### Edycja

- Formularz identyczny jak przy tworzeniu.
- `PUT /api/tables/{tableId}/matches/{matchId}`

  Body:

  ```json
  {
    "Country1": "...",
    "Country2": "...",
    "MatchDateTime": "..."
  }
  ```

- Dostępne, gdy:
    - mecz się jeszcze nie rozpoczął (`started == false` – frontend może to odczytać z `GET /match` lub z listy).

#### Usuwanie

- `DELETE /api/tables/{tableId}/matches/{matchId}`
- Potwierdzenie w UI: „Czy na pewno chcesz usunąć mecz? Wszystkie typy zostaną anulowane.”  
  (logika finansowa po stronie backendu – czy i jak zwraca środki – do doprecyzowania, ale frontend może komunikować „mecz anulowany, pula przeliczona”).

---

### 4.4. Widok: Wpisywanie wyniku meczu

#### Formularz

- Pola:
    - `Score1`,
    - `Score2`:
        - numeryczne (np. input number),
        - w API to stringi, ale frontend może wysyłać stringową wersję liczb.

- Dostępne, gdy:
    - mecz już się zakończył (data w przeszłości),
    - wynik jeszcze nie wpisany.

#### API

- `PUT /api/tables/{tableId}/matches/{matchId}/result`

  Body:

  ```json
  {
    "Score1": "2",
    "Score2": "1"
  }
  ```

- Odpowiedź 200:

  ```json
  {
    "message": "Match result set successfully"
  }
  ```

#### Po zapisaniu wyniku

- UI:
    - odświeża listę meczów,
    - podświetla zwycięzców (po integracji z danymi o rozliczeniu puli – to wymaga rozszerzeń API),
    - w historii meczu można pokazać:
        - kto trafił,
        - jaka kwota została przyznana.

---

## 5. Obstawianie meczów (wszyscy gracze)

### 5.1. Widok: Szczegóły meczu

**Cel:** Jeden ekran na mecz, zawierający:

- metadane meczu (kraj1 vs kraj2, czas, status),
- aktualną pulę (jeśli będzie wystawiona przez API),
- formularz typowania,
- listę typów innych graczy (wersja 1.0 – jawna),
- wyniki po zakończeniu meczu.

#### Dane z API

- `GET /api/tables/{tableId}/matches/{matchId}` → szczegóły meczu.
- `GET /api/tables/{tableId}/matches/{matchId}/bets/my` → typ aktualnego użytkownika (lub 404).
- Dodatkowy endpoint (do ustalenia z backendem):
    - np. `GET /api/tables/{tableId}/matches/{matchId}/bets` → typy wszystkich graczy w meczu (potrzebne dla sekcji „jakie wyniki obstawili inni”).

---

### 5.2. Formularz typowania

#### Zachowanie

- Dostępne, gdy:
    - `started == false` (czas meczu w przyszłości),
    - użytkownik należy do stołu.
- Dwa główne warianty:
    - UI z trzema polami:
        - gole gospodarzy, gole gości (numeryczne),
        - checkbox lub radio „remis (X)”.
    - albo:
        - jedno pole tekstowe `Prediction` – ale lepsze UX to rozdzielone gole + automatyczny format.

#### API

- Zapis / edycja typu:

    - `POST /api/tables/{tableId}/matches/{matchId}/bets`

      Body:

      ```json
      {
        "Prediction": "2:1"
      }
      ```

- Po stronie frontu:
    - jeśli użytkownik już obstawił:
        - formularz jest wypełniony istniejącym typem (`prediction` z `BetResponse`),
        - zapisanie nadpisuje poprzedni typ (aktualizuje zakład – API zawsze zwraca 201).

#### Obsługa błędów

- 400 – walidacja `Prediction`:

  ```json
  {
    "errors": {
      "Prediction": [
        "Typowanie nie może mieć więcej niż 10 znaków",
        "Typowanie musi być w formacie 'wynik1:wynik2' lub 'X' (remis)"
      ]
    }
  }
  ```

- Frontend:
    - wyświetla komunikaty przy polu „typ”,
    - dodatkowo waliduje format po swojej stronie (wzorzec `^\d+:\d+$` albo `"X"`).

---

### 5.3. Brak obstawienia vs. udział w meczu

Zasada gry:

- Nawet jeśli użytkownik **nie obstawi**:
    - płaci stawkę (wchodzi w pulę),
    - **nie ma szansy na wygraną** (brak zakładu → nie uczestniczy w podziale puli).

Wymaganie UX:

- W widoku meczu:
    - wyraźny status:
        - „Nie obstawiłeś jeszcze”,
        - „Twój typ: 2:1”,
    - tooltip/komunikat:
        - „Brak typowania nie zwalnia z opłaty – bierzesz udział w meczu, ale nie możesz wygrać”.

---

### 5.4. Podgląd typów innych graczy

Wersja 1.0 (jawne typy):

- Widok listy uczestników meczu:

    - login,
    - obstawiony wynik (np. `2:1`),
    - po meczu – dodatkowe oznaczenie:
        - trafił / nie trafił.

- Dane będą wymagać osobnego endpointu (np. `GET /bets` dla meczu).

Wersja 2.0 („tajemniczy typer” – flaga `isSecretMode` na stole):

- Jeśli `isSecretMode == true`:
    - przed meczem:
        - zamiast wyniku pokazujemy np. „obstawione” / „nie obstawione”,
    - po meczu:
        - odsłaniamy faktyczne typy.

---

## 6. Statystyki i historia

### 6.1. Statystyki użytkownika

Widok „Moje statystyki” powinien pokazywać:

- Liczbę meczów, w których brał udział.
- Liczbę meczów, które obstawił.
- Liczbę wygranych (z trafionym wynikiem i wypłatą).
- Łączną wygraną kwotę.
- Ewentualnie sumę wpłat (stawka × liczba meczów stołów, w których brał udział).

API jeszcze nie jest widoczne w snapshotach, ale:

- w bazie występuje tabela `user_stats` – backend już może liczyć.
- frontend:
    - zakłada, że będzie endpoint np. `GET /api/users/me/stats` albo `GET /api/tables/{tableId}/stats/my`.

---

### 6.2. Historia meczów i wyników

Dwa poziomy:

1. **Per stół:**
    - lista wszystkich meczów:
        - data/godzina,
        - wynik,
        - ilu graczy trafiło,
        - przycisk „szczegóły”.

2. **Szczegóły meczu w historii:**
    - wynik końcowy,
    - pula i jej rozliczenie (ile wynosiła, ilu zwycięzców, ile każdy dostał),
    - lista uczestników:
        - ich typy,
        - info, czy trafili,
        - czy dostali wypłatę i w jakiej wysokości.

Te dane wymagają dodatkowych endpointów po stronie backendu; z perspektywy frontu:

- iterować z backendem nad JSON-ową strukturą historii,
- zadbać o klarowną prezentację w UI (filtry po fazie turnieju, szukanie po drużynach itp. – późniejsze rozszerzenia).

---

## 7. Uwagi techniczne dla frontendu

- **Autoryzacja:**
    - standardowy schemat Bearer JWT:
        - `Authorization: Bearer <accessToken>`.
    - odświeżanie tokena:
        - `POST /api/auth/refresh` z `RefreshToken`.
    - przy wylogowaniu:
        - `POST /api/auth/logout`.

- **Obsługa błędów:**
    - Standaryzować parsing:
        - błędy walidacyjne:
            - `errors` jako mapa `FieldName -> [komunikaty]`,
        - błędy domenowe:
            - `errors` jako lista `{ errorCode / code, description }`.
    - Frontend powinien:
        - mapować je na przyjazne komunikaty przy polach lub globalne notyfikacje.

- **Czas i strefy:**
    - `MatchDateTime` jest `DateTimeOffset`:
        - frontend powinien używać strefy lokalnej użytkownika przy prezentacji,
        - ale przy obliczaniu „czy mecz już się zaczął” opiera się na informacjach z backendu (`started` flag) lub na ustalonej strefie (np. UTC).

- **Walidacja po stronie klienta:**
    - powinna odzwierciedlać walidację backendu:
        - hasła,
        - format `Prediction`,
        - pozytywne stawki,
        - różne kraje w meczu,
        - przyszła data meczu.

- **Stan aplikacji:**
    - Z punktu widzenia frontendu kluczowe jest utrzymywanie w stanie:
        - zalogowany użytkownik (`user`),
        - aktualnie wybrany stół (`currentTable`),
        - lista stołów użytkownika (`tables`),
        - mecze danego stołu (`matches`),
        - zakłady użytkownika (`myBets`),
        - ewentualnie: globalne statystyki / historia.

---
