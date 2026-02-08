using Bagman.Api.Attributes;
using Bagman.Api.Controllers.Mappers;
using Bagman.Application.Common;
using Bagman.Application.Features.EventTypes.CreateEventType;
using Bagman.Application.Features.EventTypes.DeactivateEventType;
using Bagman.Application.Features.EventTypes.UpdateEventType;
using Bagman.Contracts.Models;
using Bagman.Contracts.Models.EventTypes;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ActiveGetEventTypesQuery = Bagman.Application.Features.EventTypes.GetActiveEventTypes.GetActiveEventTypesQuery;
using ActiveGetEventTypesResult = Bagman.Application.Features.EventTypes.GetActiveEventTypes.GetEventTypesResult;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/event-types")]
public class EventTypesController : AppControllerBase
{
    private readonly FeatureDispatcher _dispatcher;

    public EventTypesController(FeatureDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    ///     Get all active event types (public endpoint)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveEventTypes()
    {
        var result = await _dispatcher.HandleAsync<ActiveGetEventTypesQuery, ActiveGetEventTypesResult>(
            new ActiveGetEventTypesQuery());

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(result.Value.ToEventTypeListResponse());
    }

    /// <summary>
    ///     Create new event type (SuperAdmin only)
    /// </summary>
    [HttpPost]
    [Route("/api/admin/event-types")]
    [Authorize]
    [SuperAdminOnly]
    public async Task<IActionResult> CreateEventType([FromBody] CreateEventTypeRequest request)
    {
        var command = new CreateEventTypeCommand
        {
            Code = request.Code,
            Name = request.Name,
            StartDate = request.StartDate
        };

        var result = await _dispatcher.HandleAsync<CreateEventTypeCommand, CreateEventTypeResult>(command);

        if (result.IsError)
            return MapErrors(result.Errors);

        return CreatedAtAction(
            nameof(GetActiveEventTypes),
            new {id = result.Value.Id},
            result.Value.ToEventTypeResponse());
    }

    /// <summary>
    ///     Update event type (SuperAdmin only)
    /// </summary>
    [HttpPut]
    [Route("/api/admin/event-types/{id:guid}")]
    [Authorize]
    [SuperAdminOnly]
    public async Task<IActionResult> UpdateEventType(Guid id, [FromBody] UpdateEventTypeRequest request)
    {
        var command = new UpdateEventTypeCommand
        {
            Id = id,
            Name = request.Name,
            StartDate = request.StartDate
        };

        var result = await _dispatcher.HandleAsync<UpdateEventTypeCommand, UpdateEventTypeResult>(command);

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(result.Value.ToEventTypeResponse());
    }

    /// <summary>
    ///     Deactivate event type (SuperAdmin only, soft delete)
    /// </summary>
    [HttpPost]
    [Route("/api/admin/event-types/{id:guid}/deactivate")]
    [Authorize]
    [SuperAdminOnly]
    public async Task<IActionResult> DeactivateEventType(Guid id)
    {
        var command = new DeactivateEventTypeCommand {Id = id};
        var result = await _dispatcher.HandleAsync<DeactivateEventTypeCommand, Success>(command);

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new SuccessResponse("Typ wydarzenia został dezaktywowany"));
    }
}
