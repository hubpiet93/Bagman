using Bagman.Api.Validators;
using Bagman.Contracts.Models.Auth;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Bagman.UnitTests.Validators;

[TestFixture]
public class RegisterRequestValidatorTests
{
    private RegisterRequestValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new RegisterRequestValidator();
    }

    [TestFixture]
    public class LoginValidationTests : RegisterRequestValidatorTests
    {
        [Test]
        public void Validate_WhenLoginIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "",
                Email = "test@example.com",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Login);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Login" && e.ErrorMessage == "Login jest wymagany");
        }

        [Test]
        public void Validate_WhenLoginIsTooShort_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "ab",
                Email = "test@example.com",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Login);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Login" && e.ErrorMessage == "Login musi mieć od 3 do 50 znaków");
        }

        [Test]
        public void Validate_WhenLoginIsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = new string('a', 51),
                Email = "test@example.com",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Login);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Login" && e.ErrorMessage == "Login musi mieć od 3 do 50 znaków");
        }

        [Test]
        public void Validate_WhenLoginContainsInvalidCharacters_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "user@name",
                Email = "test@example.com",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Login);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Login" && e.ErrorMessage == "Login może zawierać tylko litery, cyfry i podkreślnik");
        }

        [Test]
        public void Validate_WhenLoginContainsSpaces_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "user name",
                Email = "test@example.com",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Login);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Login" && e.ErrorMessage == "Login nie może zawierać spacji");
        }

        [Test]
        public void Validate_WhenLoginIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "valid_user123",
                Email = "test@example.com",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Login);
        }
    }

    [TestFixture]
    public class EmailValidationTests : RegisterRequestValidatorTests
    {
        [Test]
        public void Validate_WhenEmailIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Email" && e.ErrorMessage == "Email jest wymagany");
        }

        [Test]
        public void Validate_WhenEmailIsNull_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = null!,
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void Validate_WhenEmailIsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "invalid-email",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Email" && e.ErrorMessage == "Nieprawidłowy format email");
        }

        [Test]
        public void Validate_WhenEmailIsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = new string('a', 250) + "@example.com",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Email" && e.ErrorMessage == "Email nie może być dłuższy niż 255 znaków");
        }

        [Test]
        public void Validate_WhenEmailIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "test@example.com",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }

    [TestFixture]
    public class PasswordValidationTests : RegisterRequestValidatorTests
    {
        [Test]
        public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "test@example.com",
                Password = ""
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Hasło jest wymagane");
        }

        [Test]
        public void Validate_WhenPasswordIsTooShort_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "test@example.com",
                Password = "Short1!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Hasło musi mieć minimum 10 znaków");
        }

        [Test]
        public void Validate_WhenPasswordIsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "test@example.com",
                Password = new string('a', 129)
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Hasło nie może być dłuższe niż 128 znaków");
        }

        [Test]
        public void Validate_WhenPasswordMissingUppercase_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "test@example.com",
                Password = "validpassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Hasło musi zawierać wielką literę");
        }

        [Test]
        public void Validate_WhenPasswordMissingLowercase_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "test@example.com",
                Password = "VALIDPASSWORD123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Hasło musi zawierać małą literę");
        }

        [Test]
        public void Validate_WhenPasswordMissingDigit_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "test@example.com",
                Password = "ValidPassword!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Hasło musi zawierać cyfrę");
        }

        [Test]
        public void Validate_WhenPasswordMissingSpecialCharacter_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "test@example.com",
                Password = "ValidPassword123"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Hasło musi zawierać znak specjalny");
        }

        [Test]
        public void Validate_WhenPasswordContainsSpaces_ShouldHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "test@example.com",
                Password = "Valid Password123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Hasło nie może zawierać spacji");
        }

        [Test]
        public void Validate_WhenPasswordIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "validuser",
                Email = "test@example.com",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }

    [TestFixture]
    public class CompleteValidationTests : RegisterRequestValidatorTests
    {
        [Test]
        public void Validate_WhenAllFieldsAreValid_ShouldNotHaveAnyValidationErrors()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "valid_user123",
                Email = "test@example.com",
                Password = "ValidPassword123!"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Test]
        public void Validate_WhenMultipleFieldsAreInvalid_ShouldHaveMultipleValidationErrors()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Login = "",
                Email = "invalid-email",
                Password = "short"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCountGreaterThan(1);
            result.ShouldHaveValidationErrorFor(x => x.Login);
            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
} 