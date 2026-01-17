using System.Net;
using System.Text.Json;
using Bagman.Contracts.Models;
using FluentValidation;

namespace Bagman.Api.Middleware;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errors = ex.Errors
                .Select(error => new ValidationError
                {
                    ErrorCode = MapErrorCode(error.ErrorCode),
                    ErrorMessage = error.ErrorMessage,
                    ErrorSource = error.PropertyName
                })
                .ToList();

            var response = new ValidationErrorResponse
            {
                Errors = errors
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    private static string MapErrorCode(string fluentValidationCode)
    {
        return fluentValidationCode switch
        {
            "NotEmptyValidator" => ValidationErrorCode.Required.ToString(),
            "NotNullValidator" => ValidationErrorCode.Required.ToString(),
            "EmailValidator" => ValidationErrorCode.InvalidEmail.ToString(),
            "UrlValidator" => ValidationErrorCode.InvalidUrl.ToString(),
            "MinimumLengthValidator" => ValidationErrorCode.MinLength.ToString(),
            "MaximumLengthValidator" => ValidationErrorCode.MaxLength.ToString(),
            "LengthValidator" => ValidationErrorCode.MaxLength.ToString(),
            "RegularExpressionValidator" => ValidationErrorCode.PatternMismatch.ToString(),
            "EqualValidator" => ValidationErrorCode.InvalidFormat.ToString(),
            "NotEqualValidator" => ValidationErrorCode.InvalidFormat.ToString(),
            "GreaterThanValidator" => ValidationErrorCode.RangeError.ToString(),
            "GreaterThanOrEqualValidator" => ValidationErrorCode.RangeError.ToString(),
            "LessThanValidator" => ValidationErrorCode.RangeError.ToString(),
            "LessThanOrEqualValidator" => ValidationErrorCode.RangeError.ToString(),
            "InclusiveBetweenValidator" => ValidationErrorCode.RangeError.ToString(),
            "ExclusiveBetweenValidator" => ValidationErrorCode.RangeError.ToString(),
            "EnumValidator" => ValidationErrorCode.InvalidFormat.ToString(),
            _ => ValidationErrorCode.Other.ToString()
        };
    }
}
