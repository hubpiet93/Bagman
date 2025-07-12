# Implementacja Domain-Driven Design (DDD) w Bagman API

## 🏗️ Architektura DDD

Implementacja API Bagman została zbudowana zgodnie z zasadami Domain-Driven Design, zapewniając:

- **Separację warstw** - Domain, Infrastructure, API
- **Encapsulation** - logika biznesowa w domenie
- **Value Objects** - niezmienne obiekty wartości
- **Aggregate Roots** - zarządzanie spójnością danych
- **Domain Events** - komunikacja między agregatami
- **Repository Pattern** - abstrakcja dostępu do danych

## 📁 Struktura projektu

```
src/
├── TypowanieMeczy.Api/           # Warstwa prezentacji
│   ├── Controllers/              # Kontrolery API
│   ├── Models/                   # DTOs
│   ├── Middleware/               # Custom middleware
│   └── Infrastructure/           # Konfiguracja DI
├── TypowanieMeczy.Domain/        # Warstwa domeny
│   ├── Entities/                 # Agregaty i encje
│   ├── ValueObjects/             # Obiekty wartości
│   ├── Interfaces/               # Interfejsy domenowe
│   ├── Services/                 # Serwisy domenowe
│   ├── Events/                   # Zdarzenia domenowe
│   └── Exceptions/               # Wyjątki domenowe
└── TypowanieMeczy.Infrastructure/ # Warstwa infrastruktury
    ├── Services/                 # Implementacje serwisów
    ├── Repositories/             # Implementacje repozytoriów
    └── Supabase/                 # Integracja z Supabase
```

## 🎯 Kluczowe komponenty DDD

### 1. Agregaty (Aggregate Roots)

#### **User Aggregate**
```csharp
public class User : AggregateRoot
{
    public UserId Id { get; private set; }
    public Login Login { get; private set; }
    public Email Email { get; private set; }
    
    // Metody domenowe
    public void JoinTable(Table table, bool isAdmin = false)
    public void LeaveTable(TableId tableId)
    public bool IsAdminOfTable(TableId tableId)
}
```

#### **Table Aggregate**
```csharp
public class Table : AggregateRoot
{
    public TableId Id { get; private set; }
    public TableName Name { get; private set; }
    public MaxPlayers MaxPlayers { get; private set; }
    
    // Metody domenowe
    public void AddMember(UserId userId, bool isAdmin = false)
    public void GrantAdminRole(UserId userId)
    public void AddMatch(Match match)
    public decimal CalculatePoolForMatch(MatchId matchId)
}
```

#### **Match Aggregate**
```csharp
public class Match : AggregateRoot
{
    public MatchId Id { get; private set; }
    public Country Country1 { get; private set; }
    public Country Country2 { get; private set; }
    
    // Metody domenowe
    public void PlaceBet(UserId userId, MatchPrediction prediction)
    public void StartMatch()
    public void FinishMatch(MatchResult result)
    private void CalculateWinners()
}
```

### 2. Value Objects

#### **Identity Value Objects**
```csharp
public record UserId
{
    public Guid Value { get; }
    public static UserId New() => new UserId(Guid.NewGuid());
    public static UserId FromString(string value) => new UserId(Guid.Parse(value));
}
```

#### **Business Value Objects**
```csharp
public record Login
{
    public string Value { get; }
    
    public Login(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Login cannot be empty");
        if (value.Length < 3)
            throw new ArgumentException("Login must be at least 3 characters");
        // ... walidacja
    }
}
```

### 3. Domain Services

#### **Table Service**
```csharp
public interface ITableService
{
    Task<Table> CreateTableAsync(TableName name, PasswordHash passwordHash, 
                                MaxPlayers maxPlayers, Stake stake, UserId createdBy);
    Task<Table> JoinTableAsync(TableName tableName, PasswordHash tablePassword, UserId userId);
    Task GrantAdminRoleAsync(TableId tableId, UserId targetUserId, UserId adminUserId);
}
```

#### **Match Service**
```csharp
public interface IMatchService
{
    Task<Match> CreateMatchAsync(TableId tableId, Country country1, Country country2, 
                                MatchDateTime matchDateTime, UserId createdBy);
    Task<Match> UpdateMatchResultAsync(MatchId matchId, MatchResult result, UserId adminUserId);
    Task StartMatchesAsync(); // Background service
}
```

### 4. Repository Pattern

#### **Interfaces**
```csharp
public interface ITableRepository
{
    Task<Table?> GetByIdAsync(TableId id);
    Task<Table?> GetByNameAsync(TableName name);
    Task<IEnumerable<Table>> GetByUserIdAsync(UserId userId);
    Task AddAsync(Table table);
    Task UpdateAsync(Table table);
}
```

### 5. Domain Events

```csharp
public class TableCreatedEvent : IDomainEvent
{
    public TableId TableId { get; }
    public TableName Name { get; }
    public UserId CreatedBy { get; }
}

public class BetPlacedEvent : IDomainEvent
{
    public MatchId MatchId { get; }
    public UserId UserId { get; }
    public MatchPrediction Prediction { get; }
}
```

## 🔄 Flow biznesowy

### 1. Tworzenie stołu
```
1. User -> CreateTableRequest
2. TablesController -> ITableService.CreateTableAsync()
3. TableService -> Table Aggregate (new Table())
4. Table Aggregate -> Domain Events (TableCreatedEvent)
5. Repository -> Save to Supabase
6. Response -> TableDto
```

### 2. Dodawanie meczu
```
1. Admin -> CreateMatchRequest
2. MatchesController -> IMatchService.CreateMatchAsync()
3. MatchService -> Validate admin permissions
4. MatchService -> Table Aggregate (AddMatch)
5. Match Aggregate -> Domain Events (MatchCreatedEvent)
6. Repository -> Save to Supabase
7. Response -> MatchDto
```

### 3. Typowanie wyniku
```
1. User -> PlaceBetRequest
2. BetsController -> IBetService.PlaceBetAsync()
3. BetService -> Match Aggregate (PlaceBet)
4. Match Aggregate -> Validate match not started
5. Match Aggregate -> Create/Update Bet
6. Match Aggregate -> Domain Events (BetPlacedEvent)
7. Repository -> Save to Supabase
8. Response -> BetDto
```

## 🛡️ Walidacja i bezpieczeństwo

### 1. Value Object Validation
```csharp
public record Password
{
    public string Value { get; }
    
    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password cannot be empty");
        if (value.Length < 10)
            throw new ArgumentException("Password must be at least 10 characters");
        // ... dodatkowa walidacja
    }
}
```

### 2. Business Rules
```csharp
public void AddMember(UserId userId, bool isAdmin = false)
{
    if (_memberships.Count >= MaxPlayers.Value)
        throw new TableFullException($"Table {Name.Value} is full");
    
    if (_memberships.Any(m => m.UserId == userId))
        throw new DomainException("User is already a member");
    
    // ... dodanie członka
}
```

### 3. Authorization
```csharp
public void AddMatch(Match match)
{
    if (!IsAdmin(match.CreatedBy))
        throw new UnauthorizedAccessException("Only admins can add matches");
    
    _matches.Add(match);
    AddDomainEvent(new MatchAddedEvent(Id, match.Id));
}
```

## 📊 Przykłady użycia

### Tworzenie stołu
```http
POST /api/v1/tables
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "name": "EURO 2024 - Grupa znajomych",
  "password": "min10characters",
  "maxPlayers": 20,
  "stake": 10.00
}
```

### Dodawanie meczu
```http
POST /api/v1/tables/{{tableId}}/matches
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "country1": "Poland",
  "country2": "Germany",
  "matchDateTime": "2024-06-15T20:00:00Z"
}
```

### Typowanie wyniku
```http
POST /api/v1/tables/{{tableId}}/matches/{{matchId}}/bets
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "prediction": "2:1"
}
```

## 🎯 Korzyści z DDD

1. **Separacja odpowiedzialności** - każda warstwa ma swoje zadanie
2. **Testowalność** - łatwe testowanie logiki biznesowej
3. **Maintainability** - kod łatwy w utrzymaniu i rozwijaniu
4. **Scalability** - możliwość łatwego skalowania
5. **Domain Expertise** - kod odzwierciedla ekspertyzę domenową
6. **Consistency** - spójne zasady biznesowe

## 🚀 Następne kroki

1. **Implementacja repozytoriów** - Supabase repositories
2. **Background services** - automatyczne startowanie/kończenie meczów
3. **Event handlers** - obsługa zdarzeń domenowych
4. **Caching** - Redis dla wydajności
5. **Monitoring** - logowanie i metryki
6. **Testing** - testy jednostkowe i integracyjne 