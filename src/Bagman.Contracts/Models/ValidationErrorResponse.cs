namespace Bagman.Contracts.Models;

public class ValidationError
{
    public string ErrorCode { get; set; } = null!;
    public string ErrorMessage { get; set; } = null!;
    public string? ErrorSource { get; set; }
}

public class ValidationErrorResponse
{
    public List<ValidationError> Errors { get; set; } = new();
}
