# Plan implementacji Leaderboard w Dashboard

## Cel

Rozszerzyć endpoint `GET /api/tables/{tableId}/dashboard` o pole `leaderboard` zawierające ranking użytkowników z obliczonymi punktami za trafienia.

---

## Obecny stan

### Istniejące struktury

**UserStats** (śledzenie statystyk):
```csharp
public class UserStats
{
    public Guid UserId { get; set; }
    public Guid TableId { get; set; }
    public int MatchesPlayed { get; set; }
    public int BetsPlaced { get; set; }
    public int PoolsWon { get; set; }
    public decimal TotalWon { get; set; }
}
```

**Dashboard Response** (aktualny):
```json
{
  "table": { ... },
  "members": [ ... ],
  "matches": [ ... ],
  "bets": [ ... ],
  "pools": [ ... ],
  "stats": [
    { "userId": "...", "matchesPlayed": 5, "betsPlaced": 5, "poolsWon": 2, "totalWon": 150.0 }
  ]
}
```

### Problem

- `Stats` śledzi tylko pule wygranych (`PoolsWon`, `TotalWon`)
- Brak punktów za trafienia wyników meczów
- Brak posortowanego rankingu

---

## Proponowane rozwiązanie

### System punktacji

| Typ trafienia | Punkty | Opis |
|---------------|--------|------|
| Dokładny wynik | 3 pkt | Prediction = Result (np. "2:1" = "2:1") |
| Trafiony zwycięzca | 1 pkt | Prawidłowy znak (1/X/2), ale inny wynik |
| Trafiony remis | 1 pkt | Prediction = "X" i Result jest remisem (np. "1:1") |
| Pudło | 0 pkt | Błędny typ |

### Docelowy response

```json
{
  "table": { ... },
  "members": [ ... ],
  "matches": [ ... ],
  "bets": [ ... ],
  "pools": [ ... ],
  "stats": [ ... ],
  "leaderboard": [
    {
      "position": 1,
      "userId": "guid-1",
      "login": "player1",
      "points": 15,
      "exactHits": 4,
      "winnerHits": 3,
      "totalBets": 10,
      "accuracy": 70.0
    },
    {
      "position": 2,
      "userId": "guid-2",
      "login": "player2",
      "points": 12,
      "exactHits": 3,
      "winnerHits": 3,
      "totalBets": 10,
      "accuracy": 60.0
    }
  ]
}
```

---

## Plan implementacji

### Faza 1: Domain Layer

#### 1.1 Dodanie Value Object dla punktacji

**Plik:** `src/Bagman.Domain/ValueObjects/BetResult.cs`

```csharp
public enum BetResultType
{
    Pending,      // Mecz nie zakończony
    ExactHit,     // Dokładne trafienie (3 pkt)
    WinnerHit,    // Trafiony zwycięzca/remis (1 pkt)
    Miss          // Pudło (0 pkt)
}

public record BetResult(BetResultType Type, int Points)
{
    public static BetResult Pending => new(BetResultType.Pending, 0);
    public static BetResult ExactHit => new(BetResultType.ExactHit, 3);
    public static BetResult WinnerHit => new(BetResultType.WinnerHit, 1);
    public static BetResult Miss => new(BetResultType.Miss, 0);
}
```

#### 1.2 Dodanie logiki obliczania wyniku

**Plik:** `src/Bagman.Domain/Services/BetScoringService.cs`

```csharp
public static class BetScoringService
{
    public static BetResult CalculateResult(string prediction, string? matchResult)
    {
        if (matchResult == null)
            return BetResult.Pending;

        // Dokładne trafienie
        if (prediction == matchResult)
            return BetResult.ExactHit;

        // Parsowanie wyników
        var (pred1, pred2) = ParseScore(prediction);
        var (res1, res2) = ParseScore(matchResult);

        // Sprawdzenie znaku (1/X/2)
        var predSign = GetSign(pred1, pred2);
        var resSign = GetSign(res1, res2);

        if (predSign == resSign)
            return BetResult.WinnerHit;

        return BetResult.Miss;
    }

    private static (int, int) ParseScore(string score)
    {
        if (score == "X") return (0, 0); // Remis - special case
        var parts = score.Split(':');
        return (int.Parse(parts[0]), int.Parse(parts[1]));
    }

    private static int GetSign(int score1, int score2)
    {
        if (score1 > score2) return 1;  // Wygrana gospodarzy
        if (score1 < score2) return 2;  // Wygrana gości
        return 0;                        // Remis
    }
}
```

---

### Faza 2: Application Layer

#### 2.1 Nowy model wyniku dla leaderboard

**Plik:** `src/Bagman.Application/Features/Tables/GetTableDashboard/LeaderboardEntryResult.cs`

```csharp
public record LeaderboardEntryResult
{
    public int Position { get; init; }
    public Guid UserId { get; init; }
    public string Login { get; init; } = string.Empty;
    public int Points { get; init; }
    public int ExactHits { get; init; }
    public int WinnerHits { get; init; }
    public int TotalBets { get; init; }
    public double Accuracy { get; init; }
}
```

#### 2.2 Rozszerzenie TableDashboardResult

**Plik:** `src/Bagman.Application/Features/Tables/GetTableDashboard/TableDashboardResult.cs`

Dodać pole:
```csharp
public required List<LeaderboardEntryResult> Leaderboard { get; init; }
```

#### 2.3 Modyfikacja Handler'a

**Plik:** `src/Bagman.Application/Features/Tables/GetTableDashboard/Handler.cs`

Dodać metodę:

```csharp
private List<LeaderboardEntryResult> CalculateLeaderboard(
    List<MemberResult> members,
    List<MatchDetailResult> matches,
    List<BetDetailResult> bets)
{
    // Słownik: matchId -> result
    var matchResults = matches
        .Where(m => m.Result != null)
        .ToDictionary(m => m.Id, m => m.Result!);

    // Grupowanie zakładów po użytkowniku
    var userBets = bets.GroupBy(b => b.UserId);

    var leaderboard = new List<LeaderboardEntryResult>();

    foreach (var group in userBets)
    {
        var userId = group.Key;
        var member = members.FirstOrDefault(m => m.UserId == userId);
        if (member == null) continue;

        int points = 0;
        int exactHits = 0;
        int winnerHits = 0;
        int totalBets = 0;

        foreach (var bet in group)
        {
            if (!matchResults.TryGetValue(bet.MatchId, out var result))
                continue; // Mecz jeszcze nie zakończony

            totalBets++;
            var betResult = BetScoringService.CalculateResult(bet.Prediction, result);

            points += betResult.Points;
            if (betResult.Type == BetResultType.ExactHit) exactHits++;
            if (betResult.Type == BetResultType.WinnerHit) winnerHits++;
        }

        var accuracy = totalBets > 0
            ? Math.Round((exactHits + winnerHits) * 100.0 / totalBets, 1)
            : 0;

        leaderboard.Add(new LeaderboardEntryResult
        {
            UserId = userId,
            Login = member.Login,
            Points = points,
            ExactHits = exactHits,
            WinnerHits = winnerHits,
            TotalBets = totalBets,
            Accuracy = accuracy
        });
    }

    // Sortowanie i przypisanie pozycji
    return leaderboard
        .OrderByDescending(e => e.Points)
        .ThenByDescending(e => e.ExactHits)
        .ThenByDescending(e => e.Accuracy)
        .Select((e, i) => e with { Position = i + 1 })
        .ToList();
}
```

W metodzie `Handle()` dodać wywołanie:
```csharp
var leaderboard = CalculateLeaderboard(members, matches, bets);

return new TableDashboardResult
{
    // ... existing fields
    Leaderboard = leaderboard
};
```

---

### Faza 3: Contracts Layer

#### 3.1 Nowy model response

**Plik:** `src/Bagman.Contracts/Models/TableDashboardResponse.cs`

Dodać:
```csharp
public record LeaderboardEntry
{
    public required int Position { get; init; }
    public required Guid UserId { get; init; }
    public required string Login { get; init; }
    public required int Points { get; init; }
    public required int ExactHits { get; init; }
    public required int WinnerHits { get; init; }
    public required int TotalBets { get; init; }
    public required double Accuracy { get; init; }
}
```

W `TableDashboardResponse` dodać:
```csharp
public required List<LeaderboardEntry> Leaderboard { get; init; }
```

---

### Faza 4: API Layer

#### 4.1 Rozszerzenie mappera

**Plik:** `src/Bagman.Api/Controllers/Mappers/TablesControllerMappers.cs`

Dodać:
```csharp
public static LeaderboardEntry ToLeaderboardEntry(this LeaderboardEntryResult result)
{
    return new LeaderboardEntry
    {
        Position = result.Position,
        UserId = result.UserId,
        Login = result.Login,
        Points = result.Points,
        ExactHits = result.ExactHits,
        WinnerHits = result.WinnerHits,
        TotalBets = result.TotalBets,
        Accuracy = result.Accuracy
    };
}
```

W `ToTableDashboardResponse()` dodać:
```csharp
Leaderboard = result.Leaderboard.Select(l => l.ToLeaderboardEntry()).ToList()
```

---

### Faza 5: Testy

#### 5.1 Testy jednostkowe

**Plik:** `tests/Bagman.UnitTests/Domain/BetScoringServiceTests.cs`

```csharp
public class BetScoringServiceTests
{
    [Theory]
    [InlineData("2:1", "2:1", BetResultType.ExactHit, 3)]   // Dokładne
    [InlineData("3:0", "2:1", BetResultType.WinnerHit, 1)]  // Trafiony zwycięzca
    [InlineData("1:1", "0:0", BetResultType.WinnerHit, 1)]  // Trafiony remis
    [InlineData("X", "2:2", BetResultType.WinnerHit, 1)]    // X = remis
    [InlineData("2:1", "0:1", BetResultType.Miss, 0)]       // Pudło
    [InlineData("2:1", null, BetResultType.Pending, 0)]     // Mecz nierozegrany
    public void CalculateResult_ReturnsCorrectResult(
        string prediction, string? result, BetResultType expectedType, int expectedPoints)
    {
        var betResult = BetScoringService.CalculateResult(prediction, result);

        Assert.Equal(expectedType, betResult.Type);
        Assert.Equal(expectedPoints, betResult.Points);
    }
}
```

#### 5.2 Test integracyjny

**Plik:** `tests/Bagman.IntegrationTests/Controllers/TablesControllerTests.cs`

Dodać test:
```csharp
[Fact]
public async Task GetTableDashboard_WithFinishedMatches_ReturnsLeaderboard()
{
    // Arrange: Stworzyć stół, członków, mecze z wynikami, zakłady
    // Act: GET /api/tables/{tableId}/dashboard
    // Assert: Sprawdzić że leaderboard jest posortowany i ma prawidłowe punkty
}
```

---

## Struktura plików do modyfikacji/utworzenia

### Nowe pliki:
```
src/Bagman.Domain/ValueObjects/BetResult.cs
src/Bagman.Domain/Services/BetScoringService.cs
src/Bagman.Application/Features/Tables/GetTableDashboard/LeaderboardEntryResult.cs
tests/Bagman.UnitTests/Domain/BetScoringServiceTests.cs
```

### Modyfikowane pliki:
```
src/Bagman.Application/Features/Tables/GetTableDashboard/TableDashboardResult.cs
src/Bagman.Application/Features/Tables/GetTableDashboard/Handler.cs
src/Bagman.Contracts/Models/TableDashboardResponse.cs
src/Bagman.Api/Controllers/Mappers/TablesControllerMappers.cs
tests/Bagman.IntegrationTests/Controllers/TablesControllerTests.cs
```

---

## Kolejność implementacji

1. **Domain Layer** - BetResult, BetScoringService (+ testy jednostkowe)
2. **Application Layer** - LeaderboardEntryResult, rozszerzenie TableDashboardResult i Handler
3. **Contracts Layer** - LeaderboardEntry, rozszerzenie TableDashboardResponse
4. **API Layer** - rozszerzenie mappera
5. **Testy integracyjne** - weryfikacja całego flow

---

## Uwagi

### Obsługa "X" jako prediction

Prediction "X" oznacza remis. Przy porównaniu:
- `"X"` vs `"1:1"` → WinnerHit (trafiony remis)
- `"X"` vs `"0:0"` → WinnerHit (trafiony remis)
- `"X"` vs `"2:1"` → Miss (nie był remis)

### Edge cases

1. **Użytkownik bez zakładów** - nie pojawia się w leaderboard (lub z 0 punktów)
2. **Brak zakończonych meczów** - leaderboard pusty lub wszyscy z 0 punktów
3. **Remisy w punktach** - sortowanie po: Points → ExactHits → Accuracy

### Przyszłe rozszerzenia

1. **Konfigurowalny system punktacji** - punkty mogą być ustawiane per EventType
2. **Bonus points** - dodatkowe punkty za serię trafień
3. **Historyczny leaderboard** - ranking z poprzednich edycji EventType
