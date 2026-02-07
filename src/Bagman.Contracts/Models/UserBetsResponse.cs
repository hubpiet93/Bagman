namespace Bagman.Contracts.Models;

using Bagman.Contracts.Models.Tables;

public record UserBetsResponse(List<BetResponse> Bets);
