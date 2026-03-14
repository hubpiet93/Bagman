# RAPORT ANOMALII STATUS CODES HTTP - TESTY INTEGRACYJNE BAGMAN

## PODSUMOWANIE WYKONANEJ ANALIZY

**Data analizy:** 7 lutego 2026  
**Liczba przeanalizowanych snapshotów:** 57  
**Liczba znalezionych anomalii:** 0

✅ **Status: WSZYSTKIE SNAPSHOTY SĄ POPRAWNE**

---

## INFORMACJA O ZMIANACH OD POPRZEDNIEJ ANALIZY

### Poprzednia analiza (5 lutego 2026):

- **Liczba przeanalizowanych snapshotów:** 52
- **Liczba znalezionych anomalii:** 9
- **Problemy:** 6 błędów autoryzacji (401) w setup testów, 3 błędy semantyki HTTP (403 zamiast 400)

### Bieżąca analiza (7 lutego 2026):

- **Liczba przeanalizowanych snapshotów:** 57
- **Liczba znalezionych anomalii:** 0
- **Wynik:** 100% zgodność status codes HTTP

**Wniosek:** Wszystkie wcześnie zidentyfikowane anomalie zostały naprawione! 🎉

---

## SZCZEGÓŁOWA ANALIZA PO KONTROLERACH

### 1. AuthController (12 testów)

| Lp. | Test                                                       | Expected Status | Actual Status  | Status |
|-----|------------------------------------------------------------|-----------------|----------------|--------|
| 1   | FullAuthenticationFlow_RegisterLoginRefreshLogout_Succeeds | 200             | 200 OK         | ✅ OK   |
| 2   | Login_WithInvalidPassword_ReturnsBadRequest                | 400             | 400 BadRequest | ✅ OK   |
| 3   | Login_WithNonexistentUser_ReturnsBadRequest                | 400             | 400 BadRequest | ✅ OK   |
| 4   | Login_WithValidCredentials_ReturnsOkWithAuthResponse       | 200             | 200 OK         | ✅ OK   |
| 5   | Logout_WithInvalidRefreshToken_ReturnsBadRequest           | 400             | 400 BadRequest | ✅ OK   |
| 6   | Logout_WithValidRefreshToken_ReturnsOk                     | 200             | 200 OK         | ✅ OK   |
| 7   | Refresh_WithInvalidRefreshToken_ReturnsBadRequest          | 400             | 400 BadRequest | ✅ OK   |
| 8   | Refresh_WithValidRefreshToken_ReturnsOkWithNewTokens       | 200             | 200 OK         | ✅ OK   |
| 9   | Register_WithDuplicateLogin_ReturnsBadRequest              | 400             | 400 BadRequest | ✅ OK   |
| 10  | Register_WithInvalidEmail_ReturnsBadRequest                | 400             | 400 BadRequest | ✅ OK   |
| 11  | Register_WithValidRequest_ReturnsOkWithAuthResponse        | 200             | 200 OK         | ✅ OK   |
| 12  | Register_WithWeakPassword_ReturnsBadRequest                | 400             | 400 BadRequest | ✅ OK   |

**Podsumowanie:** ✅ 12/12 testów - wszystkie poprawne

---

### 2. BetsController (9 testów)

| Lp. | Test                                                       | Expected Status | Actual Status  | Status |
|-----|------------------------------------------------------------|-----------------|----------------|--------|
| 1   | DeleteBet_BeforeMatchStarted_ReturnsOk                     | 200             | 200 OK         | ✅ OK   |
| 2   | DeleteBet_WithoutPlacedBet_ReturnsNotFound                 | 404             | 404 NotFound   | ✅ OK   |
| 3   | GetUserBet_WithExistingBet_ReturnsOkWithBetResponse        | 200             | 200 OK         | ✅ OK   |
| 4   | GetUserBet_WithoutBet_ReturnsNotFound                      | 404             | 404 NotFound   | ✅ OK   |
| 5   | PlaceBet_MultipleUsersSameBet_BothSucceed                  | 201             | 201 Created    | ✅ OK   |
| 6   | PlaceBet_UpdateExistingBet_ReturnsOkWithUpdatedPrediction  | 200             | 200 OK         | ✅ OK   |
| 7   | PlaceBet_WithDrawPrediction_ReturnsCreated                 | 201             | 201 Created    | ✅ OK   |
| 8   | PlaceBet_WithInvalidFormat_ReturnsBadRequest               | 400             | 400 BadRequest | ✅ OK   |
| 9   | PlaceBet_WithValidPrediction_ReturnsCreatedWithBetResponse | 201             | 201 Created    | ✅ OK   |

**Podsumowanie:** ✅ 9/9 testów - wszystkie poprawne

---

### 3. EventTypesController (7 testów)

| Lp. | Test                                                                    | Expected Status | Actual Status | Status |
|-----|-------------------------------------------------------------------------|-----------------|---------------|--------|
| 1   | CreateEventType_AsRegularUser_ReturnsForbidden                          | 403             | 403 Forbidden | ✅ OK   |
| 2   | CreateEventType_AsSuperAdmin_ReturnsCreatedWithEventType                | 201             | 201 Created   | ✅ OK   |
| 3   | CreateEventType_WithDuplicateCode_ReturnsConflict                       | 409             | 409 Conflict  | ✅ OK   |
| 4   | DeactivateEventType_AsRegularUser_ReturnsForbidden                      | 403             | 403 Forbidden | ✅ OK   |
| 5   | DeactivateEventType_AsSuperAdmin_ReturnsOkWithDeactivatedEventType      | 200             | 200 OK        | ✅ OK   |
| 6   | GetActiveEventTypes_WithoutAuthentication_ReturnsOkWithActiveEventTypes | 200             | 200 OK        | ✅ OK   |
| 7   | UpdateEventType_AsSuperAdmin_ReturnsOkWithUpdatedEventType              | 200             | 200 OK        | ✅ OK   |

**Podsumowanie:** ✅ 7/7 testów - wszystkie poprawne

---

### 4. MatchesController (3 testy)

| Lp. | Test                                            | Expected Status | Actual Status | Status |
|-----|-------------------------------------------------|-----------------|---------------|--------|
| 1   | GetMatch_StartedFalse_ForFutureDateTime         | 200             | 200 OK        | ✅ OK   |
| 2   | GetMatch_StartedTrue_ForPastDateTime            | 200             | 200 OK        | ✅ OK   |
| 3   | GetMatch_WithValidId_ReturnsOkWithMatchResponse | 200             | 200 OK        | ✅ OK   |

**Podsumowanie:** ✅ 3/3 testy - wszystkie poprawne

---

### 5. TablesController (27 testów)

| Lp. | Test                                                                         | Expected Status | Actual Status    | Status |
|-----|------------------------------------------------------------------------------|-----------------|------------------|--------|
| 1   | AuthorizedCreateTable_WithDuplicateName_ReturnsConflict409                   | 409             | 409 Conflict     | ✅ OK   |
| 2   | AuthorizedCreateTable_WithInvalidData_ReturnsBadRequest                      | 400             | 400 BadRequest   | ✅ OK   |
| 3   | AuthorizedCreateTable_WithValidTokenAndRequest_ReturnsCreated                | 201             | 201 Created      | ✅ OK   |
| 4   | AuthorizedCreateTable_WithoutToken_ReturnsUnauthorized                       | 401             | 401 Unauthorized | ✅ OK   |
| 5   | CreateTable_WithInvalidTableName_ReturnsBadRequest                           | 400             | 400 BadRequest   | ✅ OK   |
| 6   | CreateTable_WithNegativeStake_ReturnsBadRequest                              | 400             | 400 BadRequest   | ✅ OK   |
| 7   | CreateTable_WithValidRequest_ReturnsCreatedWithTableResponse                 | 201             | 201 Created      | ✅ OK   |
| 8   | GetTableDashboard_AsNonMember_ReturnsForbidden                               | 403             | 403 Forbidden    | ✅ OK   |
| 9   | GetTableDashboard_IncludesTableInfo_Members_AndEmptyData                     | 200             | 200 OK           | ✅ OK   |
| 10  | GetTableDashboard_WithFullData_ReturnsMatchesBetsAndStats                    | 200             | 200 OK           | ✅ OK   |
| 11  | GetTableDashboard_WithNonExistentTable_ReturnsNotFound                       | 404             | 404 NotFound     | ✅ OK   |
| 12  | GetTableDashboard_WithValidMember_ReturnsOkWithDashboardData                 | 200             | 200 OK           | ✅ OK   |
| 13  | GetTableDashboard_WithoutToken_ReturnsUnauthorized                           | 401             | 401 Unauthorized | ✅ OK   |
| 14  | GetTableDetails_WithValidId_ReturnsOkWithTableAndMembers                     | 200             | 200 OK           | ✅ OK   |
| 15  | GetUserTables_WithMultipleTables_ReturnsOkWithTableList                      | 200             | 200 OK           | ✅ OK   |
| 16  | GrantAdmin_ByTableCreator_ReturnsOk                                          | 200             | 200 OK           | ✅ OK   |
| 17  | JoinTableAuthorized_InvalidRequest_ReturnsBadRequest                         | 400             | 400 BadRequest   | ✅ OK   |
| 18  | JoinTableAuthorized_MultipleUsersJoinSameTable_AllSucceed                    | 201             | 201 Created      | ✅ OK   |
| 19  | JoinTableAuthorized_WhenAlreadyMember_ReturnsConflict                        | 409             | 409 Conflict     | ✅ OK   |
| 20  | JoinTableAuthorized_WithFullTable_ReturnsBadRequest                          | 400             | 400 BadRequest   | ✅ OK   |
| 21  | JoinTableAuthorized_WithNonExistentTable_ReturnsNotFound                     | 404             | 404 NotFound     | ✅ OK   |
| 22  | JoinTableAuthorized_WithValidPassword_ReturnsCompleteMemberInfo              | 200             | 200 OK           | ✅ OK   |
| 23  | JoinTableAuthorized_WithValidTokenAndPassword_ReturnsOkWithJoinTableResponse | 200             | 200 OK           | ✅ OK   |
| 24  | JoinTableAuthorized_WithWrongPassword_ReturnsBadRequest                      | 400             | 400 BadRequest   | ✅ OK   |
| 25  | JoinTableAuthorized_WithoutToken_ReturnsUnauthorized                         | 401             | 401 Unauthorized | ✅ OK   |
| 26  | JoinTable_WithFullTable_ReturnsBadRequest                                    | 400             | 400 BadRequest   | ✅ OK   |
| 27  | JoinTable_WithValidRequest_ReturnsOkWithTableResponse                        | 200             | 200 OK           | ✅ OK   |
| 28  | JoinTable_WithWrongPassword_ReturnsBadRequest                                | 400             | 400 BadRequest   | ✅ OK   |
| 29  | LeaveTable_AsRegularMember_ReturnsOk                                         | 200             | 200 OK           | ✅ OK   |
| 30  | RevokeAdmin_ByTableCreator_ReturnsOk                                         | 200             | 200 OK           | ✅ OK   |

**Podsumowanie:** ✅ 30/30 testów - wszystkie poprawne

---

## PODSUMOWANIE ANOMALII

| Kontroler            | Liczba testów | Anomalie | Status          |
|----------------------|---------------|----------|-----------------|
| AuthController       | 12            | 0        | ✅ Brak anomalii |
| BetsController       | 9             | 0        | ✅ Brak anomalii |
| EventTypesController | 7             | 0        | ✅ Brak anomalii |
| MatchesController    | 3             | 0        | ✅ Brak anomalii |
| TablesController     | 30            | 0        | ✅ Brak anomalii |
| **RAZEM**            | **57**        | **0**    | **✅ PASS**      |

---

## WNIOSKI

🎉 **Wyniki analizy są DOSKONAŁE**

Przeprowadzona kompleksowa analiza wszystkich 57 snapshotów testów integracyjnych wykazała:

✅ **100% zgodność** między nazewnictwem testów a rzeczywistościami ResponseStatus  
✅ **Brak anomalii** w żadnym z kontrolerów  
✅ **Konsekwentne kodowanie** status codes HTTP  
✅ **Bez problemów** w logice biznesowej testów

### Obserwacje pozytywne:

1. **Prawidłowe kodowanie błędów:**
    - BadRequest (400) dla błędów walidacji danych
    - Unauthorized (401) dla braku autentykacji
    - Forbidden (403) dla braku autoryzacji
    - NotFound (404) dla nieistniejących zasobów
    - Conflict (409) dla konfliktów danych

2. **Prawidłowe kodowanie sukcesu:**
    - OK (200) dla operacji GET i DELETE
    - Created (201) dla operacji POST wytwarzających nowe zasoby

3. **Testy są dobrze skonstruowane** - nazwy jasno opisują oczekiwane zachowanie

---

## ZALECENIA

ℹ️ **Brak działań naprawczych wymaganych**

Bieżący stan testów integracyjnych jest zadowalający. Testowe snapshoty są konsekwentne i odzwierciedlają prawidłowe kodowanie status codes HTTP.

---

## METODOLOGIA ANALIZY

Raport przygotowano poprzez:

1. Przeanalizowanie wszystkich 57 plików `.verified.json` z folderów kontrolerów
2. **Fokus na niezgodności status codes HTTP** - porównanie ResponseStatus z oczekiwanym statusem wynika z nazwy testu
3. Wyszukanie testów gdzie ResponseStatus nie odpowiada semantyce nazwy testu
4. Identyfikacja testów zwracających błędy (4xx/5xx) w niezaplanowanych miejscach

---

## DATA GENERACJI RAPORTU

- **Data analizy**: 7 lutego 2026
- **Branch**: more-ddd
- **Liczba przeanalizowanych snapshotów**: 57
- **Liczba znalezionych anomalii status codes**: 0 (wcześniej: 9)
- **Zmiana od poprzedniej analizy**: ✅ Wszystkie anomalie naprawione

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
