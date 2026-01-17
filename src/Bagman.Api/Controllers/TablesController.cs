using Bagman.Contracts.Models.Tables;
using Bagman.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TablesController : AppControllerBase
{
    private readonly ITableService _tableService;
    private readonly IAuthService _authService;

    public TablesController(ITableService tableService, IAuthService authService)
    {
        _tableService = tableService;
        _authService = authService;
    }

    /// <summary>
    ///     Utworzenie nowego stołu z automatyczną rejestracją użytkownika
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateTable([FromBody] CreateTableRequest request)
    {
        // Register user first. If user already exists, try to login (tests may reuse logins).
        var registerResult = await _authService.RegisterAsync(request.UserLogin, request.UserPassword, $"{request.UserLogin}@bagman.local");
        Guid userId;
        if (registerResult.IsError)
        {
            // If user already exists, attempt to login with provided credentials
            if (registerResult.FirstError.Code == "User.AlreadyExists")
            {
                var loginResult = await _authService.LoginAsync(request.UserLogin, request.UserPassword);
                if (loginResult.IsError)
                    return MapErrors(loginResult.Errors);

                userId = loginResult.Value.User.Id;
            }
            else
            {
                return MapErrors(registerResult.Errors);
            }
        }
        else
        {
            userId = registerResult.Value.User.Id;
        }

        // Create table
        var tableResult = await _tableService.CreateTableAsync(
            request.TableName,
            request.TablePassword, // In production, hash this password
            request.MaxPlayers,
            request.Stake,
            userId
        );

        if (tableResult.IsError)
            return MapErrors(tableResult.Errors);

        var createdResponse = new TableResponse
        {
            Id = tableResult.Value.Id,
            Name = tableResult.Value.Name,
            MaxPlayers = tableResult.Value.MaxPlayers,
            Stake = tableResult.Value.Stake,
            CreatedBy = tableResult.Value.CreatedBy,
            CreatedAt = tableResult.Value.CreatedAt,
            IsSecretMode = tableResult.Value.IsSecretMode
        };

        return CreatedAtAction(nameof(GetTableDetails), new { tableId = tableResult.Value.Id }, createdResponse);
    }

    /// <summary>
    ///     Dołączenie do istniejącego stołu
    /// </summary>
    [HttpPost("join")]
    [AllowAnonymous]
    public async Task<IActionResult> JoinTable([FromBody] JoinTableRequest request)
    {
        // Authenticate or register user
        var loginResult = await _authService.LoginAsync(request.UserLogin, request.UserPassword);
        var userId = loginResult.IsError
            ? (await _authService.RegisterAsync(request.UserLogin, request.UserPassword, $"{request.UserLogin}@bagman.local")).Value.User.Id
            : loginResult.Value.User.Id;

        // Get table by name
        var getTableResult = await _tableService.GetTableByNameAsync(request.TableName);
        if (getTableResult.IsError)
            return MapErrors(getTableResult.Errors);

        if (getTableResult.Value == null)
            return NotFound(new { message = "Stół nie został znaleziony" });

        var tableId = getTableResult.Value.Id;

        // Join table
        var joinResult = await _tableService.JoinTableAsync(tableId, userId, request.TablePassword);
        if (joinResult.IsError)
            return MapErrors(joinResult.Errors);

        // Get updated table details
        var tableDetailsResult = await _tableService.GetTableByIdAsync(tableId);
        if (tableDetailsResult.IsError)
            return MapErrors(tableDetailsResult.Errors);

        var table = tableDetailsResult.Value;
        return Ok(new TableResponse
        {
            Id = table!.Id,
            Name = table.Name,
            MaxPlayers = table.MaxPlayers,
            Stake = table.Stake,
            CreatedBy = table.CreatedBy,
            CreatedAt = table.CreatedAt,
            IsSecretMode = table.IsSecretMode
        });
    }

    /// <summary>
    ///     Pobranie listy stołów użytkownika
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetUserTables()
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _tableService.GetUserTablesAsync(userId.Value);
        if (result.IsError)
            return MapErrors(result.Errors);

        var response = result.Value.Select(t => new TableResponse
        {
            Id = t.Id,
            Name = t.Name,
            MaxPlayers = t.MaxPlayers,
            Stake = t.Stake,
            CreatedBy = t.CreatedBy,
            CreatedAt = t.CreatedAt,
            IsSecretMode = t.IsSecretMode
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    ///     Pobranie szczegółów stołu
    /// </summary>
    [HttpGet("{tableId}")]
    public async Task<IActionResult> GetTableDetails(Guid tableId)
    {
        var result = await _tableService.GetTableByIdAsync(tableId);
        if (result.IsError)
            return MapErrors(result.Errors);

        if (result.Value == null)
            return NotFound();

        var table = result.Value;
        var response = new TableDetailResponse
        {
            Id = table.Id,
            Name = table.Name,
            MaxPlayers = table.MaxPlayers,
            Stake = table.Stake,
            CreatedAt = table.CreatedAt,
            Members = table.Members.Select(m => new TableMemberResponse
            {
                UserId = m.UserId,
                Login = m.User?.Login ?? "Unknown",
                IsAdmin = m.IsAdmin,
                JoinedAt = m.JoinedAt
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    ///     Pobranie dashboard stołu (wszystkie informacje)
    /// </summary>
    [HttpGet("{tableId}/dashboard")]
    public async Task<IActionResult> GetTableDashboard(Guid tableId)
    {
        var result = await _tableService.GetTableByIdAsync(tableId);
        if (result.IsError)
            return MapErrors(result.Errors);

        if (result.Value == null)
            return NotFound();

        // Return full dashboard data
        return Ok(result.Value);
    }

    /// <summary>
    ///     Wylogowanie ze stołu
    /// </summary>
    [HttpDelete("{tableId}/members")]
    public async Task<IActionResult> LeaveTable(Guid tableId)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _tableService.LeaveTableAsync(tableId, userId.Value);
        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new { message = "Successfully left table" });
    }

    /// <summary>
    ///     Przekazanie roli administratora
    /// </summary>
    [HttpPost("{tableId}/admins")]
    public async Task<IActionResult> GrantAdmin(Guid tableId, [FromBody] GrantAdminRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _tableService.GrantAdminAsync(tableId, userId.Value, request.UserId);
        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new { message = "Admin role granted" });
    }

    /// <summary>
    ///     Odebranie roli administratora
    /// </summary>
    [HttpDelete("{tableId}/admins/{userId}")]
    public async Task<IActionResult> RevokeAdmin(Guid tableId, Guid userId)
    {
        var currentUserId = GetUserId();
        if (!currentUserId.HasValue)
            return Unauthorized();

        var result = await _tableService.RevokeAdminAsync(tableId, currentUserId.Value, userId);
        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new { message = "Admin role revoked" });
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }
}
