# RAPORT ANOMALII SNAPSZOTÓW TESTÓW INTEGRACYJNYCH

## STRESZCZENIE WYKONAWCZE

Przeanalizowano wszystkie snapshoty z 5 kontrolerów. Znaleziono **9 anomalii** z niezgodnymi status codami HTTP:
- **AuthController**: 0 anomalii
- **BetsController**: 4 anomalie (krytyczne - błędy w setup'ie)
- **EventTypesController**: 0 anomalii
- **MatchesController**: 0 anomalii
- **TablesController**: 5 anomalii (krytyczne + poważne)

---

## 1. AuthController

## 1. AuthController

Brak zidentyfikowanych anomalii w snapszotach AuthController.

---

## 2. BetsController

### Anomalia 2.1: DeleteBet_BeforeMatchStarted_ReturnsOk - Setup zwraca 401 zamiast 201

**Test**: `DeleteBet_BeforeMatchStarted_ReturnsOk`  
**Plik snapshotu**: `BetsControllerTests.DeleteBet_BeforeMatchStarted_ReturnsOk.verified.json`  
**Problem**: Setup testu - POST do `/api/tables` zwraca `Unauthorized 401` zamiast `Created 201`  
**Co jest**: ResponseStatus = `Unauthorized 401`  
**Co powinno być**: ResponseStatus = `Created 201` (tabela powinna zostać utworzona)  
**Ścieżka w pliku**: Pierwsza lub druga operacja w tablicy (setup)  
**Wpływ**: Krytyczne - test nie może się wykonać, bo tabela nie została utworzona. Wskazuje na problem z autoryzacją w setup'ie testu.

### Anomalia 2.2: GetUserBet_WithExistingBet_ReturnsOkWithBetResponse - Setup zwraca 401 zamiast 201

**Test**: `GetUserBet_WithExistingBet_ReturnsOkWithBetResponse`  
**Plik snapshotu**: `BetsControllerTests.GetUserBet_WithExistingBet_ReturnsOkWithBetResponse.verified.json`  
**Problem**: Setup testu - POST do `/api/tables` zwraca `Unauthorized 401` zamiast `Created 201`  
**Co jest**: ResponseStatus = `Unauthorized 401`  
**Co powinno być**: ResponseStatus = `Created 201`  
**Ścieżka w pliku**: Setup - pierwsza operacja  
**Wpływ**: Krytyczne - test nie może się wykonać z powodu błędu autoryzacji w setup'ie.

### Anomalia 2.3: GetUserBet_WithoutBet_ReturnsNotFound - Setup zwraca 401 zamiast 201

**Test**: `GetUserBet_WithoutBet_ReturnsNotFound`  
**Plik snapshotu**: `BetsControllerTests.GetUserBet_WithoutBet_ReturnsNotFound.verified.json`  
**Problem**: Setup testu - POST do `/api/tables` zwraca `Unauthorized 401` zamiast `Created 201`  
**Co jest**: ResponseStatus = `Unauthorized 401`  
**Co powinno być**: ResponseStatus = `Created 201`  
**Ścieżka w pliku**: Setup - pierwsza operacja  
**Wpływ**: Krytyczne - błąd autoryzacji uniemożliwia stworzenie tabeli, warunek wstępny testu nie spełniony.

### Anomalia 2.4: PlaceBet_MultipleUsersSameBet_BothSucceed - Setup zwraca 401 zamiast 201

**Test**: `PlaceBet_MultipleUsersSameBet_BothSucceed`  
**Plik snapshotu**: `BetsControllerTests.PlaceBet_MultipleUsersSameBet_BothSucceed.verified.json`  
**Problem**: Setup testu - POST do `/api/tables` zwraca `Unauthorized 401` zamiast `Created 201`  
**Co jest**: ResponseStatus = `Unauthorized 401`  
**Co powinno być**: ResponseStatus = `Created 201`  
**Ścieżka w pliku**: Setup - pierwsza operacja  
**Wpływ**: Krytyczne - błąd autoryzacji w setup'ie uniemożliwia całkowite wykonanie testu.

---

## 3. EventTypesController

Brak zidentyfikowanych anomalii w snapshotach EventTypesController.

---

## 4. MatchesController

Brak zidentyfikowanych anomalii w snapshotach MatchesController.

---

## 5. TablesController

### Anomalia 5.1: JoinTable_WithValidRequest_ReturnsOkWithTableResponse - Zwraca 403 zamiast zwrócenia 200 OK

**Test**: `JoinTable_WithValidRequest_ReturnsOkWithTableResponse`  
**Plik snapshotu**: `TablesControllerTests.JoinTable_WithValidRequest_ReturnsOkWithTableResponse.verified.json`  
**Problem**: Operacja POST do `/api/tables/join` zwraca `Forbidden 403` zamiast `OK 200`  
**Co jest**: ResponseStatus = `Forbidden 403`  
**Co powinno być**: ResponseStatus = `OK 200`  
**Ścieżka w pliku**: Ostatnia operacja w tablicy  
**Wpływ**: Poważne - endpoint odmawia dostępu do dołączenia do tabeli, pomimo że użytkownik powinien mieć uprawnienia.

### Anomalia 5.2: JoinTable_WithWrongPassword_ReturnsBadRequest - Zwraca 403 zamiast 400

**Test**: `JoinTable_WithWrongPassword_ReturnsBadRequest`  
**Plik snapshotu**: `TablesControllerTests.JoinTable_WithWrongPassword_ReturnsBadRequest.verified.json`  
**Problem**: Operacja POST do `/api/tables/join` z błędnym hasłem zwraca `Forbidden 403` zamiast `BadRequest 400`  
**Co jest**: ResponseStatus = `Forbidden 403`  
**Co powinno być**: ResponseStatus = `BadRequest 400` (błąd walidacji, a nie brak dostępu)  
**Ścieżka w pliku**: Ostatnia operacja w tablicy  
**Wpływ**: Poważne - niezgodna semantyka HTTP. 403 oznacza brak uprawnień, a 400 oznacza błąd walidacji.

### Anomalia 5.3: JoinTable_WithFullTable_ReturnsBadRequest - Zwraca 403 zamiast 400

**Test**: `JoinTable_WithFullTable_ReturnsBadRequest`  
**Plik snapshotu**: `TablesControllerTests.JoinTable_WithFullTable_ReturnsBadRequest.verified.json`  
**Problem**: Operacja POST do `/api/tables/join` dla pełnej tabeli zwraca `Forbidden 403` zamiast `BadRequest 400`  
**Co jest**: ResponseStatus = `Forbidden 403`  
**Co powinno być**: ResponseStatus = `BadRequest 400` (tabela jest pełna - to walidacja, nie brak dostępu)  
**Ścieżka w pliku**: Ostatnia operacja w tablicy  
**Wpływ**: Poważne - błędna semantyka HTTP - powinno być 400 dla błędu biznesowego (tabela pełna).

### Anomalia 5.4: GetTableDashboard_IncludesTableInfo_Members_AndEmptyData - Setup zwraca 401 zamiast 201

**Test**: `GetTableDashboard_IncludesTableInfo_Members_AndEmptyData`  
**Plik snapshotu**: `TablesControllerTests.GetTableDashboard_IncludesTableInfo_Members_AndEmptyData.verified.json`  
**Problem**: Setup testu - POST do `/api/tables` zwraca `Unauthorized 401` zamiast `Created 201`  
**Co jest**: ResponseStatus = `Unauthorized 401`  
**Co powinno być**: ResponseStatus = `Created 201`  
**Ścieżka w pliku**: Setup - pierwsza operacja  
**Wpływ**: Krytyczne - błąd autoryzacji w setup'ie uniemożliwia stworzenie tabeli, test nie może się wykonać.

### Anomalia 5.5: GetTableDashboard_WithValidMember_ReturnsOkWithDashboardData - Setup zwraca 401 zamiast 201

**Test**: `GetTableDashboard_WithValidMember_ReturnsOkWithDashboardData`  
**Plik snapshotu**: `TablesControllerTests.GetTableDashboard_WithValidMember_ReturnsOkWithDashboardData.verified.json`  
**Problem**: Setup testu - POST do `/api/tables` zwraca `Unauthorized 401` zamiast `Created 201`  
**Co jest**: ResponseStatus = `Unauthorized 401`  
**Co powinno być**: ResponseStatus = `Created 201`  
**Ścieżka w pliku**: Setup - pierwsza operacja  
**Wpływ**: Krytyczne - błąd autoryzacji w setup'ie uniemożliwia stworzenie tabeli dla testu.

---

## PODSUMOWANIE PO KONTROLERACH

| Kontroler | Liczba anomalii | Typ problemu | Priorytet |
|-----------|-----------------|---|---|
| AuthController | 0 | ✓ Brak anomalii | - |
| BetsController | 4 | 401 Unauthorized w setup | KRYTYCZNE |
| EventTypesController | 0 | ✓ Brak anomalii | - |
| MatchesController | 0 | ✓ Brak anomalii | - |
| TablesController | 5 | 403 Forbidden zamiast 400 BadRequest, 401 w setup | KRYTYCZNE + POWAŻNE |
| **RAZEM** | **9** | | |

---

## REKOMENDACJE PRIORYTETOWE

### Krytyczne (Blokuje testy - problem z autoryzacją):

1. **Anomalie 2.1, 2.2, 2.3, 2.4** - BetsController
   - Setup testu nie może utworzyć tabeli (401 Unauthorized zamiast 201)
   - **Przyczyna**: Błąd autoryzacji w setup'ie - token nie jest prawidłowo przesyłany lub jest invalid
   - **Wpływ**: 4 testy są całkowicie nie funkcjonalne
   - **Akcja**: Sprawdzić middleware autoryzacji, generowanie tokenów w setup'ie

2. **Anomalia 5.4, 5.5** - TablesController
   - Setup testu zwraca 401 Unauthorized zamiast 201 Created
   - **Przyczyna**: Taki sam problem jak w BetsController
   - **Wpływ**: 2 testy nie mogą się wykonać
   - **Akcja**: Takie same naprawa autoryzacji

### Poważne (Błąd semantyki HTTP):

3. **Anomalie 5.1, 5.2, 5.3** - TablesController
   - POST `/api/tables/join` zwraca 403 Forbidden zamiast 400 BadRequest
   - **Przyczyna**: Endpoint traktuje błędy walidacji (złe hasło, pełna tabela) jako brak uprawnień
   - **Wpływ**: Klient dostaje niewłaściwe kody statusów
   - **Akcja**: Zmienić logikę weryfikacji hasła - zwracać 400 zamiast 403

---

## ANALIZA PROBLEMU GŁÓWNEGO

**Wspólny mianownik**: Błędy autoryzacji (401) w setup'ie testów

4 z 9 anomalii (anomalie 2.1-2.4, 5.4-5.5) wskazuje na ten sam problem:
- **Operacje POST do `/api/tables` w setup'ie zwracają 401 Unauthorized**
- To sugeruje, że token nie jest prawidłowo generowany lub przesyłany w setup'ie testu
- Mogą to powodować:
  - Problem z generowaniem tokenu (JWT, refresh token) w setup'ie
  - Brakujący Authorization header
  - Token wygasł lub jest invalid
  - Problem z middleware'em autoryzacji

**Rekomendacja**: Najpierw naprawić problem autoryzacji w setup'ie, co powinno rozwiązać 6 anomalii naraz.

---

## METODOLOGIA ANALIZY

Raport przygotowano poprzez:
1. Przeanalizowanie wszystkich 52 plików `.verified.json` z folderów kontrolerów
2. **Fokus na niezgodności status codes HTTP** - porównanie ResponseStatus z oczekiwanym statusem wynika z nazwy testu
3. Wyszukanie testów gdzie ResponseStatus nie odpowiada semantyce nazwy testu
4. Identyfikacja testów zwracających błędy (4xx/5xx) w niezaplanowanych miejscach

---

## DATA GENERACJI RAPORTU

- **Data analizy**: 2026-02-05
- **Branch**: more-ddd
- **Liczba przeanalizowanych snapshotów**: 52
- **Liczba znalezionych anomalii status codes**: 9 (poprzednio 3)
- **Aktualizacja**: 2026-02-05 - pełna ponowna analiza
- **Nowe odkrycia**: 6 dodatkowych anomalii w setup'ie testów (401 Unauthorized)

---

## GOTOWY PROMPT DO PONOWNEJ ANALIZY

Aby powtórzyć analizę snapshotów pod kątem niezgodności status codes, użyj poniższego promptu:

```
Przeanalizuj wszystkie snapshoty testów integracyjnych w projekcie Bagman i wyszukaj anomalie w status codes HTTP.

Zadanie:
1. Przeczytaj WSZYSTKIE pliki .json (52 snapshoty) z folderów:
   - tests/Bagman.IntegrationTests/Controllers/Endpoints/AuthController/
   - tests/Bagman.IntegrationTests/Controllers/Endpoints/BetsController/
   - tests/Bagman.IntegrationTests/Controllers/Endpoints/EventTypesController/
   - tests/Bagman.IntegrationTests/Controllers/Endpoints/MatchesController/
   - tests/Bagman.IntegrationTests/Controllers/Endpoints/TablesController/

2. Dla każdego snapshotu przeanalizuj:
   - Nazwę testu (element [0] w JSON)
   - ResponseStatus każdej operacji HTTP
   - Czy ResponseStatus zgadza się z oczekiwanym statusem wynika z nazwy testu
   - Przykład: test "Logout_WithInvalidRefreshToken_ReturnsBadRequest" powinien zwrócić 400, a nie 200

3. Wyszukaj TYLKO anomalie gdzie:
   - Nazwa testu sugeruje jeden status code (np. "ReturnsBadRequest" = 400)
   - Ale ResponseStatus to zupełnie inny kod (np. 200, 409 itd)
   - Ignoruj anomalie w formatach, headerach czy strukturze danych - TYLKO status codes

4. Wygeneruj raport markdown zawierający:
   - Sekcja na każdy kontroler
   - Pod każdym kontrolerem TYLKO anomalie status codes
   - Dla każdej anomalii: nazwa testu, plik snapshotu, co jest (ResponseStatus) vs co powinno być (oczekiwany status)
   - Podsumowanie tabelowe: liczba anomalii per kontroler
   - Rekomendacje priorytetowe (krytyczne na górze)

Zwróć mi pełną zawartość raportu markdown (bez barier kodowych) gotową do wklejenia do pliku.
```

### Jak użyć promptu:

1. Otwórz VS Code i uruchom Command Palette (`Cmd+Shift+P` na Mac)
2. Szukaj opcji do wysłania do subagenta
3. Skopiuj powyższy prompt dokładnie tak jak jest
4. Czekaj na raport z anomaliami status codes

### Alternatywnie:

Możesz też samodzielnie przechodzić przez snapshoty w EditorContexcie i sprawdzać status codes manualnie, szukając niezgodności między nazwą testu a ResponseStatus w JSON.
