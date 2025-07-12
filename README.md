# Bagman - System do typowania meczów piłki nożnej

System do obstawiania meczów piłki nożnej dla prywatnych grup znajomych podczas okazjonalnych wydarzeń sportowych (EURO, Mistrzostwa Świata).

## 🎯 O projekcie

Bagman to aplikacja webowa umożliwiająca organizowanie prywatnych zakładów wśród zamkniętej grupy znajomych. Użytkownicy mogą zakładać własne "stoły do grania", zapraszać znajomych i wspólnie typować wyniki meczów turniejowych.

### Kluczowe funkcje

- 🏠 **Prywatne stoły do grania** - każdy stół ma własną konfigurację i hasło
- 👥 **Zarządzanie graczami** - możliwość dołączania do wielu stołów
- ⚽ **Typowanie meczów** - obstawianie wyników do momentu rozpoczęcia meczu
- 💰 **System puli** - automatyczne rozliczanie wygranych i rollover niewygranych kwot
- 📊 **Statystyki** - podgląd wyników i historii meczów
- 🔐 **Bezpieczeństwo** - hasła min. 10 znaków, autoryzacja JWT

## 🏗️ Architektura

- **Frontend**: Blazor WebAssembly + MudBlazor
- **Backend**: ASP.NET Core API (.NET 8)
- **Baza danych**: Supabase (PostgreSQL)
- **Autoryzacja**: Supabase Auth

## 📚 Dokumentacja

### [Koncept aplikacji](docs/bet-concept.md)
Początkowe założenia i pomysły na funkcjonalności systemu.

### [Wymagania systemowe](docs/system-requirements.md)
Szczegółowa dokumentacja wymagań funkcjonalnych, przypadków użycia i modelu danych.

### [Dokumentacja techniczna](docs/technical-documentation.md)
Specyfikacja techniczna, architektura, stack technologiczny i instrukcje wdrożenia.

## 🚀 Rozpoczęcie pracy

### Wymagania

- .NET 8 SDK
- Konto Supabase
- Node.js (opcjonalnie)

### Instalacja

1. Sklonuj repozytorium
2. Skonfiguruj połączenie z Supabase (patrz dokumentacja techniczna)
3. Uruchom migracje bazy danych
4. Uruchom API i aplikację Blazor

Szczegółowe instrukcje znajdują się w [dokumentacji technicznej](docs/technical-documentation.md).

## 🎨 UI/UX

Aplikacja wykorzystuje MudBlazor do stworzenia nowoczesnego, responsywnego interfejsu z:
- Nawigacją w formie kart i paneli
- Mobile-first design
- Jasnym motywem z dużymi odstępami
- Zaokrąglonymi elementami

## 🔮 Rozszerzenia (Wersja 2.0)

- Tryb "tajemniczego typera"
- Powiadomienia email/SMS
- Wersja charytatywna
- Integracja z zewnętrznymi API sportowymi
- System komentarzy i czatu
- Aplikacja mobilna

## 🤝 Współpraca

Projekt jest w fazie planowania i dokumentacji. Wszystkie sugestie i propozycje są mile widziane!

## 📄 Licencja

Projekt prywatny - do użytku osobistego i wśród znajomych.

---

**Uwaga**: System nie obsługuje transakcji pieniężnych online. Wszystkie kwoty są ewidencyjne, a rozliczenia odbywają się osobiście między graczami. 