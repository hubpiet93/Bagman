using Bagman.Api.Validators;
using Bagman.Contracts.Models.Auth;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Bagman.UnitTests.Validators;

[TestFixture]
public class LoginRequestValidatorTests
{
    private LoginRequestValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new LoginRequestValidator();
    }

    [TestFixture]
    public class LoginValidationTests : LoginRequestValidatorTests
    {
        [Test]
        public void Validate_WhenLoginIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "",
                Password = "password123"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Login);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Login" && e.ErrorMessage == "Login jest wymagany");
        }

        [Test]
        public void Validate_WhenLoginIsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = new string('a', 51),
                Password = "password123"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Login);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Login" && e.ErrorMessage == "Login nie może być dłuższy niż 50 znaków");
        }

        [Test]
        public void Validate_WhenLoginIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "validuser",
                Password = "password123"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Login);
        }

        [Test]
        public void Validate_WhenLoginIsExactly50Characters_ShouldNotHaveValidationError()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = new string('a', 50),
                Password = "password123"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Login);
        }
    }

    [TestFixture]
    public class PasswordValidationTests : LoginRequestValidatorTests
    {
        [Test]
        public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "validuser",
                Password = ""
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Hasło jest wymagane");
        }

        [Test]
        public void Validate_WhenPasswordIsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "validuser",
                Password = new string('a', 129)
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Hasło nie może być dłuższe niż 128 znaków");
        }

        [Test]
        public void Validate_WhenPasswordIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "validuser",
                Password = "password123"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void Validate_WhenPasswordIsExactly128Characters_ShouldNotHaveValidationError()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "validuser",
                Password = new string('a', 128)
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }

    [TestFixture]
    public class CompleteValidationTests : LoginRequestValidatorTests
    {
        [Test]
        public void Validate_WhenAllFieldsAreValid_ShouldNotHaveAnyValidationErrors()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "validuser",
                Password = "password123"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        public void Validate_WhenBothFieldsAreInvalid_ShouldHaveMultipleValidationErrors()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "",
                Password = ""
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.ShouldHaveValidationErrorFor(x => x.Login);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
} 