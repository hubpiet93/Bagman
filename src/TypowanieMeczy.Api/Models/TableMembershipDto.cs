namespace TypowanieMeczy.Api.Models;

public class TableMembershipDto
{
    public string UserId { get; set; } = string.Empty;
    public string TableId { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime JoinedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
} 