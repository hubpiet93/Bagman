# Dokumentacja techniczna – System do typowania meczów piłki nożnej  
**Blazor (client-side) + ASP.NET Core API + Supabase + MudBlazor**

---

## 1. Architektura systemu

### 1.1. Stack technologiczny

- **Frontend (UI):**  
  - Blazor WebAssembly (C#) – aplikacja całkowicie po stronie klienta, hostowana statycznie.
  - MudBlazor – nowoczesny, lekki framework UI, pozwala na projektowanie nietypowych nawigacji (okienka w okienku).
- **Backend (API):**  
  - ASP.NET Core (.NET 8) – REST API, pośredniczy między Blazor a Supabase.
  - API obsługuje autoryzację, logikę biznesową, walidacje, bezpieczeństwo.
- **Baza danych:**  
  - Supabase PostgreSQL – przechowuje wszystkie dane aplikacji.
  - API komunikuje się z Supabase przez oficjalne klienty .NET (lub REST/GraphQL).
- **Autoryzacja:**  
  - Supabase Auth – zarządzanie kontami (rejestracja, logowanie, sesje).
  - API obsługuje pośrednictwo w autoryzacji, przekazuje tokeny JWT do Blazor.
- **DevOps/Uruchamianie lokalne:**  
  - VS Code/Visual Studio + .NET CLI
  - Plik `.env` do konfiguracji połączeń (np. klucz API Supabase, endpoint DB itp.)

---

## 2. Instalacja i uruchamianie lokalnie

### 2.1. Wymagania systemowe

- .NET 8 SDK  
- Node.js (opcjonalnie, do niektórych narzędzi UI, np. budowanie stylów)
- Konto Supabase (https://supabase.com)  
- Klucze API Supabase (do autoryzacji i bazy)
- Docker (opcjonalnie, do uruchomienia lokalnej bazy PostgreSQL/Supabase)

### 2.2. Kroki instalacji

1. **Klonowanie repozytorium:**
   ```bash
   git clone https://github.com/twoj-repo/typowanie-meczy.git
   cd typowanie-meczy
   ```

2. **Konfiguracja połączenia Supabase:**
   - Utwórz plik `.env.local` w katalogu API z danymi:
     ```
     SUPABASE_URL=https://xxxx.supabase.co
     SUPABASE_ANON_KEY=xxxx
     SUPABASE_SERVICE_KEY=xxxx
     ```

3. **Migracja bazy danych:**
   - W przypadku lokalnego developmentu uruchom Supabase przez Docker lub korzystaj z instancji chmurowej.
   - Skrypt migracji oraz seed danych znajduje się w katalogu `db/migrations`.

4. **Instalacja zależności:**
   ```bash
   dotnet restore
   ```

5. **Uruchomienie API:**
   ```bash
   dotnet run --project src/TypowanieMeczy.Api
   ```

6. **Uruchomienie Blazor WebAssembly:**
   ```bash
   dotnet run --project src/TypowanieMeczy.Web
   ```
   - UI będzie dostępne pod `https://localhost:5002`
   - API pod `https://localhost:5001` (domyślnie)

---

## 3. Architektura aplikacji

### 3.1. Warstwy

- **UI (Blazor WebAssembly):**  
  - Renderuje widoki, komunikuje się wyłącznie z własnym .NET API przez HTTP (REST).
  - Nie kontaktuje się bezpośrednio z Supabase.
- **API (ASP.NET Core):**  
  - Realizuje logikę biznesową, autoryzację, walidację danych, obsługę uprawnień.
  - Komunikuje się z Supabase (baza, auth, storage, realtime).
  - Zwroty JSON, statusy HTTP, obsługa błędów.
- **Supabase:**  
  - Przechowuje dane, obsługuje autoryzację, storage, realtime.
- **DevOps:**  
  - Skrypty migracji bazy, seed danych, CI/CD pipeline (testy, deployment), monitoring.

---

## 4. Integracja Blazor <-> API <-> Supabase

- **Blazor komunikuje się WYŁĄCZNIE z własnym API przez REST.**
- **API pośredniczy w autoryzacji użytkownika:**  
  - UI przesyła dane logowania do API  
  - API kontaktuje się z Supabase Auth, otrzymuje JWT, zwraca je do UI
  - Zarządzanie sesją po stronie UI (token JWT w localStorage lub sessionStorage)
- **API realizuje całą logikę dostępu do danych:**  
  - Pobieranie/przesyłanie danych (mecze, stoły, typy, pule, statystyki)
  - Walidacja uprawnień i ról (np. admin stołu)
  - Obsługa błędów i logowanie zdarzeń

---

## 5. Bezpieczeństwo

- **Hasła min. 10 znaków, hashowane przez Supabase Auth**
- **Dostęp do API chroniony przez JWT (Bearer token)**
- **Weryfikacja uprawnień i ról w API**
- **CORS skonfigurowany na API**
- **Ochrona przed typowymi atakami (XSS, CSRF, brute force) – szczegóły w README API**
- **Rate limiting dla endpointów API**
- **Logowanie zdarzeń i błędów (np. do Supabase Storage lub zewnętrznego systemu logującego)**

---

## 6. Obsługa migracji i wersjonowania bazy danych

- **Skrypty migracji w katalogu `db/migrations`**
- **Seed danych testowych**
- **Instrukcja aktualizacji schematu bazy przy wdrożeniach**
- **Wersjonowanie API – np. `/api/v1/`, `/api/v2/`**

---

## 7. Testowanie i jakość

- **Testy jednostkowe (xUnit) – logika API**
- **Testy integracyjne – komunikacja API z Supabase**
- **Testy E2E – UI + API (np. Playwright lub Selenium)**
- **Testy UI (MudBlazor) – snapshot tests**
- **Linter C# (dotnet-format), analiza statyczna kodu**
- **CI/CD pipeline – GitHub Actions, Azure DevOps, itp.**

---

## 8. Monitoring / Logging

- **Logowanie błędów w API (ILogger, np. do pliku, Azure Monitor, Sentry)**
- **Monitorowanie wydajności API**
- **Alertowanie o krytycznych błędach**
- **Logi audytowe (np. ważne operacje: zmiana admina, dodanie meczu, typowanie)**

---

## 9. UI/UX

- **MudBlazor – elastyczne, lekkie komponenty, okienka w okienku, brak typowych dashboardowych nawigacji**
- **Nawigacja w formie kart, paneli, dialogów, nie jako stały pasek**
- **Mobile-first, responsywność, jasny motyw, duże odstępy, zaokrąglone elementy**
- **Mockupy i wireframes – dołączone w katalogu `design/`**
- **Onboarding użytkownika, rejestracja, reset hasła, flow logowania – opisane w osobnym rozdziale**
- **WCAG 2.1 – minimalne wymagania dostępności**

---

## 10. Model danych

**Szczegółowy ERD i typy pól dołączone w katalogu `db/ERD.png` oraz `db/schema.sql`.**

Przykład encji:

- `users`  
  - id (uuid, PK), login (varchar, unique), password_hash, email (unique), created_at, is_active
- `tables`  
  - id (uuid, PK), name (varchar), password_hash, max_players (int), stake (decimal), created_by (uuid, FK), created_at
- `table_members`  
  - user_id (FK), table_id (FK), is_admin (bool), joined_at
- `matches`  
  - id (uuid, PK), table_id (FK), country_1, country_2, datetime, result, status (enum), started (bool)
- `bets`  
  - id (uuid, PK), user_id (FK), match_id (FK), prediction (varchar), edited_at
- `pools`  
  - id (uuid, PK), match_id (FK), amount (decimal), status (enum: won, rollover, expired), winners[]
- `stats`  
  - user_id (FK), table_id (FK), matches_played, bets_placed, pools_won, total_won

---

## 11. Rozszerzenia i wersja 2.0

- Tryb „tajemniczego typera" – flaga w tabeli `tables`, obsługa w UI i API
- Powiadomienia – integracja z Supabase Functions lub zewnętrzne API (np. SendGrid, Twilio)
- Wersja charytatywna – tabela `charity_history`
- Mobile – PWA (Blazor WebAssembly natywnie wspiera), dedykowany layout mobile
- Komentarze/czat – Supabase Realtime Channels przez API
- Integracja z zewnętrznym API – np. [API-Football](https://www.api-football.com/), obsługa przez backend API

**Roadmapa jest opisana w katalogu `roadmap.md` z priorytetami i szacunkami czasowymi.**

---

## 12. Monitoring, backup i disaster recovery

- **Backup bazy danych – codzienny eksport przez Supabase lub narzędzia zewnętrzne**
- **Instrukcja odtwarzania backupu**
- **Monitorowanie dostępności API (np. UptimeRobot, HealthChecks w .NET)**
- **Procedury awaryjne – opisane w katalogu `ops/`**

---

## 13. FAQ dla developerów

- **Jak debugować typowe problemy z Supabase/Blazor/API**
- **Jak dodać nową migrację bazy**
- **Jak dodać nowy endpoint API**
- **Jak stylizować MudBlazor zgodnie z wymaganiami UI**
- **Jak dodać testy do projektu**

---

## 14. Linki i zasoby

- [Supabase .NET Client Docs](https://supabase.com/docs/reference/dotnet)
- [MudBlazor Docs](https://mudblazor.com/getting-started/installation)
- [Blazor WebAssembly Docs](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-8.0)
- [ASP.NET Core API Docs](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-8.0)

---

**W razie pytań dotyczących wdrożenia lub kodu – zgłaszaj uwagi i propozycje na repozytorium projektu!**

---

# Pytania do Ciebie (w celu dopełnienia dokumentacji)

1. **Czy API będzie publiczne, czy tylko dla frontendu? (Warto doprecyzować politykę CORS i uprawnienia dostępu)**
2. **Czy przewidujesz wersjonowanie API od początku? (np. /api/v1/)**
3. **Czy chcesz obsługiwać upload plików (np. avatarów, eksportów) przez API/Supabase Storage?**
4. **Czy potrzebujesz integracji z zewnętrznymi systemami powiadomień (email, SMS)?**
5. **Jakie są preferencje co do hostingu: chmura, VPS, lokalnie?**
6. **Czy w projekcie będą wymagane logi audytowe (np. kto, kiedy zmienił typ, dodał mecz itp.)?**
7. **Czy chcesz, żeby UI był dostępny bez logowania (np. jako demo)?**
8. **Czy jest wymóg wsparcia dla języka angielskiego (multilanguage)?**
9. **Czy są jakieś limity na liczbę stołów, graczy, meczów?**

---

Odpowiedz na powyższe, a doprecyzuję dokumentację według Twoich preferencji! 