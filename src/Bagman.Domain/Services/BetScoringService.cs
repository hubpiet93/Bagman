using Bagman.Domain.ValueObjects;

namespace Bagman.Domain.Services;

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
