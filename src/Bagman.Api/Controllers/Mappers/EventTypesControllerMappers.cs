using Bagman.Application.Features.EventTypes.CreateEventType;
using Bagman.Application.Features.EventTypes.UpdateEventType;
using Bagman.Contracts.Models;
using ActiveEventTypeDto = Bagman.Application.Features.EventTypes.GetActiveEventTypes.EventTypeDto;
using ActiveGetEventTypesResult = Bagman.Application.Features.EventTypes.GetActiveEventTypes.GetEventTypesResult;
using AllEventTypeDto = Bagman.Application.Features.EventTypes.GetAllEventTypes.EventTypeDto;
using AllGetEventTypesResult = Bagman.Application.Features.EventTypes.GetAllEventTypes.GetEventTypesResult;

namespace Bagman.Api.Controllers.Mappers;

public static class EventTypesControllerMappers
{
    public static EventTypeResponse ToEventTypeResponse(this ActiveEventTypeDto dto)
    {
        return new EventTypeResponse(
            dto.Id,
            dto.Code,
            dto.Name,
            dto.StartDate,
            dto.IsActive
        );
    }

    public static EventTypeResponse ToEventTypeResponse(this AllEventTypeDto dto)
    {
        return new EventTypeResponse(
            dto.Id,
            dto.Code,
            dto.Name,
            dto.StartDate,
            dto.IsActive
        );
    }

    public static EventTypeResponse ToEventTypeResponse(this CreateEventTypeResult result)
    {
        return new EventTypeResponse(
            result.Id,
            result.Code,
            result.Name,
            result.StartDate,
            result.IsActive
        );
    }

    public static EventTypeResponse ToEventTypeResponse(this UpdateEventTypeResult result)
    {
        return new EventTypeResponse(
            result.Id,
            result.Code,
            result.Name,
            result.StartDate,
            result.IsActive
        );
    }

    public static EventTypeListResponse ToEventTypeListResponse(this ActiveGetEventTypesResult result)
    {
        return new EventTypeListResponse(
            result.EventTypes.Select(et => et.ToEventTypeResponse()).ToList()
        );
    }

    public static EventTypeListResponse ToEventTypeListResponse(this AllGetEventTypesResult result)
    {
        return new EventTypeListResponse(
            result.EventTypes.Select(et => et.ToEventTypeResponse()).ToList()
        );
    }
}
