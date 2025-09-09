// AuthServiceTests.cs
using Digital_Library.Core.Constant;
using Digital_Library.Service.Implementation;
using Digital_Library.Service.Interface;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using IEmailSender = Microsoft.AspNetCore.Identity.UI.Services.IEmailSender;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using User = Digital_Library.Core.Models.User;


namespace Digital_Library.Test;


public class AuthServiceTests
{
    // Mocks for all dependencies
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<SignInManager<User>> _mockSignInManager;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly Mock<IUrlHelperFactory> _mockUrlHelperFactory;
    private readonly Mock<IActionContextAccessor> _mockActionContextAccessor;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<AuthService>> _mockLogger;
    private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;

    // The service instance we are testing
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        // Mocking UserManager requires mocking its dependencies (IUserStore)
        var userStoreMock = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

        // Mocking SignInManager requires mocking its dependencies
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
        _mockSignInManager = new Mock<SignInManager<User>>(_mockUserManager.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, null, null, null, null);

        _mockEmailSender = new Mock<IEmailSender>();
        _mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
        _mockActionContextAccessor = new Mock<IActionContextAccessor>();
        _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<AuthService>>();
        _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

        // Setup common mocks
        SetupUrlHelper();

        // Instantiate the service with the mocked dependencies
        _authService = new AuthService(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockEmailSender.Object,
            _mockUrlHelperFactory.Object,
            _mockActionContextAccessor.Object,
            _mockLogger.Object,
            _mockWebHostEnvironment.Object
        );
    }

    // Helper to mock the URL generation logic
    private void SetupUrlHelper()
    {
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor()
        };

        _mockActionContextAccessor.Setup(x => x.ActionContext).Returns(actionContext);

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                     .Returns("http://mock-url.com/some/path");

        _mockUrlHelperFactory.Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>()))
                             .Returns(mockUrlHelper.Object);
    }

    // --- الاختبارات هنا ---
    [Fact]
    public async Task SignInAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var user = new User { Email = "test@example.com", FullName = "Test User" };
        _mockUserManager.Setup(um => um.FindByEmailAsync("test@example.com")).ReturnsAsync(user);
        _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(user, "ValidPassword123", true, false)).ReturnsAsync(SignInResult.Success);

        // Act
        var result = await _authService.SignInAsync("test@example.com", "ValidPassword123");

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Sign-in successful");
        _mockEmailSender.Verify(es => es.SendEmailAsync(user.Email, "Welcome Back!", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SignInAsync_WithNonExistentUser_ShouldReturnFail()
    {
        // Arrange
        _mockUserManager.Setup(um => um.FindByEmailAsync("nonexistent@example.com")).ReturnsAsync((User)null);

        // Act
        var result = await _authService.SignInAsync("nonexistent@example.com", "anypassword");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    [Fact]
    public async Task SignInAsync_WithInvalidPassword_ShouldReturnFail()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };
        _mockUserManager.Setup(um => um.FindByEmailAsync("test@example.com")).ReturnsAsync(user);
        _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(user, "InvalidPassword", true, false)).ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _authService.SignInAsync("test@example.com", "InvalidPassword");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task SignUpAsync_WithNewUser_ShouldReturnSuccess()
    {
        // Arrange
        var newUser = new User { Email = "new@example.com", FullName = "New User" };
        _mockUserManager.Setup(um => um.FindByEmailAsync("new@example.com")).ReturnsAsync((User)null);
        _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null); // For unique username
        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), "Password123!")).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), Roles.Customer)).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>())).ReturnsAsync("dummy-token");

        // For email template reading
        _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
        var tempTemplatePath = Path.Combine(Path.GetTempPath(), "html/EmailVerification.html");
        Directory.CreateDirectory(Path.GetDirectoryName(tempTemplatePath));
        await File.WriteAllTextAsync(tempTemplatePath, "Hello [User's Name], please click [Verification Link]");

        // Act
        var result = await _authService.SignUpAsync("New User", "new@example.com", "Password123!");

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Sign-up successful");
        _mockEmailSender.Verify(es => es.SendEmailAsync("new@example.com", "Please Verify Your Email Address", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SignUpAsync_WithExistingEmail_ShouldReturnFail()
    {
        // Arrange
        var existingUser = new User { Email = "existing@example.com" };
        _mockUserManager.Setup(um => um.FindByEmailAsync("existing@example.com")).ReturnsAsync(existingUser);

        // Act
        var result = await _authService.SignUpAsync("Any Name", "existing@example.com", "Password123!");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email already exists");
    }

    [Fact]
    public async Task SignUpAsync_WhenCreateUserFails_ShouldReturnFail()
    {
        // Arrange
        var error = new IdentityError { Description = "Password is too weak." };
        _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
        _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(error));

        // Act
        var result = await _authService.SignUpAsync("Test User", "test@example.com", "weak");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Password is too weak.");
    }

    [Fact]
    public async Task ForgetPasswordAsync_WithExistingUser_ShouldReturnSuccess()
    {
        // Arrange
        var user = new User { Id = "123", Email = "test@example.com", FullName = "Test User" };
        _mockUserManager.Setup(um => um.FindByEmailAsync("test@example.com")).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("reset-token");

        // For email template reading
        _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
        var tempTemplatePath = Path.Combine(Path.GetTempPath(), "html/PasswordReset.html");
        Directory.CreateDirectory(Path.GetDirectoryName(tempTemplatePath));
        await File.WriteAllTextAsync(tempTemplatePath, "Hello [User's Name], click [Reset Link]");

        // Act
        var result = await _authService.ForgetPasswordAsync("test@example.com");

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Password reset email sent");
        result.Data.Should().NotBeNull();
        _mockEmailSender.Verify(es => es.SendEmailAsync(user.Email, "Reset Your Password", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ForgetPasswordAsync_WithNonExistentUser_ShouldReturnFailButFriendlyMessage()
    {
        // Arrange
        _mockUserManager.Setup(um => um.FindByEmailAsync("nonexistent@example.com")).ReturnsAsync((User)null);

        // Act
        var result = await _authService.ForgetPasswordAsync("nonexistent@example.com");

        // Assert
        // This is a security feature to prevent user enumeration. The service returns a "Fail" but with a success-like message.
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Password reset email sent");
        _mockEmailSender.Verify(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var user = new User { Id = "user1", Email = "test@example.com" };
        _mockUserManager.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.ResetPasswordAsync(user, "valid-token", "NewPassword123!")).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.ResetPasswordAsync("user1", "valid-token", "NewPassword123!");

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Password reset successful");
    }

    [Fact]
    public async Task ResetPasswordAsync_WithInvalidToken_ShouldReturnFail()
    {
        // Arrange
        var user = new User { Id = "user1", Email = "test@example.com" };
        var error = new IdentityError { Description = "Invalid token." };
        _mockUserManager.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.ResetPasswordAsync(user, "invalid-token", "NewPassword123!")).ReturnsAsync(IdentityResult.Failed(error));

        // Act
        var result = await _authService.ResetPasswordAsync("user1", "invalid-token", "NewPassword123!");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid token.");
    }

    [Fact]
    public async Task VerifyEmailAsync_WithValidTokenAndUnconfirmedEmail_ShouldReturnSuccess()
    {
        // Arrange
        var user = new User { Id = "user1", Email = "test@example.com" };
        _mockUserManager.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.IsEmailConfirmedAsync(user)).ReturnsAsync(false);
        _mockUserManager.Setup(um => um.ConfirmEmailAsync(user, "valid-token")).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.VerifyEmailAsync("user1", "valid-token");

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Email verified successfully");
    }

    [Fact]
    public async Task VerifyEmailAsync_WhenEmailIsAlreadyVerified_ShouldReturnFail()
    {
        // Arrange
        var user = new User { Id = "user1", Email = "test@example.com" };
        _mockUserManager.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(user);
        _mockUserManager.Setup(um => um.IsEmailConfirmedAsync(user)).ReturnsAsync(true); // Email is already confirmed

        // Act
        var result = await _authService.VerifyEmailAsync("user1", "any-token");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Your email is already verified. You can log in directly.");
    }


}