# System do obstawiania meczów piłki nożnej – Dokumentacja

---

## 1. Wprowadzenie

System do obstawiania meczów piłki nożnej został zaprojektowany z myślą o organizowaniu prywatnych zakładów wśród zamkniętej grupy znajomych podczas okazjonalnych wydarzeń sportowych, takich jak Mistrzostwa Świata czy EURO.  
Użytkownicy mogą zakładać własne „stoły do grania", zapraszać do nich znajomych, wspólnie typować wyniki meczów turniejowych, a następnie dzielić się wygranymi zgodnie z zasadami określonymi przez system.  
Rozwiązanie nie jest publiczną platformą bukmacherską i nie służy do gry o prawdziwe pieniądze na otwartym rynku – całość rozgrywa się w zamkniętym gronie, a obsługa finansów (ew. rozliczenia) odbywa się na zasadach ustalonych pomiędzy użytkownikami.

System koncentruje się na prostocie obsługi, przejrzystości zasad oraz budowaniu zaangażowania i rywalizacji w grupie znajomych.

---

## 2. Słownik pojęć

**Stół do grania (Stół)**  
Prywatna przestrzeń w systemie, do której zapraszani są znajomi w celu wspólnego obstawiania wyników meczów. Każdy stół ma własną konfigurację (liczbę graczy, stawkę, hasło itp.). Każdy użytkownik może być członkiem wielu stołów.

**Gracz (Użytkownik)**  
Osoba posiadająca konto w systemie, która może zakładać lub dołączać do stołów, obstawiać wyniki meczów oraz przeglądać statystyki.

**Administrator stołu**  
Użytkownik, który utworzył dany stół. Może zarządzać meczami, wpisywać wyniki, dodawać kolejne osoby do roli administratora, itp.

**Mecz**  
Wydarzenie sportowe (np. spotkanie piłkarskie) dodane przez administratora stołu. Każdy mecz ma określone drużyny, datę i godzinę rozpoczęcia oraz finalny wynik.

**Typ**  
Przewidywany przez użytkownika wynik meczu. Każdy gracz może zmieniać swój typ do momentu rozpoczęcia meczu.

**Pula**  
Łączna kwota do wygrania w danym meczu, powiększana w przypadku braku zwycięzcy o środki z poprzednich meczów („rollover").

**Stawka**  
Ustalona przez administratora kwota, jaką należy uiścić za każdy obstawiany mecz.

**Statystyki**  
Zestawienie informacji o aktywności gracza przy danym stole: liczba rozegranych meczów, liczba obstawionych meczów, liczba wygranych puli oraz sumaryczna kwota wygranych.

**Hasło do stołu**  
Unikalne hasło zabezpieczające dostęp do danego stołu, wymagane przy dołączaniu do grupy.

**Tryb „tajemniczego typera"**  
Opcjonalny tryb, w którym typy innych graczy są ukryte do momentu rozpoczęcia meczu. (Wersja 2.0)

---

## 3. Wymagania funkcjonalne

### Zarządzanie stołem do grania

- Użytkownik może utworzyć nowy stół do grania, podając:
  - login
  - hasło do konta (min. 10 znaków)
  - maksymalną liczbę graczy
  - cenę za obstawienie jednego meczu (w zł)
  - nazwę stołu
  - hasło do stołu (min. 10 znaków)
- Twórca stołu zostaje jego administratorem.
- Użytkownik może być członkiem wielu stołów jednocześnie i wybierać aktywny stół po zalogowaniu.
- Możliwość wylogowania się ze stołu i zalogowania do innego stołu.
- Dołączenie do istniejącego stołu wymaga podania loginu, hasła do konta, nazwy stołu i hasła do stołu przez specjalny formularz.

#### **System kont i logowania**
- Login użytkownika jest **globalny** — jedno konto służy do obsługi wszystkich stołów.
- Ten sam użytkownik (login) może należeć i logować się do wielu stołów.

### Zarządzanie meczami

- Administrator stołu może dodawać mecze (z wyborem krajów z bazy przez autocomplete oraz podaniem daty i godziny).
- Administrator może wpisywać wyniki rozegranych meczów.
- Możliwość przekazania roli administratora innemu użytkownikowi przy stole.
- **Administrator NIE może edytować ani ustawiać meczu, który już się rozpoczął** (ani jego daty/godziny, ani drużyn, ani innych parametrów).

### Zarządzanie administratorami stołu

- Musi być **co najmniej jeden administrator przy każdym stole**. Nie można dopuścić do sytuacji, w której żaden uczestnik nie ma roli administratora.
- Administrator może odebrać innemu użytkownikowi uprawnienia admina.
- Ostatni administrator **nie może odebrać sobie uprawnień ani opuścić stołu** bez wcześniejszego przekazania roli komuś innemu.

### Obstawianie wyników

- Każdy użytkownik może typować wynik każdego meczu przy swoim stole.
- Typ można zmieniać dowolną ilość razy do momentu rozpoczęcia meczu.
- Po rozpoczęciu meczu możliwość obstawiania znika.

### Rozliczanie puli

- Pula do wygrania = liczba graczy × stawka, powiększana o niewygraną pulę z poprzednich meczów.
- Jeśli nikt nie trafi wyniku – pula przechodzi na kolejny mecz.
- Jeśli ktoś trafi wynik – cała pula trafia do zwycięzcy.
- Jeśli trafi kilka osób – pula dzielona równo.
- Jeśli nikt nie trafi wyniku w ostatnim meczu turnieju – pula przepada.

### **Mechanizm płatności i rozliczeń**
- **System nie obsługuje żadnych transakcji ani rozliczeń pieniężnych online.** Wszystkie kwoty i pule są wyłącznie ewidencyjne.
- Gracze rozliczają się między sobą osobiście po zakończeniu turnieju.

### Statystyki i podgląd

- Każdy użytkownik może zobaczyć:
  - swoje statystyki (liczba rozegranych meczów, liczba obstawionych meczów, suma wygranych, liczba zwycięstw)
  - listę uczestników przy stole
  - typy obstawione przez innych graczy (w wersji podstawowej – jawne; wersja 2.0 – tryb „tajemniczego typera")
  - historyczne wyniki meczów, w tym kto trafił, a kto nie
- Jeśli użytkownik nie obstawił wyniku, nadal bierze udział w meczu i płaci stawkę, ale nie ma szansy na wygraną.

### Bezpieczeństwo

- Hasła (do konta i do stołu) muszą mieć min. 10 znaków.
- Dostęp do stołu chroniony hasłem.

---

## 4. Przypadki użycia / Scenariusze

### 1. Założenie nowego stołu do grania
**Aktor:** Nowy użytkownik  
**Scenariusz:**  
- Użytkownik wchodzi do systemu.
- Wybiera opcję „Załóż stół".
- Wypełnia formularz: login, hasło do konta (min. 10 znaków), maksymalna liczba graczy, cena za obstawienie, nazwa stołu, hasło do stołu (min. 10 znaków).
- Potwierdza założenie stołu.
- Zostaje automatycznie administratorem nowo utworzonego stołu.
- System umożliwia natychmiastowe zalogowanie do nowego stołu.

---

### 2. Dołączenie do istniejącego stołu
**Aktor:** Zarejestrowany użytkownik  
**Scenariusz:**  
- Użytkownik wybiera opcję „Dołącz do stołu".
- Wypełnia formularz: login, hasło do konta, nazwa stołu, hasło do stołu.
- Po poprawnej weryfikacji zostaje członkiem stołu.

---

### 3. Zmiana aktywnego stołu / Wylogowanie
**Aktor:** Użytkownik  
**Scenariusz:**  
- Użytkownik może wylogować się ze stołu.
- Po wylogowaniu może zalogować się do innego stołu, do którego należy.

---

### 4. Dodawanie meczu
**Aktor:** Administrator stołu  
**Scenariusz:**  
- Administrator wybiera zakładkę „Mecze".
- Wybiera opcję „Dodaj mecz".
- Wybiera kraje (autocomplete z bazy państw).
- Podaje datę i godzinę meczu.
- Zatwierdza mecz.

---

### 5. Obstawianie wyniku meczu
**Aktor:** Gracz  
**Scenariusz:**  
- Gracz widzi listę nadchodzących meczów.
- Wybiera mecz i podaje swój typ (wynik).
- Może dowolnie zmieniać typ do momentu rozpoczęcia meczu.
- Po rozpoczęciu meczu opcja typowania znika.

---

### 6. Wprowadzanie wyniku meczu
**Aktor:** Administrator stołu  
**Scenariusz:**  
- Po zakończeniu meczu administrator wybiera mecz z listy.
- Wprowadza oficjalny wynik.
- System automatycznie rozlicza pulę i wyświetla zwycięzców.

---

### 7. Rozliczanie puli
**Aktor:** System  
**Scenariusz:**  
- Po wprowadzeniu wyniku system sprawdza typy graczy.
- Jeśli nikt nie trafił – pula przechodzi na następny mecz.
- Jeśli jedna osoba trafiła – zgarnia całą pulę.
- Jeśli kilka osób trafiło – pula dzielona równo.
- Jeśli nikt nie trafił w ostatnim meczu turnieju – pula przepada.

---

### 8. Przekazanie roli administratora
**Aktor:** Administrator stołu  
**Scenariusz:**  
- Administrator wybiera gracza z listy uczestników stołu.
- Przekazuje mu uprawnienia administratora.
- **Nie można odebrać uprawnień ostatniemu administratorowi przy stole ani pozwolić mu opuścić stół.**

---

### 9. Odebranie uprawnień administratora
**Aktor:** Administrator stołu  
**Scenariusz:**  
- Administrator wybiera innego administratora z listy.
- Odbiera mu uprawnienia admina.
- **Nie można odebrać uprawnień, jeśli miałby zostać 0 administratorów przy stole.**

---

### 10. Podgląd statystyk i historii
**Aktor:** Gracz  
**Scenariusz:**  
- Gracz może zobaczyć swoje statystyki przy stole (liczba meczów, liczba obstawionych, wygrane, zwycięstwa).
- Może podejrzeć, jakie wyniki obstawiali inni uczestnicy (chyba że aktywny jest tryb „tajemniczego typera").
- Może sprawdzić wyniki historyczne meczów i zwycięzców.

---

### 11. Brak obstawienia meczu
**Aktor:** Gracz  
**Scenariusz:**  
- Gracz nie podał typu przed rozpoczęciem meczu.
- System odnotowuje brak typowania – gracz płaci stawkę, ale nie może wygrać puli.

---

## 5. Model danych (propozycja uproszczona)

### Kluczowe encje i relacje

#### **Użytkownik**
- id (unikalny identyfikator)
- login
- hasło (hash)
- lista stołów, do których należy

#### **Stół**
- id (unikalny identyfikator)
- nazwa stołu
- hasło do stołu (hash)
- maksymalna liczba graczy
- stawka za mecz
- lista graczy (relacja do Użytkownika)
- lista administratorów (relacja do Użytkownika, **ZAWSZE co najmniej jeden admin**)
- lista meczów

#### **Mecz**
- id (unikalny identyfikator)
- stół (relacja do Stół)
- kraj_1
- kraj_2
- data i godzina meczu
- wynik (opcjonalny, ustawiany po zakończeniu meczu)
- status (zaplanowany / rozegrany)
- lista typów (relacja do Typ)
- **flaga: czy rozpoczęty — po rozpoczęciu niedozwolona edycja**

#### **Typ**
- id (unikalny identyfikator)
- użytkownik (relacja do Użytkownika)
- mecz (relacja do Mecz)
- przewidywany wynik (np. 2:0)
- data i godzina ostatniej edycji

#### **Pula**
- id (unikalny identyfikator)
- mecz (relacja do Mecz)
- kwota
- status (wygrana, przechodzi, przepada)
- lista zwycięzców (relacja do Użytkownika; może być pusta)

#### **Statystyki**
- użytkownik (relacja do Użytkownika)
- stół (relacja do Stół)
- liczba meczów
- liczba obstawionych meczów
- liczba wygranych
- suma wygranych

---

**Relacje:**
- Użytkownik może należeć do wielu Stołów; Stół ma listę użytkowników.
- Stół ma wielu administratorów (**minimum jeden**).
- Stół ma wiele Meczów; Mecz należy do jednego Stołu.
- Mecz ma wiele Typów; Typ powiązany jest z jednym Użytkownikiem i jednym Mecz em.
- Mecz ma jedną Pulę; Pula dotyczy jednego meczu.
- Statystyki są prowadzone dla każdej pary Użytkownik-Stół.

---

## 6. Rozszerzenia i wersja 2.0

### Propozycje rozszerzeń funkcjonalności

1. **Tryb „tajemniczego typera"**
   - Po obstawieniu typy innych graczy są ukryte do momentu rozpoczęcia meczu.
   - Po rozpoczęciu meczu typy wszystkich graczy zostają ujawnione.
   - Opcja włączana przez administratora stołu.

2. **Wersja charytatywna**
   - Jeśli w ostatnim meczu turnieju pula nie zostanie wygrana, administrator może wskazać cel charytatywny, na który zostanie ona przeznaczona.
   - Dodanie historii celów charytatywnych i potwierdzeń przekazania środków.

3. **Powiadomienia**
   - E-mail/SMS/push o nadchodzących meczach, możliwości obstawienia, rozpoczęciu meczu, wygranej puli itp.

4. **Zaawansowane statystyki**
   - Ranking graczy przy stole, wykresy skuteczności, porównania z innymi stołami.

5. **Obsługa wielu dyscyplin sportowych**
   - Możliwość typowania nie tylko piłki nożnej, ale także innych sportów (siatkówka, tenis, koszykówka itd.).

6. **Integracja z API sportowymi**
   - Automatyczne pobieranie terminarza i wyników meczów z zewnętrznych źródeł.

7. **System komentarzy i czatu**
   - Możliwość komentowania meczów i wyników w obrębie stołu.

8. **Weryfikacja płatności**
   - Integracja z systemami płatności online w celu automatyzacji rozliczeń.

9. **Tryb turniejowy**
   - Obsługa rozbudowanych turniejów z własnymi zasadami (np. faza grupowa, drabinka pucharowa).

10. **Aplikacja mobilna**
    - Wersja aplikacji dedykowana na Android/iOS.

---

### Uwagi końcowe

Lista rozszerzeń jest otwarta – system można rozwijać według potrzeb użytkowników i specyfiki nowych wydarzeń sportowych. 