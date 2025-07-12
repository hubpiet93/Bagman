using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TypowanieMeczy.Api.Models;
using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Domain.ValueObjects;

namespace TypowanieMeczy.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TablesController : ControllerBase
{
    private readonly ITableService _tableService;
    private readonly ILogger<TablesController> _logger;

    public TablesController(ITableService tableService, ILogger<TablesController> logger)
    {
        _tableService = tableService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<TableDto>> CreateTable([FromBody] CreateTableRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var tableName = new TableName(request.Name);
            var passwordHash = new PasswordHash(request.Password); // Should be hashed
            var maxPlayers = new MaxPlayers(request.MaxPlayers);
            var stake = new Stake(request.Stake);

            var table = await _tableService.CreateTableAsync(tableName, passwordHash, maxPlayers, stake, userId);
            
            return CreatedAtAction(nameof(GetTable), new { id = table.Id }, TableDto.FromEntity(table));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating table");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TableDto>>> GetUserTables()
    {
        try
        {
            var userId = GetCurrentUserId();
            var tables = await _tableService.GetUserTablesAsync(userId);
            
            return Ok(tables.Select(TableDto.FromEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user tables");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TableDto>> GetTable(string id)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var userId = GetCurrentUserId();
            
            var table = await _tableService.GetTableDetailsAsync(tableId, userId);
            if (table == null)
                return NotFound();

            return Ok(TableDto.FromEntity(table));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting table {TableId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("join")]
    public async Task<ActionResult<TableDto>> JoinTable([FromBody] JoinTableRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var tableName = new TableName(request.TableName);
            var tablePassword = new PasswordHash(request.TablePassword);

            var table = await _tableService.JoinTableAsync(tableName, tablePassword, userId);
            
            return Ok(TableDto.FromEntity(table));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining table");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}/members/me")]
    public async Task<ActionResult> LeaveTable(string id)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var userId = GetCurrentUserId();

            await _tableService.LeaveTableAsync(tableId, userId);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving table {TableId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}/members")]
    public async Task<ActionResult<IEnumerable<TableMemberDto>>> GetTableMembers(string id)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var userId = GetCurrentUserId();

            var memberships = await _tableService.GetTableMembersAsync(tableId, userId);
            
            return Ok(memberships.Select(TableMemberDto.FromEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting table members {TableId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/members/{userId}/admin")]
    public async Task<ActionResult> GrantAdminRole(string id, string userId)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var targetUserId = UserId.FromString(userId);
            var adminUserId = GetCurrentUserId();

            await _tableService.GrantAdminRoleAsync(tableId, targetUserId, adminUserId);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error granting admin role");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}/members/{userId}/admin")]
    public async Task<ActionResult> RevokeAdminRole(string id, string userId)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var targetUserId = UserId.FromString(userId);
            var adminUserId = GetCurrentUserId();

            await _tableService.RevokeAdminRoleAsync(tableId, targetUserId, adminUserId);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking admin role");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/settings")]
    public async Task<ActionResult<TableDto>> UpdateTableSettings(string id, [FromBody] UpdateTableSettingsRequest request)
    {
        try
        {
            var tableId = TableId.FromString(id);
            var adminUserId = GetCurrentUserId();
            var tableName = new TableName(request.Name);
            var maxPlayers = new MaxPlayers(request.MaxPlayers);
            var stake = new Stake(request.Stake);

            await _tableService.UpdateTableSettingsAsync(tableId, tableName, maxPlayers, stake, request.IsSecretMode, adminUserId);
            
            var updatedTable = await _tableService.GetTableDetailsAsync(tableId, adminUserId);
            return Ok(TableDto.FromEntity(updatedTable));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating table settings");
            return BadRequest(new { error = ex.Message });
        }
    }

    private UserId GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in token");

        return UserId.FromString(userIdClaim);
    }
} 