using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Digital_Library.Service.Implementation;

public class AuthService : IAuthService
{
	private readonly UserManager<User> _userManager;
	private readonly SignInManager<User> _signInManager;
	private readonly IEmailSender _emailSender;
	private readonly IUrlHelperFactory _urlHelperFactory;
	private readonly IActionContextAccessor _actionContextAccessor;
	private readonly ILogger<AuthService> _logger;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public AuthService(UserManager<User> userManager,
					   SignInManager<User> signInManager,
					   IEmailSender emailSender,
					   IUrlHelperFactory urlHelperFactory,
					   IActionContextAccessor actionContextAccessor,
					   ILogger<AuthService> logger,
					   IWebHostEnvironment webHostEnvironment)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_emailSender = emailSender;
		_urlHelperFactory = urlHelperFactory;
		_actionContextAccessor = actionContextAccessor;
		_logger = logger;
		_webHostEnvironment = webHostEnvironment;
	}

	// ===================== SignIn =====================
	public async Task<Response> SignInAsync(string email, string password)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null)
			return Response.Fail("User not found");

		var result = await _signInManager.PasswordSignInAsync(user, password, true, false);
		if (!result.Succeeded)
			return Response.Fail("Invalid credentials");

		_logger.LogInformation($"User {user.Email} signed in.");

		try
		{
			string htmlMessage = $"<p>Hello {user.FullName},</p>" +
																								"<p>Welcome back to E-BOOK platform! We're glad to see you again.</p>" +
																								"<p>Enjoy browsing our library.</p>";

			await _emailSender.SendEmailAsync(user.Email, "Welcome Back!", htmlMessage);
			_logger.LogInformation($"Welcome email sent to {user.Email}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Failed to send welcome email to {user.Email}");
		}

		return Response.Ok("Sign-in successful");
	}


	// ===================== SignOut =====================
	public async Task<Response> SignOutAsync()
	{
		await _signInManager.SignOutAsync();
		_logger.LogInformation("User signed out.");
		return Response.Ok("Sign-out successful");
	}

	// ===================== SignUp =====================
	public async Task<Response> SignUpAsync(string name, string email, string password)
	{
		var existingUser = await _userManager.FindByEmailAsync(email);
		if (existingUser != null) return Response.Fail("Email already exists");

		var user = new User()
		{
			Email = email,
			UserName = await CreateUniqueUsernameFromNameAsync(name),
			FullName = name
		};

		var res = await _userManager.CreateAsync(user, password);
		if (!res.Succeeded) return Response.Fail(res.Errors.FirstOrDefault()?.Description ?? "Sign-up failed");

		await SendEmailVerificationAsync(user);

		_logger.LogInformation($"User {user.Email} signed up.");
		return Response.Ok("Sign-up successful", user);
	}

	// ===================== ForgetPassword =====================
	public async Task<Response> ForgetPasswordAsync(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null) return Response.Fail("User not found");

		try
		{
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			await SendPasswordResetEmail(user, token);
			_logger.LogInformation($"Password reset email sent to {email}");
			return Response.Ok("Password reset email sent");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error sending password reset email");
			return Response.Fail("Failed to send password reset email");
		}
	}

	// ===================== ResetPassword =====================
	public async Task<Response> ResetPasswordAsync(string userId, string token, string newPassword)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null) return Response.Fail("User not found");

		var res = await _userManager.ResetPasswordAsync(user, token, newPassword);
		if (!res.Succeeded) return Response.Fail(res.Errors.FirstOrDefault()?.Description ?? "Reset password failed");

		_logger.LogInformation($"Password reset for user {user.Email}");
		return Response.Ok("Password reset successful");
	}

	// ===================== ChangePassword =====================
	public async Task<Response> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null) return Response.Fail("User not found");

		var check = await _userManager.CheckPasswordAsync(user, oldPassword);
		if (!check) return Response.Fail("Old password is incorrect");

		var res = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
		if (!res.Succeeded) return Response.Fail(res.Errors.FirstOrDefault()?.Description ?? "Change password failed");

		_logger.LogInformation($"Password changed for user {user.Email}");
		return Response.Ok("Password changed successfully");
	}

	// ===================== VerifyEmail =====================
	public async Task<Response> VerifyEmailAsync(string userId, string token)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null) return Response.Fail("User not found");

		var isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
		if (isConfirmed) return Response.Fail("Your email is already verified. You can log in directly.");

        var res = await _userManager.ConfirmEmailAsync(user, token);
		if (!res.Succeeded) return Response.Fail(res.Errors.FirstOrDefault()?.Description ?? "Email verification failed");

		_logger.LogInformation($"Email verified for user {user.Email}");
		return Response.Ok("Email verified successfully");
	}

	// ===================== ChangeEmail =====================
	public async Task<Response> ChangeEmailAsync(string userId, string newEmail)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null) return Response.Fail("User not found");

		var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
		var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

		if (!result.Succeeded) return Response.Fail(result.Errors.FirstOrDefault()?.Description ?? "Change email failed");

		_logger.LogInformation($"Email changed for user {user.UserName} to {newEmail}");
		return Response.Ok("Email changed successfully");
	}

	// ===================== Helpers =====================
	#region Helper Methods

	private async Task<string> CreateUniqueUsernameFromNameAsync(string fullName)
	{
		string baseUsername = GenerateUsernamePrefix(fullName);
		string currentUsername = baseUsername;
		int counter = 1;

		while (await _userManager.FindByNameAsync(currentUsername) != null)
		{
			currentUsername = $"{baseUsername}{counter}";
			counter++;
		}

		return currentUsername;
	}

	private static string GenerateUsernamePrefix(string fullName, int maxLength = 20)
	{
		if (string.IsNullOrWhiteSpace(fullName)) return "user";

		string[] nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		string firstName = SanitizePart(nameParts.FirstOrDefault());
		string lastName = nameParts.Length > 1 ? SanitizePart(nameParts.LastOrDefault()) : "";

		string usernamePrefix = string.IsNullOrEmpty(lastName) ? firstName : $"{firstName}{lastName}";
		if (usernamePrefix.Length > maxLength) usernamePrefix = usernamePrefix.Substring(0, maxLength);

		return string.IsNullOrWhiteSpace(usernamePrefix) ? "user" : usernamePrefix;
	}

	private static string SanitizePart(string namePart)
	{
		if (string.IsNullOrEmpty(namePart)) return "";
		string normalized = new string(namePart.ToLowerInvariant()
						.Normalize(NormalizationForm.FormD)
						.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
						.ToArray());
		return Regex.Replace(normalized, @"[^a-z0-9]", "");
	}

	private async Task SendEmailVerificationAsync(User user)
	{
		try
		{
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var actionContext = _actionContextAccessor.ActionContext;
			var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);

			var verificationLink = urlHelper.Action("ConfirmEmail", "Auth",
							new { userId = user.Id, token = token },
							protocol: actionContext.HttpContext.Request.Scheme);

			string wwwRoot = _webHostEnvironment.WebRootPath;
			string template = Path.Combine(wwwRoot, "html/EmailVerification.html");
			string html = await File.ReadAllTextAsync(template);

			html = html.Replace("[User's Name]", user.UserName)
														.Replace("[Verification Link]", verificationLink)
														.Replace("[App Name]", "E-BOOK")
														.Replace("[Company Name]", "ITI")
														.Replace("[Company Address]", "Mansoura University");

			await _emailSender.SendEmailAsync(user.Email, "Please Verify Your Email Address", html);
			_logger.LogInformation($"Email verification sent to {user.Email}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to send email verification");
		}
	}

	private async Task SendPasswordResetEmail(User user, string token)
	{
		var actionContext = _actionContextAccessor.ActionContext;
		var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
		var resetLink = urlHelper.Action("ResetPassword", "Account",
						new { userId = user.Id, token = token },
						protocol: actionContext.HttpContext.Request.Scheme);

		string wwwRoot = _webHostEnvironment.WebRootPath;
		string template = Path.Combine(wwwRoot, "templates/email/PasswordReset.html");
		string html = await File.ReadAllTextAsync(template);

		html = html.Replace("[User's Name]", user.FullName)
													.Replace("[Reset Link]", resetLink)
													.Replace("[App Name]", "E-BOOK")
													.Replace("[Company Name]", "ITI")
													.Replace("[Company Address]", "Mansoura University");

		await _emailSender.SendEmailAsync(user.Email, "Reset Your Password", html);
	}

	#endregion
}
