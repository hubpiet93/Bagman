using Bagman.Contracts.Models.Tables;

namespace Bagman.Contracts.Models;

public record UserBetsResponse(List<BetResponse> Bets);
