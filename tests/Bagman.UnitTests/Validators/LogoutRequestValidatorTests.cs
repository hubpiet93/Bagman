using Bagman.Api.Validators;
using Bagman.Contracts.Models.Auth;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Bagman.UnitTests.Validators;

[TestFixture]
public class LogoutRequestValidatorTests
{
    private LogoutRequestValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new LogoutRequestValidator();
    }

    [TestFixture]
    public class RefreshTokenValidationTests : LogoutRequestValidatorTests
    {
        [Test]
        public void Validate_WhenRefreshTokenIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = ""
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "RefreshToken" && e.ErrorMessage == "Refresh token jest wymagany");
        }

        [Test]
        public void Validate_WhenRefreshTokenIsNull_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = null!
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
        }

        [Test]
        public void Validate_WhenRefreshTokenIsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = new string('a', 1001)
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "RefreshToken" && e.ErrorMessage == "Refresh token jest zbyt długi");
        }

        [Test]
        public void Validate_WhenRefreshTokenHasInvalidJwtFormat_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = "invalid.jwt.token.format"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "RefreshToken" && e.ErrorMessage == "Nieprawidłowy format refresh token");
        }

        [Test]
        public void Validate_WhenRefreshTokenHasOnlyOnePart_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = "header"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "RefreshToken" && e.ErrorMessage == "Nieprawidłowy format refresh token");
        }

        [Test]
        public void Validate_WhenRefreshTokenHasOnlyTwoParts_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = "header.payload"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "RefreshToken" && e.ErrorMessage == "Nieprawidłowy format refresh token");
        }

        [Test]
        public void Validate_WhenRefreshTokenHasMoreThanThreeParts_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = "header.payload.signature.extra"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "RefreshToken" && e.ErrorMessage == "Nieprawidłowy format refresh token");
        }

        [Test]
        public void Validate_WhenRefreshTokenHasValidJwtFormat_ShouldNotHaveValidationError()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
        }

        [Test]
        public void Validate_WhenRefreshTokenIsExactly1000Characters_ShouldNotHaveValidationError()
        {
            // Arrange
            // JWT: 3 części oddzielone kropkami, całość 1000 znaków
            var prefix = "a.b.";
            var rest = new string('c', 1000 - prefix.Length);
            var token = prefix + rest;
            var request = new LogoutRequest
            {
                RefreshToken = token
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
        }

        [Test]
        public void Validate_WhenRefreshTokenIsValid_ShouldNotHaveAnyValidationErrors()
        {
            // Arrange
            var request = new LogoutRequest
            {
                RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
} 