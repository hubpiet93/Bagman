namespace Bagman.Contracts.Models.Auth;

public class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
