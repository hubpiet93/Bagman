using System.Security.Claims;
using Bagman.Api.Controllers.Mappers;
using Bagman.Application.Common;
using Bagman.Application.Features.Tables.CreateTable;
using Bagman.Application.Features.Tables.GetTableByName;
using Bagman.Application.Features.Tables.GetTableDashboard;
using Bagman.Application.Features.Tables.GetTableDetails;
using Bagman.Application.Features.Tables.GetUserTables;
using Bagman.Application.Features.Tables.GrantAdmin;
using Bagman.Application.Features.Tables.JoinTable;
using Bagman.Application.Features.Tables.LeaveTable;
using Bagman.Application.Features.Tables.RevokeAdmin;
using Bagman.Contracts.Models;
using Bagman.Contracts.Models.Tables;
using Bagman.Domain.Services;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Bagman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TablesController : AppControllerBase
{
    private readonly IAuthService _authService;
    private readonly FeatureDispatcher _dispatcher;

    public TablesController(FeatureDispatcher dispatcher, IAuthService authService)
    {
        _dispatcher = dispatcher;
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
        var tableResult = await _dispatcher.HandleAsync<CreateTableCommand, CreateTableResult>(
            new CreateTableCommand
            {
                Name = request.TableName,
                Password = request.TablePassword,
                MaxPlayers = request.MaxPlayers,
                Stake = request.Stake,
                CreatedBy = userId,
                EventTypeId = request.EventTypeId
            });

        if (tableResult.IsError)
            return MapErrors(tableResult.Errors);

        var createdResponse = tableResult.Value.ToTableResponse();

        return CreatedAtAction(nameof(GetTableDetails), new {tableId = tableResult.Value.Id}, createdResponse);
    }

    /// <summary>
    ///     Utworzenie nowego stołu z użyciem istniejącego użytkownika
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateTableAuthorized([FromBody] AuthorizedCreateTableRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        // Check if table with the same name already exists
        var existingTable = await _dispatcher.HandleAsync<GetTableByNameQuery, TableBasicResult?>(
            new GetTableByNameQuery {TableName = request.TableName});

        if (!existingTable.IsError && existingTable.Value != null)
            return Conflict(new {code = "Table.DuplicateName", message = "Stół o podanej nazwie już istnieje"});

        // Create the table
        var result = await _dispatcher.HandleAsync<CreateTableCommand, CreateTableResult>(
            new CreateTableCommand
            {
                Name = request.TableName,
                Password = request.TablePassword,
                MaxPlayers = request.MaxPlayers,
                Stake = request.Stake,
                CreatedBy = userId.Value,
                EventTypeId = request.EventTypeId
            });

        if (result.IsError)
            return MapErrors(result.Errors);

        var createdResponse = result.Value.ToTableResponse();

        return CreatedAtAction(nameof(GetTableDetails), new {tableId = result.Value.Id}, createdResponse);
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
        var getTableResult = await _dispatcher.HandleAsync<GetTableByNameQuery, TableBasicResult?>(
            new GetTableByNameQuery {TableName = request.TableName});

        if (getTableResult.IsError)
            return MapErrors(getTableResult.Errors);

        if (getTableResult.Value == null)
            return NotFound(new {message = "Stół nie został znaleziony"});

        var tableId = getTableResult.Value.Id;

        // Join table
        var joinResult = await _dispatcher.HandleAsync<JoinTableCommand, Success>(
            new JoinTableCommand
            {
                TableId = tableId,
                UserId = userId,
                Password = request.TablePassword
            });

        if (joinResult.IsError)
            return MapErrors(joinResult.Errors);

        // Get updated table details
        var tableDetailsResult = await _dispatcher.HandleAsync<GetTableByNameQuery, TableBasicResult?>(
            new GetTableByNameQuery {TableName = request.TableName});

        if (tableDetailsResult.IsError)
            return MapErrors(tableDetailsResult.Errors);

        var table = tableDetailsResult.Value!;
        return Ok(table.ToTableResponse());
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

        var result = await _dispatcher.HandleAsync<GetUserTablesQuery, List<UserTableResult>>(
            new GetUserTablesQuery {UserId = userId.Value});

        if (result.IsError)
            return MapErrors(result.Errors);

        var response = result.Value
            .OrderBy(t => t.CreatedAt)
            .Select(t => t.ToTableResponse())
            .ToList();

        return Ok(response);
    }

    /// <summary>
    ///     Pobranie szczegółów stołu
    /// </summary>
    [HttpGet("{tableId}")]
    public async Task<IActionResult> GetTableDetails(Guid tableId)
    {
        var result = await _dispatcher.HandleAsync<GetTableDetailsQuery, TableDetailResult>(
            new GetTableDetailsQuery {TableId = tableId});

        if (result.IsError)
            return MapErrors(result.Errors);

        var response = result.Value.ToTableDetailResponse();

        return Ok(response);
    }

    /// <summary>
    ///     Pobranie dashboard stołu (wszystkie informacje)
    /// </summary>
    [HttpGet("{tableId}/dashboard")]
    public async Task<IActionResult> GetTableDashboard(Guid tableId)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _dispatcher.HandleAsync<GetTableDashboardQuery, TableDashboardResult>(
            new GetTableDashboardQuery
            {
                TableId = tableId,
                UserId = userId.Value
            });

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(result.Value.ToTableDashboardResponse());
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

        var result = await _dispatcher.HandleAsync<LeaveTableCommand, Success>(
            new LeaveTableCommand
            {
                TableId = tableId,
                UserId = userId.Value
            });

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new SuccessResponse("Successfully left table"));
    }

    /// <summary>
    ///     Dołączenie zalogowanego użytkownika do istniejącego stołu
    /// </summary>
    [HttpPost("{tableId}/join")]
    public async Task<IActionResult> JoinTableAuthorized(Guid tableId, [FromBody] JoinTableAuthorizedRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        // Join table
        var joinResult = await _dispatcher.HandleAsync<JoinTableCommand, Success>(
            new JoinTableCommand
            {
                TableId = tableId,
                UserId = userId.Value,
                Password = request.Password
            });

        if (joinResult.IsError)
            return MapErrors(joinResult.Errors);

        // Get table details to return with member info
        var tableDetailsResult = await _dispatcher.HandleAsync<GetTableDetailsQuery, TableDetailResult>(
            new GetTableDetailsQuery {TableId = tableId});

        if (tableDetailsResult.IsError)
            return MapErrors(tableDetailsResult.Errors);

        var tableDetail = tableDetailsResult.Value;
        var currentMember = tableDetail.Members.FirstOrDefault(m => m.UserId == userId.Value);

        var response = new JoinTableResponse
        {
            TableId = tableDetail.Id,
            TableName = tableDetail.Name,
            MaxPlayers = tableDetail.MaxPlayers,
            Stake = tableDetail.Stake,
            TableCreatedAt = tableDetail.CreatedAt,
            UserId = currentMember!.UserId,
            UserLogin = currentMember.Login,
            IsAdmin = currentMember.IsAdmin,
            JoinedAt = currentMember.JoinedAt,
            CurrentMemberCount = tableDetail.Members.Count
        };

        return Ok(response);
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

        var result = await _dispatcher.HandleAsync<GrantAdminCommand, Success>(
            new GrantAdminCommand
            {
                TableId = tableId,
                RequestingUserId = userId.Value,
                TargetUserId = request.UserId
            });

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new SuccessResponse("Admin role granted"));
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

        var result = await _dispatcher.HandleAsync<RevokeAdminCommand, Success>(
            new RevokeAdminCommand
            {
                TableId = tableId,
                RequestingUserId = currentUserId.Value,
                TargetUserId = userId
            });

        if (result.IsError)
            return MapErrors(result.Errors);

        return Ok(new SuccessResponse("Admin role revoked"));
    }

    private Guid? GetUserId()
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (Guid.TryParse(subClaim, out var userIdFromSub))
            return userIdFromSub;

        var nameIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(nameIdClaim, out var userIdFromNameId))
            return userIdFromNameId;

        return null;
    }
}
