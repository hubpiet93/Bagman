namespace TypowanieMeczy.Infrastructure.Services;

public class SupabaseUser
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
} 