using Digital_Library.Core.Models;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authentication;
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

    public async Task<bool> ChangePassword(string userId, string oldPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null) 
            throw new ArgumentNullException($"User by Id = {userId} does not exists");

        if (await _userManager.CheckPasswordAsync(user, oldPassword) is false)
            throw new ArgumentException("The old password is wrong");

        var res = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (res == IdentityResult.Success)
            return true;

        return false;
    }
    public async Task ForgetPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            throw new ArgumentNullException($"User by email = {email} does not exists");

        try
        {
            // 2. Generate a password reset token and the callback URL
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var actionContext = _actionContextAccessor.ActionContext;
            var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);

            var resetLink = urlHelper.Action("ResetPassword", "Account",
                new { userId = user.Id, token = token },
                protocol: actionContext.HttpContext.Request.Scheme);

            // 3. Read the HTML email template from the file
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string templatePath = Path.Combine(wwwRootPath, "templates/email/PasswordReset.html");
            string htmlTemplate = await File.ReadAllTextAsync(templatePath);

            // 4. Replace the placeholders with actual data
            htmlTemplate = htmlTemplate.Replace("[User's Name]", user.FullName);
            htmlTemplate = htmlTemplate.Replace("[Reset Link]", resetLink);
            htmlTemplate = htmlTemplate.Replace("[App Name]", "E-BOOK");
            htmlTemplate = htmlTemplate.Replace("[Company Name]", "ITI");
            htmlTemplate = htmlTemplate.Replace("[Company Address]", "Mansoura University");

            // 5. Send the email using your service
            await _emailSender.SendEmailAsync(
                email: user.Email,
                subject: "Reset Your Password",
                htmlMessage: htmlTemplate
            );

            _logger.LogInformation($"Message Send Success From {nameof(ForgetPassword)}");
        }
        catch (Exception ex)
        {
            // Log the exception (using an ILogger is recommended here)
            _logger.LogError(ex, $"Error occurred during forgot password process for email {email}", $"From {nameof(ForgetPassword)}");

        }
    }
    public async Task<bool> ResetPassword(string userId, string token, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new ArgumentNullException($"User by Id = {userId} does not exists");

        var res = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return res == IdentityResult.Success? true: false;
    }
    public async Task SignIn(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync( email );

        if (user == null)
            throw new ArgumentNullException($"User by email = {email} does not exists");

        var props = new AuthenticationProperties()
        {
            AllowRefresh = true,
            IssuedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(10),
            IsPersistent = true,
            RedirectUri = "/login"
        };

        await _signInManager.SignInAsync(user, props);
    }
    public async Task SignOut()
    {
        await _signInManager.SignOutAsync();
    }
    public async Task<bool> SignUp(string name, string email, string password)
    {
        var user = new User()
        {
            Email = email,
            UserName = await CreateUniqueUsernameFromNameAsync(name),
            FullName = name,
        };

        var res = await _userManager.CreateAsync(user, password);

        return res == IdentityResult.Success;
    }
    public async Task VerifyEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new ArgumentNullException($"User by Id = {userId} does not exists");

        await _userManager.ConfirmEmailAsync(user, token);
    }

    #region Helper Methods

    private async Task<string> CreateUniqueUsernameFromNameAsync(string fullName)
    {
        // 1. Generate the base username prefix from the full name
        string baseUsername = GenerateUsernamePrefix(fullName);
        string currentUsername = baseUsername;
        int counter = 1;

        // 2. Check for uniqueness and append a number if it's already taken
        // In a real app, you would check against your database of users.
        while (await IsUsernameTakenAsync(currentUsername))
        {
            // If "johndoe" is taken, try "johndoe1", "johndoe2", etc.
            currentUsername = $"{baseUsername}{counter}";
            counter++;
        }

        return currentUsername;
    }
    private static string GenerateUsernamePrefix(string fullName, int maxLength = 20)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return "user"; // Fallback for empty names
        }

        // 1. Split the name into parts
        string[] nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        string firstName = SanitizePart(nameParts.FirstOrDefault());
        string lastName = nameParts.Length > 1 ? SanitizePart(nameParts.LastOrDefault()) : "";

        // 2. Combine the parts
        string usernamePrefix = string.IsNullOrEmpty(lastName) ? firstName : $"{firstName}{lastName}";

        // 3. Handle cases where sanitization results in an empty string
        if (string.IsNullOrWhiteSpace(usernamePrefix))
        {
            return "user";
        }

        // 4. Truncate to the max length
        if (usernamePrefix.Length > maxLength)
        {
            usernamePrefix = usernamePrefix.Substring(0, maxLength);
        }

        return usernamePrefix;
    }
    private static string SanitizePart(string namePart)
    {
        if (string.IsNullOrEmpty(namePart)) return "";

        // ToLower and remove accents (diacritics)
        string normalized = new string(namePart.ToLowerInvariant()
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        // Remove all non-alphanumeric characters
        string sanitized = Regex.Replace(normalized, @"[^a-z0-9]", "");

        return sanitized;
    }
    private async Task<bool> IsUsernameTakenAsync(string username)
    {
        // Use your UserManager to check if the username exists
        var user = await _userManager.FindByNameAsync(username);
        return user != null;
    }
    private async Task CreateEmailVerification(User user)
    {
        try
        {
            // 1. Generate the email confirmation token and the callback URL
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var actionContext = _actionContextAccessor.ActionContext;
            var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);

            var verificationLink = urlHelper.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token },
                protocol: actionContext.HttpContext.Request.Scheme);

            // 2. Read the HTML email template
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string templatePath = Path.Combine(wwwRootPath, "html/EmailVerification.html");
            string htmlTemplate = await File.ReadAllTextAsync(templatePath);

            // 3. Replace placeholders
            htmlTemplate = htmlTemplate.Replace("[User's Name]", user.UserName);
            htmlTemplate = htmlTemplate.Replace("[Verification Link]", verificationLink);
            htmlTemplate = htmlTemplate.Replace("[App Name]", "E-BOOK");
            htmlTemplate = htmlTemplate.Replace("[Company Name]", "ITI");
            htmlTemplate = htmlTemplate.Replace("[Company Address]", "Mansoura University");
            // ... replace any other placeholders

            // 4. Send the email
            await _emailSender.SendEmailAsync(
                email: user.Email,
                subject: "Please Verify Your Email Address",
                htmlMessage: htmlTemplate
            );

            _logger.LogInformation("Message Send Success...");
        }
        catch (Exception ex)
        {
            // Log the error
            _logger.LogError($"Send Message Failed From {nameof(CreateEmailVerification)}");
        }
    }
    
    #endregion

}