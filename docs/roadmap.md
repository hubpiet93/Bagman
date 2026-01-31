# Roadmap rozwoju - Bagman

## Faza 1: Podstawowa funkcjonalność (MVP) - 4-6 tygodni

### Priorytet 1: Infrastruktura i autoryzacja
- [ ] Konfiguracja projektu Blazor WebAssembly + ASP.NET Core API
- [ ] Integracja z Supabase (baza danych + autoryzacja)
- [ ] Podstawowe modele danych i migracje
- [ ] System logowania i rejestracji użytkowników
- [ ] Podstawowy layout UI z MudBlazor

### Priorytet 2: Zarządzanie stołami
- [ ] Tworzenie nowego stołu do grania
- [ ] Dołączanie do istniejącego stołu
- [ ] Zarządzanie członkami stołu
- [ ] System uprawnień administratora
- [ ] Podstawowy dashboard stołu

### Priorytet 3: Zarządzanie meczami
- [ ] Dodawanie meczów przez administratora
- [ ] Autocomplete krajów
- [ ] Walidacja dat i godzin meczów
- [ ] Lista nadchodzących i zakończonych meczów
- [ ] Wprowadzanie wyników meczów

### Priorytet 4: System typowania
- [ ] Interfejs do obstawiania wyników
- [ ] Walidacja typów (format wyniku)
- [ ] Blokada typowania po rozpoczęciu meczu
- [ ] Historia typów użytkownika
- [ ] Podgląd typów innych graczy

### Priorytet 5: System puli i rozliczeń
- [ ] Automatyczne obliczanie puli
- [ ] Logika rollover (przechodzenie puli)
- [ ] Rozliczanie wygranych
- [ ] Podział puli między zwycięzców
- [ ] Historia pul i wygranych

## Faza 2: Rozszerzenia i ulepszenia - 3-4 tygodnie

### Priorytet 6: Statystyki i raporty
- [ ] Statystyki użytkownika per stół
- [ ] Ranking graczy
- [ ] Historia meczów i wyników
- [ ] Eksport danych
- [ ] Wykresy i wizualizacje

### Priorytet 7: Ulepszenia UX
- [ ] Responsywny design mobile
- [ ] Powiadomienia o nadchodzących meczach
- [ ] Animacje i przejścia
- [ ] Dark/Light mode
- [ ] Dostępność (WCAG 2.1)

### Priorytet 8: Bezpieczeństwo i walidacja
- [ ] Weryfikacja uprawnień
- [ ] Rate limiting
- [ ] Walidacja danych po stronie klienta i serwera
- [ ] Logowanie zdarzeń
- [ ] Backup i recovery

## Faza 3: Funkcje zaawansowane (Wersja 2.0) - 4-6 tygodni

### Priorytet 9: Tryb "tajemniczego typera"
- [ ] Flaga w konfiguracji stołu
- [ ] Ukrywanie typów przed rozpoczęciem meczu
- [ ] Ujawnianie typów po rozpoczęciu
- [ ] UI dla trybu tajemniczego

### Priorytet 10: Powiadomienia
- [ ] Integracja z SendGrid (email)
- [ ] Powiadomienia push (PWA)
- [ ] Konfiguracja powiadomień
- [ ] Szablony wiadomości

### Priorytet 11: Integracje zewnętrzne
- [ ] API-Football dla danych meczów
- [ ] Automatyczne pobieranie terminarza
- [ ] Synchronizacja wyników
- [ ] Cache'owanie danych

### Priorytet 12: System komentarzy
- [ ] Komentarze do meczów
- [ ] Czat w czasie rzeczywistym (Supabase Realtime)
- [ ] Moderacja komentarzy
- [ ] Powiadomienia o nowych komentarzach