using System.Security.Claims;
using Bagman.Api.Controllers.Mappers;
using Bagman.Application.Common;
using Bagman.Application.Features.Matches.GetMatchDetails;
using Bagman.Contracts.Models.Tables;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/tables/{tableId}/matches")]
[Authorize]
public class MatchesController : AppControllerBase
{
    private readonly FeatureDispatcher _dispatcher;

    public MatchesController(FeatureDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    ///     Pobranie szczegółów meczu
    /// </summary>
    [HttpGet("{matchId}")]
    public async Task<IActionResult> GetMatchDetails(Guid tableId, Guid matchId)
    {
        var result = await _dispatcher.HandleAsync<GetMatchDetailsQuery, MatchDetailsResult>(
            new GetMatchDetailsQuery { MatchId = matchId });
        
        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(result.Value.ToMatchResponse());
    }
}
