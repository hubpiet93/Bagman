namespace TypowanieMeczy.Api.Models;

public class UpdateTableRequest
{
    public string? Name { get; set; }
    public int? MaxPlayers { get; set; }
    public decimal? Stake { get; set; }
    public bool? IsSecretMode { get; set; }
} 