namespace Bagman.IntegrationTests.Helpers;

/// <summary>
///     Constants used across integration tests to maintain consistency.
/// </summary>
public static class TestConstants
{
    // User passwords
    public const string DefaultUserPassword = "Pass@12345";
    public const string CreatorPassword = "Creator@12345";
    public const string JoinerPassword = "Joiner@12345";
    public const string MemberPassword = "Member@12345";

    // Table passwords
    public const string DefaultTablePassword = "TablePass@123";
    public const string AuthTablePassword = "AuthTablePass@123";
    public const string WrongPassword = "WrongPassword@123";

    // Default values
    public const decimal DefaultStake = 50m;
    public const decimal AuthTableStake = 25m;
    public const int DefaultMaxPlayers = 10;
}
