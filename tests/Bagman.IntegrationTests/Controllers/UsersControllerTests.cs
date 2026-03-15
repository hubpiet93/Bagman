using Bagman.IntegrationTests.Controllers.Endpoints;
using Bagman.IntegrationTests.Controllers.Endpoints.Users;
using Bagman.IntegrationTests.Helpers;
using Bagman.IntegrationTests.TestFixtures;
using Xunit.Abstractions;

namespace Bagman.IntegrationTests.Controllers;

[CollectionDefinition("Users Tests")]
public class UsersTestsCollection : ICollectionFixture<PostgresFixture>
{
}

/// <summary>
///     Integration tests for UsersController actions.
///     Tests GET /api/users/me endpoint.
/// </summary>
[Collection("Users Tests")]
public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    public UsersControllerTests(PostgresFixture postgresFixture, ITestOutputHelper testOutputHelper) : base(postgresFixture, testOutputHelper)
    {
    }

    public async Task InitializeAsync()
    {
        await Init();
    }

    public new async Task DisposeAsync()
    {
        await Dispose();
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ReturnsOkWithUserProfile()
    {
        // Arrange
        var (token, _, _) = await HttpClient.RegisterAndGetTokenAsync("me_user", TestConstants.DefaultUserPassword);

        // Act
        await HttpClient.GetCurrentUserAsync<HttpResponseMessage>(token);

        // Assert
        await VerifyHttpRecording();
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        await HttpClient.GetCurrentUserAsync<HttpResponseMessage>();

        // Assert
        await VerifyHttpRecording();
    }
}
