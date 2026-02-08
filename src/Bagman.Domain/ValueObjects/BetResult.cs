namespace Bagman.Domain.ValueObjects;

public enum BetResultType
{
    Pending, // Mecz nie zakończony
    ExactHit, // Dokładne trafienie (3 pkt)
    WinnerHit, // Trafiony zwycięzca/remis (1 pkt)
    Miss // Pudło (0 pkt)
}

public record BetResult(BetResultType Type, int Points)
{
    public static BetResult Pending => new(BetResultType.Pending, 0);
    public static BetResult ExactHit => new(BetResultType.ExactHit, 3);
    public static BetResult WinnerHit => new(BetResultType.WinnerHit, 1);
    public static BetResult Miss => new(BetResultType.Miss, 0);
}
