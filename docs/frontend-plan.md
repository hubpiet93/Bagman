# Plan realizacji frontendu - Bagman

---

## Spis treści - Postęp implementacji

### Ekrany

- [ ] **Ekran 1: Autoryzacja (Auth)** - logowanie, rejestracja, dołączanie do stołu
- [ ] **Ekran 2: Moje stoły** - lista stołów, tworzenie, dołączanie
- [ ] **Ekran 3: Dashboard stołu** - główny widok gry
  - [ ] 3.1 Nagłówek stołu
  - [ ] 3.2 Lista meczów
  - [ ] 3.3 Panel typowania
  - [ ] 3.4 Typy innych graczy
  - [ ] 3.5 Leaderboard
  - [ ] 3.6 Pule
  - [ ] 3.7 Statystyki
  - [ ] 3.8 Lista członków
  - [ ] 3.9 Panel admina stołu
- [ ] **Ekran 4: Panel Super Admina** - zarządzanie wydarzeniami i meczami
  - [ ] 4.1 Lista typów wydarzeń
  - [ ] 4.2 CRUD typów wydarzeń
  - [ ] 4.3 Lista meczów
  - [ ] 4.4 Dodawanie meczów
- [ ] **Ekran 5: Szczegóły meczu** - widok szczegółów (modal lub strona)

### Elementy wspólne

- [ ] **Header/Nawigacja** - logo, użytkownik, wylogowanie, linki
- [ ] **Obsługa błędów** - 401, 403, 404, 400, 409
- [ ] **Automatyczne odświeżanie tokenów**

### Testy manualne

- [ ] Przeprowadzono testy ekranu Auth
- [ ] Przeprowadzono testy ekranu Moje stoły
- [ ] Przeprowadzono testy Dashboard stołu
- [ ] Przeprowadzono testy Panel Super Admina
- [ ] Przeprowadzono testy nawigacji i obsługi błędów

---

## Podsumowanie

Aplikacja Bagman to system do typowania meczów piłkarskich w gronie znajomych (EURO, Mistrzostwa Świata). Plan opisuje ekrany, akcje dla każdego typu użytkownika oraz kryteria akceptacji do manualnej weryfikacji.

**Typy użytkowników:**
1. **Gość** - niezalogowany użytkownik
2. **Gracz** - zalogowany członek stołu bez uprawnień admina
3. **Administrator stołu** - gracz z uprawnieniami administracyjnymi na danym stole
4. **Super Admin** - globalny administrator systemu (zarządza wydarzeniami i meczami)

---

## Ekran 1: Autoryzacja (Auth)

Ekran łączący logowanie, rejestrację oraz flow dołączania do stołu.

### Widoki/Zakładki:
- **Logowanie**
- **Rejestracja**
- **Dołącz do stołu** (dla nowych użytkowników - rejestracja + dołączenie)

### Akcje według typu użytkownika:

| Użytkownik | Akcja |
|------------|-------|
| Gość | Wypełnić formularz logowania (login, hasło) i zalogować się |
| Gość | Wypełnić formularz rejestracji (login, hasło, email) i utworzyć konto |
| Gość | Skorzystać z formularza "Dołącz do stołu" (login, hasło, nazwa stołu, hasło stołu) |

### Walidacje do wyświetlenia:
- Email: poprawny format
- Hasło: min. 10 znaków, wielka litera, cyfra, znak specjalny
- Obsługa błędów: duplikat loginu, nieprawidłowe dane logowania

### Kryteria akceptacji:

- [ ] Mogę przełączać się między zakładkami logowania i rejestracji
- [ ] Po wpisaniu błędnego hasła (za krótkiego, bez wielkiej litery itp.) widzę konkretne komunikaty błędów
- [ ] Po poprawnej rejestracji jestem automatycznie zalogowany
- [ ] Po poprawnym logowaniu przechodzę do ekranu wyboru stołu
- [ ] Próba rejestracji z istniejącym loginem pokazuje komunikat "Użytkownik o podanym loginie już istnieje"
- [ ] Próba logowania z błędnym hasłem pokazuje komunikat "Nieprawidłowy login lub hasło"
- [ ] Formularz "Dołącz do stołu" umożliwia jednoczesną rejestrację i dołączenie do stołu
- [ ] Błędne hasło do stołu pokazuje komunikat "Nieprawidłowe hasło do stołu"

---

## Ekran 2: Moje stoły (Lista + Akcje)

Ekran główny po zalogowaniu - lista stołów użytkownika z możliwością tworzenia nowych i dołączania do istniejących.

### Widoki:
- **Lista moich stołów** - stoły, do których należę
- **Modal/Formularz tworzenia stołu**
- **Modal/Formularz dołączania do stołu**

### Akcje według typu użytkownika:

| Użytkownik | Akcja |
|------------|-------|
| Gracz | Zobaczyć listę stołów, do których należy |
| Gracz | Kliknąć na stół, aby przejść do dashboardu |
| Gracz | Otworzyć formularz tworzenia nowego stołu |
| Gracz | Utworzyć nowy stół (nazwa, hasło, max graczy, stawka, typ wydarzenia) |
| Gracz | Otworzyć formularz dołączania do stołu |
| Gracz | Dołączyć do stołu (ID lub nazwa stołu, hasło) |
| Gracz | Wylogować się |

### Dane wyświetlane na liście stołów:
- Nazwa stołu
- Stawka za mecz
- Maksymalna liczba graczy
- Informacja czy jestem adminem

### Walidacje tworzenia stołu:
- Nazwa stołu: wymagana, max 100 znaków
- Hasło: wymagane, max 255 znaków
- MaxPlayers: > 0 i ≤ 1000
- Stake: ≥ 0
- EventTypeId: wymagany (dropdown z typami wydarzeń z API)

### Kryteria akceptacji:

- [ ] Po zalogowaniu widzę listę stołów, do których należę
- [ ] Każdy stół na liście pokazuje nazwę, stawkę i liczbę graczy
- [ ] Klikając na stół przechodzę do dashboardu tego stołu
- [ ] Mogę otworzyć modal tworzenia stołu
- [ ] W formularzu tworzenia widzę dropdown z dostępnymi typami wydarzeń (pobierane z API)
- [ ] Po utworzeniu stołu jestem automatycznie jego adminem i członkiem
- [ ] Próba utworzenia stołu z istniejącą nazwą pokazuje błąd "Stół o podanej nazwie już istnieje"
- [ ] Mogę otworzyć modal dołączania do stołu
- [ ] Po poprawnym dołączeniu stół pojawia się na mojej liście
- [ ] Próba dołączenia do pełnego stołu pokazuje błąd "Stół jest pełny"
- [ ] Przycisk wylogowania jest widoczny i działa

---

## Ekran 3: Dashboard stołu

Główny ekran dla gracza przy stole - zawiera wszystkie informacje: mecze, typy, pule, statystyki, leaderboard, listę członków.

### Sekcje dashboardu:
1. **Nagłówek stołu** - nazwa, stawka, liczba graczy
2. **Lista meczów** - nadchodzące i zakończone
3. **Moje typy** - lista moich typów na mecze
4. **Panel typowania** - formularz do obstawiania
5. **Leaderboard** - ranking graczy
6. **Pule** - informacje o pulach (active, won, rollover, expired)
7. **Statystyki użytkowników**
8. **Lista członków stołu**
9. **Panel admina** (tylko dla adminów)

### Akcje według typu użytkownika:

| Użytkownik | Akcja |
|------------|-------|
| Gracz | Zobaczyć listę wszystkich meczów (nadchodzących i zakończonych) |
| Gracz | Zobaczyć wyniki zakończonych meczów |
| Gracz | Obstawić wynik meczu (format: "X:Y" lub "X" dla remisu) |
| Gracz | Edytować swój typ na mecz (jeśli mecz się nie rozpoczął) |
| Gracz | Usunąć swój typ na mecz (jeśli mecz się nie rozpoczął) |
| Gracz | Zobaczyć typy innych graczy (po rozpoczęciu meczu) |
| Gracz | Zobaczyć leaderboard z punktacją |
| Gracz | Zobaczyć aktualną pulę i historię pul |
| Gracz | Zobaczyć swoje statystyki (mecze, typy, wygrane) |
| Gracz | Zobaczyć listę członków stołu |
| Gracz | Opuścić stół |
| Gracz | Wrócić do listy stołów |
| Admin stołu | Wszystkie akcje gracza |
| Admin stołu | Nadać rolę admina innemu członkowi |
| Admin stołu | Odebrać rolę admina członkowi |

### Logika wyświetlania typów innych graczy:
- **Przed rozpoczęciem meczu**: Typy innych graczy są ukryte
- **Po rozpoczęciu meczu**: Typy wszystkich graczy są widoczne

### System punktacji leaderboard:
- **3 punkty**: Dokładne trafienie wyniku (exact hit)
- **1 punkt**: Trafienie zwycięzcy/remisu (winner hit)
- **0 punktów**: Pudło

### Statusy pul:
- `active` - mecz nie rozpoczął się, można obstawiać
- `won` - mecz zakończony, są zwycięzcy
- `rollover` - nikt nie trafił, pula przechodzi dalej
- `expired` - ostatni mecz, nikt nie trafił

### Kryteria akceptacji:

**Mecze i typowanie:**
- [ ] Widzę listę wszystkich meczów stołu (nadchodzące i zakończone)
- [ ] Przy każdym meczu widzę: drużyny, datę/godzinę, status (rozpoczęty/nie)
- [ ] Zakończone mecze pokazują wynik
- [ ] Mogę wpisać typ na mecz w formacie "X:Y" (np. "2:1")
- [ ] Mogę wpisać typ "X" oznaczający remis
- [ ] Próba wpisania nieprawidłowego formatu pokazuje błąd walidacji
- [ ] Po rozpoczęciu meczu nie mogę edytować ani usunąć swojego typu
- [ ] Przed rozpoczęciem meczu mogę wielokrotnie zmieniać swój typ
- [ ] Przed rozpoczęciem meczu mogę usunąć swój typ

**Typy innych graczy:**
- [ ] Przed rozpoczęciem meczu NIE widzę typów innych graczy
- [ ] Po rozpoczęciu meczu widzę typy wszystkich graczy na ten mecz
- [ ] Lista typów pokazuje: login gracza, przewidywany wynik

**Leaderboard:**
- [ ] Widzę ranking wszystkich graczy stołu
- [ ] Ranking pokazuje: pozycję, login, punkty, exact hits, winner hits, accuracy
- [ ] Ranking jest posortowany wg punktów (malejąco)

**Pule:**
- [ ] Widzę aktualną pulę (kwota, status)
- [ ] Widzę historię pul z informacją o statusie i zwycięzcach

**Statystyki:**
- [ ] Widzę swoje statystyki: mecze, postawione typy, wygrane pule, suma wygranych

**Członkowie:**
- [ ] Widzę listę członków stołu z oznaczeniem adminów
- [ ] Każdy członek ma widoczny login i datę dołączenia

**Panel admina (tylko dla adminów):**
- [ ] Jako admin widzę dodatkową sekcję zarządzania
- [ ] Mogę nadać rolę admina innemu członkowi
- [ ] Mogę odebrać rolę admina członkowi

**Akcje stołu:**
- [ ] Mogę opuścić stół (przycisk "Opuść stół")
- [ ] Po opuszczeniu stołu znikam z listy członków i wracam do listy stołów
- [ ] Mogę wrócić do listy stołów (przycisk "Powrót")

---

## Ekran 4: Panel Super Admina

Ekran dostępny tylko dla użytkowników z rolą Super Admin. Zarządzanie typami wydarzeń i meczami.

### Sekcje:
1. **Lista typów wydarzeń** - wszystkie typy (aktywne i nieaktywne)
2. **Formularz dodawania typu wydarzenia**
3. **Lista meczów w danym typie wydarzenia**
4. **Formularz dodawania meczu**

### Akcje Super Admina:

| Użytkownik | Akcja |
|------------|-------|
| Super Admin | Zobaczyć listę wszystkich typów wydarzeń |
| Super Admin | Utworzyć nowy typ wydarzenia (code, name, startDate) |
| Super Admin | Edytować typ wydarzenia (name, startDate) |
| Super Admin | Dezaktywować typ wydarzenia |
| Super Admin | Zobaczyć listę meczów w danym typie wydarzenia |
| Super Admin | Dodać mecz do typu wydarzenia (country1, country2, matchDateTime) |

### Walidacje:
- Code typu wydarzenia: unikalny
- Country1, Country2: autocomplete z listy krajów lub wolny tekst
- MatchDateTime: data w przyszłości

### Kryteria akceptacji:

**Typy wydarzeń:**
- [ ] Widzę listę wszystkich typów wydarzeń (code, name, startDate, isActive)
- [ ] Mogę otworzyć formularz dodawania nowego typu wydarzenia
- [ ] Poprawne utworzenie typu wydarzenie dodaje go do listy
- [ ] Próba utworzenia z istniejącym kodem pokazuje błąd "Typ wydarzenia o podanym kodzie już istnieje"
- [ ] Mogę edytować nazwę i datę rozpoczęcia typu wydarzenia
- [ ] Mogę dezaktywować typ wydarzenia
- [ ] Dezaktywowany typ ma oznaczenie "nieaktywny"

**Mecze:**
- [ ] Mogę wybrać typ wydarzenia i zobaczyć listę jego meczów
- [ ] Widzę przy każdym meczu: drużyny, datę/godzinę, status
- [ ] Mogę dodać nowy mecz (wybór krajów, data/godzina)
- [ ] Poprawne dodanie meczu pokazuje go na liście
- [ ] Pole krajów ma autocomplete lub pozwala na wolny tekst

---

## Ekran 5: Szczegóły meczu (opcjonalny - może być modalem)

Widok szczegółów pojedynczego meczu - może być osobnym ekranem lub modalem na dashboardzie.

### Wyświetlane dane:
- Drużyny (country1 vs country2)
- Data i godzina meczu
- Status (scheduled, started, finished)
- Wynik (jeśli zakończony)
- Flaga `started` (czy mecz się rozpoczął)

### Akcje:
| Użytkownik | Akcja |
|------------|-------|
| Gracz | Zobaczyć szczegóły meczu |
| Gracz | Obstawić wynik (jeśli mecz się nie rozpoczął) |
| Gracz | Zobaczyć wszystkie typy (po rozpoczęciu meczu) |

### Kryteria akceptacji:

- [ ] Widzę pełne informacje o meczu
- [ ] Przed rozpoczęciem meczu widzę formularz do typowania
- [ ] Po rozpoczęciu meczu formularz jest zablokowany
- [ ] Po rozpoczęciu meczu widzę typy wszystkich graczy

---

## Globalny przepływ nawigacji

```
[Ekran Auth]
    |
    v
[Moje stoły] <----> [Dashboard stołu]
    |                      |
    |                      v
    |              [Szczegóły meczu]
    |
    v (tylko Super Admin)
[Panel Super Admina]
```

## Elementy wspólne (Header/Nawigacja)

### Dla zalogowanego użytkownika:
- Logo/Nazwa aplikacji
- Nazwa zalogowanego użytkownika
- Przycisk wylogowania
- Link do "Moje stoły" (zawsze widoczny)
- Link do "Panel admina" (tylko dla Super Admina)

### Dla gościa:
- Logo/Nazwa aplikacji
- Link do logowania/rejestracji

### Kryteria akceptacji nawigacji:

- [ ] Header jest widoczny na każdym ekranie
- [ ] Nazwa użytkownika jest wyświetlona gdy zalogowany
- [ ] Przycisk wylogowania działa i przekierowuje na ekran Auth
- [ ] Link "Panel admina" jest widoczny tylko dla Super Admina
- [ ] Kliknięcie logo/nazwy przekierowuje do listy stołów (zalogowany) lub Auth (gość)

---

## Obsługa błędów (globalna)

### Błędy HTTP do obsługi:
- **401 Unauthorized** - sesja wygasła, przekieruj na ekran Auth
- **403 Forbidden** - brak uprawnień, wyświetl komunikat
- **404 Not Found** - zasób nie istnieje, wyświetl komunikat
- **400 Bad Request** - wyświetl błędy walidacyjne z API
- **409 Conflict** - wyświetl komunikat o konflikcie

### Kryteria akceptacji:

- [ ] Wygaśnięcie tokena automatycznie wylogowuje użytkownika
- [ ] Próba dostępu do zasobu bez uprawnień pokazuje komunikat błędu
- [ ] Błędy walidacyjne z API są wyświetlane przy odpowiednich polach formularza
- [ ] Błędy sieciowe (offline) pokazują odpowiedni komunikat

---

## Podsumowanie ekranów

| # | Ekran | Dostęp | Główna funkcja |
|---|-------|--------|----------------|
| 1 | Auth | Gość | Logowanie, rejestracja, dołączanie do stołu |
| 2 | Moje stoły | Zalogowany | Lista stołów, tworzenie, dołączanie |
| 3 | Dashboard stołu | Członek stołu | Typowanie, leaderboard, statystyki |
| 4 | Panel Super Admina | Super Admin | Zarządzanie wydarzeniami i meczami |
| 5 | Szczegóły meczu | Członek stołu | Szczegóły meczu (modal lub strona) |

---

## Kolejność implementacji (sugestia)

1. Ekran Auth (logowanie, rejestracja)
2. Ekran Moje stoły
3. Dashboard stołu (podstawowe: mecze, typowanie)
4. Dashboard stołu (rozszerzone: leaderboard, pule, statystyki)
5. Panel admina stołu
6. Panel Super Admina
7. Szczegóły meczu (jeśli osobny ekran)
8. Obsługa błędów i edge cases

---

## Dokumentacja API

Backend API jest dostępny pod adresem: `https://bagman-production.up.railway.app`

Szczegółowa dokumentacja endpointów znajduje się w: `docs/api-documentation-v3.md`
