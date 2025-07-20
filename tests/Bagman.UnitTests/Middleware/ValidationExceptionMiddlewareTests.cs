using Bagman.Api.Middleware;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using System.Net;
using System.Text.Json;

namespace Bagman.UnitTests.Middleware;

[TestFixture]
public class ValidationExceptionMiddlewareTests
{
    private ValidationExceptionMiddleware _middleware = null!;
    private RequestDelegate _next = null!;
    private HttpContext _httpContext = null!;

    [SetUp]
    public void Setup()
    {
        _next = Substitute.For<RequestDelegate>();
        _middleware = new ValidationExceptionMiddleware(_next);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        _httpContext = context;
    }

    [Test]
    public async Task InvokeAsync_WhenNoExceptionOccurs_ShouldCallNextMiddleware()
    {
        // Arrange
        _next.Invoke(Arg.Any<HttpContext>()).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        await _next.Received(1).Invoke(_httpContext);
    }

    [Test]
    public async Task InvokeAsync_WhenValidationExceptionOccurs_ShouldReturnBadRequestWithErrors()
    {
        // Arrange
        var validationErrors = new List<ValidationFailure>
        {
            new("Login", "Login jest wymagany"),
            new("Password", "Hasło jest wymagane")
        };

        var validationException = new ValidationException(validationErrors);
        _next.Invoke(Arg.Any<HttpContext>()).Returns(Task.FromException(validationException));

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _httpContext.Response.ContentType.Should().Be("application/json");

        _httpContext.Response.Body.Position = 0;
        var responseContent = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        responseObject.Should().NotBeNull();
        var messageElement = responseObject["message"] as System.Text.Json.JsonElement?;
        messageElement.HasValue.Should().BeTrue();
        messageElement.Value.GetString().Should().Be("Walidacja nie powiodła się");

        var errors = JsonSerializer.Deserialize<string[]>(responseObject["errors"].ToString()!);
        errors.Should().NotBeNull();
        errors!.Should().HaveCount(2);
        errors.Should().Contain("Login jest wymagany");
        errors.Should().Contain("Hasło jest wymagane");
    }

    [Test]
    public async Task InvokeAsync_WhenValidationExceptionOccurs_ShouldNotCallNextMiddlewareAgain()
    {
        // Arrange
        var validationException = new ValidationException("Validation failed");
        _next.Invoke(Arg.Any<HttpContext>()).Returns(Task.FromException(validationException));

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        await _next.Received(1).Invoke(_httpContext);
    }

    [Test]
    public void InvokeAsync_WhenOtherExceptionOccurs_ShouldPropagateException()
    {
        // Arrange
        var otherException = new InvalidOperationException("Something went wrong");
        _next.Invoke(Arg.Any<HttpContext>()).Returns(Task.FromException(otherException));

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _middleware.InvokeAsync(_httpContext).GetAwaiter().GetResult());
        ex.Message.Should().Be("Something went wrong");
        _next.Received(1).Invoke(_httpContext);
    }

    [Test]
    public async Task InvokeAsync_WhenValidationExceptionOccurs_ShouldSetCorrectResponseHeaders()
    {
        // Arrange
        var validationException = new ValidationException("Validation failed");
        _next.Invoke(Arg.Any<HttpContext>()).Returns(Task.FromException(validationException));

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.ContentType.Should().Be("application/json");
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task InvokeAsync_WhenValidationExceptionOccurs_ShouldWriteJsonResponse()
    {
        // Arrange
        var validationErrors = new List<ValidationFailure>
        {
            new("Field", "Error message")
        };

        var validationException = new ValidationException(validationErrors);
        _next.Invoke(Arg.Any<HttpContext>()).Returns(Task.FromException(validationException));

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.Body.Position = 0;
        var responseContent = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        
        responseContent.Should().NotBeEmpty();
        var responseObject = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        responseObject.Should().NotBeNull();
        responseObject.Should().ContainKey("message");
        var messageElement = responseObject["message"] as System.Text.Json.JsonElement?;
        messageElement.HasValue.Should().BeTrue();
        messageElement.Value.GetString().Should().Be("Walidacja nie powiodła się");
        responseObject.Should().ContainKey("errors");
        var errorsElement = responseObject["errors"] as System.Text.Json.JsonElement?;
        errorsElement.HasValue.Should().BeTrue();
        var errorList = errorsElement.Value.EnumerateArray().Select(e => e.GetString()).ToList();
        errorList.Should().Contain("Error message");
    }
} 