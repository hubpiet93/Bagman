using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TypowanieMeczy.Api.Models;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.ValueObjects;
using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.Entities;

namespace TypowanieMeczy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TablesController : ControllerBase
{
    private readonly ITableService _tableService;
    private readonly ITableRepository _tableRepository;
    private readonly ILogger<TablesController> _logger;

    public TablesController(
        ITableService tableService,
        ITableRepository tableRepository,
        ILogger<TablesController> logger)
    {
        _tableService = tableService;
        _tableRepository = tableRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TableDto>>> GetTables()
    {
        try
        {
            var userId = GetCurrentUserId();
            var tables = await _tableRepository.GetByUserIdAsync(userId);
            var tableDtos = tables.Select(TableDto.FromEntity);
            return Ok(tableDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tables for user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TableDto>> GetTable(string id)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var userId = GetCurrentUserId();

            var table = await _tableRepository.GetByIdAsync(tableId);
            if (table == null)
                return NotFound();

            var isMember = await _tableService.IsUserMemberAsync(tableId, userId);
            if (!isMember)
                return Forbid();

            return Ok(TableDto.FromEntity(table));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting table {TableId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TableDto>> CreateTable(CreateTableRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var tableName = new TableName(request.Name);
            var passwordHash = new PasswordHash(request.Password); // In real app, hash the password
            var maxPlayers = new MaxPlayers(request.MaxPlayers);
            var stake = new Stake(request.Stake);

            var table = new Table(tableName, passwordHash, maxPlayers, stake, userId);
            await _tableRepository.AddAsync(table);

            _logger.LogInformation("Table {TableName} created by user {UserId}", request.Name, userId);
            return CreatedAtAction(nameof(GetTable), new { id = table.Id }, TableDto.FromEntity(table));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating table");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTable(string id, UpdateTableRequest request)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var userId = GetCurrentUserId();

            var canEdit = await _tableService.CanUserEditTableAsync(tableId, userId);
            if (!canEdit)
                return Forbid();

            var table = await _tableRepository.GetByIdAsync(tableId);
            if (table == null)
                return NotFound();

            // Update table properties
            if (!string.IsNullOrEmpty(request.Name))
                table.UpdateName(new TableName(request.Name));

            if (request.MaxPlayers.HasValue)
                table.UpdateMaxPlayers(new MaxPlayers(request.MaxPlayers.Value));

            if (request.Stake.HasValue)
                table.UpdateStake(new Stake(request.Stake.Value));

            await _tableRepository.UpdateAsync(table);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating table {TableId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTable(string id)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var userId = GetCurrentUserId();

            var canDelete = await _tableService.CanUserDeleteTableAsync(tableId, userId);
            if (!canDelete)
                return Forbid();

            await _tableRepository.DeleteAsync(tableId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting table {TableId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinTable(string id, JoinTableRequest request)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var userId = GetCurrentUserId();

            var canJoin = await _tableService.CanUserJoinTableAsync(tableId, userId);
            if (!canJoin)
                return BadRequest("Cannot join table");

            // Validate password if table is secret
            var isSecret = await _tableService.IsTableSecretAsync(tableId);
            if (isSecret)
            {
                var isValidPassword = await _tableService.ValidateTablePasswordAsync(tableId, request.Password);
                if (!isValidPassword)
                    return BadRequest("Invalid password");
            }

            // Add user to table (this would be implemented in the domain)
            // For now, we'll just return success
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining table {TableId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/leave")]
    public async Task<IActionResult> LeaveTable(string id)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var userId = GetCurrentUserId();

            var canLeave = await _tableService.CanUserLeaveTableAsync(tableId, userId);
            if (!canLeave)
                return BadRequest("Cannot leave table");

            // Remove user from table (this would be implemented in the domain)
            // For now, we'll just return success
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving table {TableId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private UserId GetCurrentUserId()
    {
        var userId = HttpContext.Items["UserId"] as UserId;
        if (userId == null)
            throw new InvalidOperationException("User ID not found in context");

        return userId;
    }
} 