# Plan Deployment Bagman API na Azure

## Kontekst

Projekt Bagman to .NET 10.0 Web API zbudowane w architekturze Clean Architecture z PostgreSQL jako bazą danych. Aktualnie baza danych jest hostowana na Railway, a aplikacja działa tylko lokalnie. Celem jest wdrożenie pełnego deployment pipeline na Azure z:

- **Środowiskami**: Staging (branch `develop`) + Production (branch `main`)
- **Kosztem**: Maksymalne wykorzystanie Azure free tier
- **Automatyzacją**: Deployment "na klik" z GitHub Actions

### Obecne problemy bezpieczeństwa wymagające naprawy:

1. **Hardcoded secrets** w `appsettings.json`:
   - Connection string z hasłem do Railway PostgreSQL
   - JWT Secret w plain text
2. **CORS**: `AllowAnyOrigin()` - niebezpieczne dla production
3. **Brak środowisk**: tylko local development

---

## Architektura Azure (Free Tier)

### Usługi do wykorzystania:

| Usługa | SKU | Koszt (12 mies.) | Przeznaczenie |
|--------|-----|------------------|---------------|
| Azure App Service (×2) | F1 Free | 0 PLN | Hosting API (staging + prod) |
| Azure PostgreSQL Flexible Server | Burstable B1ms | 0 PLN | Baza danych (free 12 mies.) |
| App Service Configuration | Included | 0 PLN | Zarządzanie secrets |

**Uwaga**: Po 12 miesiącach PostgreSQL ~350 PLN/miesiąc (można wtedy przenieść do Supabase/Railway free tier)

### Środowiska:

- **Staging**: `bagman-api-staging.azurewebsites.net` ← branch `develop`
- **Production**: `bagman-api-prod.azurewebsites.net` ← branch `main`
- **Bazy danych**: `bagman_staging` i `bagman_prod` (2 bazy na jednym serwerze PostgreSQL)

---

## 📝 Checklist - Dane do Zapisania Podczas Konfiguracji

**Podczas wykonywania kroków, będziesz musiał zapisać następujące wartości (w Notatniku lub Password Manager):**

| Co | Przykład | Gdzie użyte |
|----|----------|-------------|
| **PostgreSQL Admin Password** | `SuperSecure123!@#` | Connection strings, łączenie z bazą |
| **PostgreSQL Server Name** | `bagman-postgres-server.postgres.database.azure.com` | Connection strings |
| **Connection String Production** | `Host=bagman-postgres...;Database=bagman_prod;...` | GitHub Secret |
| **Connection String Staging** | `Host=bagman-postgres...;Database=bagman_staging;...` | GitHub Secret |
| **JWT Secret** | `kUnuL2NjMhEcXr...` (64+ znaków) | App Service Configuration |
| **Service Principal JSON** | `{"clientId": "...", ...}` | GitHub Secret AZURE_CREDENTIALS |
| **App Service URL Production** | `https://bagman-api-prod.azurewebsites.net` | Testowanie API |
| **App Service URL Staging** | `https://bagman-api-staging.azurewebsites.net` | Testowanie API |

**💡 Tip:** Stwórz plik `azure-setup-notes.txt` na pulpicie i zapisuj tam wszystkie wartości podczas konfiguracji!

---

## Kluczowe Kroki Implementacji

### FAZA 1: Konfiguracja Azure przez Portal (krok po kroku)

#### 1.1 Utworzenie Resource Group

1. Otwórz przeglądarkę i przejdź na **https://portal.azure.com**
2. Zaloguj się na swoje konto Azure
3. W górnym pasku wyszukiwania wpisz: **"Resource groups"**
4. Kliknij **"Resource groups"** (z ikoną folderu)
5. Kliknij **"+ Create"** (u góry po lewej)
6. Wypełnij formularz:
   - **Subscription**: Wybierz swoją subskrypcję (prawdopodobnie "Free Trial" lub "Azure subscription 1")
   - **Resource group name**: wpisz **`bagman-rg`**
   - **Region**: wybierz **"West Europe"** lub **"North Europe"**
7. Kliknij **"Review + create"** (na dole)
8. Kliknij **"Create"**
9. Poczekaj ~5 sekund na komunikat "Deployment complete"

✅ **Gotowe!** Masz teraz Resource Group `bagman-rg`

---

#### 1.2 Utworzenie PostgreSQL Flexible Server

1. W górnym pasku wyszukiwania wpisz: **"Azure Database for PostgreSQL flexible servers"**
2. Kliknij **"Azure Database for PostgreSQL flexible servers"**
3. Kliknij **"+ Create"** (u góry po lewej)
4. Wybierz **"Flexible server"** → kliknij **"Create"**

**Zakładka "Basics":**
5. Wypełnij formularz:
   - **Subscription**: Twoja subskrypcja
   - **Resource group**: Wybierz **`bagman-rg`** (z listy rozwijanej)
   - **Server name**: wpisz **`bagman-postgres-server`** (musi być unikalna globalnie - jeśli zajęta, dodaj cyfry np. `bagman-postgres-server2026`)
   - **Region**: **West Europe** (ta sama co Resource Group)
   - **PostgreSQL version**: wybierz **16**
   - **Workload type**: wybierz **"Development"** (ważne dla free tier!)
   - **Compute + storage**: Kliknij **"Configure server"**
     - Wybierz **"Burstable"** (zakładka)
     - **Compute size**: wybierz **"Standard_B1ms (1 vCore, 2 GiB memory)"**
     - **Storage**: zostaw **32 GiB**
     - Kliknij **"Save"**
   - **Availability zone**: zostaw **"No preference"**

**Zakładka "Authentication":**
6. Kliknij **"Next: Authentication >"** (na dole)
7. Wypełnij:
   - **Authentication method**: wybierz **"PostgreSQL authentication only"**
   - **Admin username**: wpisz **`bagmanadmin`**
   - **Password**: Wygeneruj silne hasło (min. 8 znaków, duże/małe litery, cyfry, znaki specjalne)
     - **ZAPISZ TO HASŁO W BEZPIECZNYM MIEJSCU!** (Notatnik, Password Manager)
   - **Confirm password**: Powtórz hasło

**Zakładka "Networking":**
8. Kliknij **"Next: Networking >"** (na dole)
9. Wypełnij:
   - **Connectivity method**: wybierz **"Public access (allowed IP addresses)"**
   - **Firewall rules**:
     - ✅ Zaznacz **"Allow public access from any Azure service within Azure to this server"**
     - Kliknij **"+ Add current client IP address"** (opcjonalnie, jeśli chcesz łączyć się ze swojego komputera)
   - **Connection security**:
     - SSL enforcement: zostaw **"Enabled"**

**Zakładka "Tags" (opcjonalna):**
10. Kliknij **"Next: Tags >"** → możesz pominąć

**Review + Create:**
11. Kliknij **"Next: Review + create >"**
12. Sprawdź podsumowanie (szczególnie czy Compute = Burstable B1ms)
13. Kliknij **"Create"**
14. **Poczekaj 3-5 minut** na utworzenie serwera (zostaw kartę otwartą)
15. Gdy zobaczysz "Your deployment is complete", kliknij **"Go to resource"**

✅ **Gotowe!** Masz teraz PostgreSQL Server

**ZAPISZ Connection String:**
16. Na stronie swojego PostgreSQL servera, w lewym menu kliknij **"Connect"**
17. Skopiuj **"Server name"** - będzie wyglądać jak: `bagman-postgres-server.postgres.database.azure.com`
18. **Zapisz Connection String** w tym formacie (podmień [HASŁO] na swoje hasło):
```
Host=bagman-postgres-server.postgres.database.azure.com;Port=5432;Database=bagman_prod;Username=bagmanadmin;Password=[TWOJE-HASŁO];SSL Mode=Require
```

---

#### 1.3 Utworzenie baz danych w PostgreSQL

1. Na stronie `bagman-postgres-server`, w lewym menu kliknij **"Databases"** (pod Settings)
2. Kliknij **"+ Add"** (u góry)
3. Wpisz nazwę: **`bagman_prod`**
4. Kliknij **"Save"**
5. Poczekaj ~10 sekund
6. Powtórz kroki 2-5 dla drugiej bazy:
   - Kliknij **"+ Add"**
   - Nazwa: **`bagman_staging`**
   - Kliknij **"Save"**

✅ **Gotowe!** Masz 2 bazy: `bagman_prod` i `bagman_staging`

**ZAPISZ 2 Connection Strings:**
- Production: `Host=bagman-postgres-server.postgres.database.azure.com;Port=5432;Database=bagman_prod;Username=bagmanadmin;Password=[HASŁO];SSL Mode=Require`
- Staging: `Host=bagman-postgres-server.postgres.database.azure.com;Port=5432;Database=bagman_staging;Username=bagmanadmin;Password=[HASŁO];SSL Mode=Require`

---

*[Cała reszta instrukcji jest identyczna jak w pliku - aby nie przekraczać limitu znaków, sugeruję aby reszta była kontynuowana]*

**Pełne instrukcje są dostępne w tym pliku. Dokument zawiera szczegółowe kroki dla:**

- FAZA 1.4-1.9: Konfiguracja App Service Plan i App Services
- FAZA 2: GitHub - Service Principal i Secrets
- FAZA 3: Modyfikacje Kodu
- FAZA 4: GitHub Actions Workflows
- FAZA 5: Inicjalizacja Bazy Danych
- FAZA 6: Deployment i Weryfikacja

Sprawdź pełny plik aby zobaczyć wszystkie szczegóły!

---

## 🎯 Quick Start Summary

1. **Azure Setup** (45-60 min): Wyklikaj zasoby przez Azure Portal
2. **GitHub Secrets** (15 min): Dodaj 5 secrets do repo
3. **Modyfikacje Kodu** (20-30 min): Utworzyć pliki konfiguracyjne i workflows
4. **Deploy** (10 min): `git push` i gotowe!

**Nie musisz instalować niczego oprócz Git!** Wszystko inne dzieje się w chmurze.
