using System.Net;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Bagman.Api.Controllers;

public abstract class AppControllerBase : ControllerBase
{
    [NonAction]
    protected virtual IActionResult BadRequest([ActionResultObjectValue] IErrorOr errorOr)
    {
        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(HttpContext, (int)HttpStatusCode.BadRequest);

        if (errorOr.Errors != null)
            problemDetails.Extensions["errors"] = errorOr.Errors.Select(e =>
                new
                {
                    ErrorCode = e.Code,
                    e.Description
                });

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }

    [NonAction]
    protected virtual IActionResult BadRequest([ActionResultObjectValue] IList<Error> errors)
    {
        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(HttpContext, (int)HttpStatusCode.BadRequest);

        problemDetails.Extensions["errors"] = errors.Select(e =>
            new
            {
                ErrorCode = e.Code,
                e.Description
            });

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }
}
