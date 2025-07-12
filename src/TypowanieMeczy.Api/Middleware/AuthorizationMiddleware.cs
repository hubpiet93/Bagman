using Microsoft.AspNetCore.Http;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.ValueObjects;
using System.Security.Claims;
using TypowanieMeczy.Domain.Services;

namespace TypowanieMeczy.Api.Middleware;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITableService tableService)
    {
        // Skip authorization for public endpoints
        if (IsPublicEndpoint(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Check if user is authenticated
        var userId = GetUserIdFromClaims(context.User);
        if (userId == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        // Add user ID to context for controllers to use
        context.Items["UserId"] = userId;

        await _next(context);
    }

    private bool IsPublicEndpoint(PathString path)
    {
        var publicPaths = new[]
        {
            "/swagger",
            "/health",
            "/api/auth/login",
            "/api/auth/register"
        };

        return publicPaths.Any(publicPath => path.StartsWithSegments(publicPath));
    }

    private UserId? GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return null;

        return UserId.FromString(userIdClaim);
    }
}

public static class AuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthorizationMiddleware>();
    }
} 