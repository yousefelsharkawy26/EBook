using Digital_Library.Core.ViewModels;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;

namespace Digital_Library.Controllers;
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService,
                          ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        TempData.Remove("CanAccessReset");
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        await Task.CompletedTask;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var res = await _authService.SignInAsync(model.Email, model.Password);

                if (res.Success)
                    return RedirectToAction("Index", "Home", null);

                ModelState.AddModelError(string.Empty, "Email or password is wrong");
                _logger.LogWarning(res.Message, $"in {nameof(Login)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"in {nameof(Login)}");

                return View();
            }
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        TempData.Remove("CanAccessReset");
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        await Task.CompletedTask;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var res = await _authService.SignUpAsync(model.FullName, model.Email, model.Password);

                if (res.Success)
                    return await Login(new LoginViewModel() { Email = model.Email, Password = model.Password });

                _logger.LogWarning(res.Message, $"in {nameof(Login)}");
            }
            catch
            {
                return View();
            }
        }
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return RedirectToAction("Index", "Home", null);
    }

    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
            return BadRequest();

        var res = await _authService.VerifyEmailAsync(userId, token);

        return res.Success ? View() : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> ForgetPassword()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
    {
        var res = await _authService.ForgetPasswordAsync(model.Email);

        if (res.Success)
        {
            TempData["CanAccessReset"] = true;
            return RedirectToAction("ResetPassword", res.Data);
        }
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string userId, string token)
    {
        if (TempData["CanAccessReset"] == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // keep TempData alive for refresh
        TempData.Keep("CanAccessReset");
        return View(new ResetPasswordViewModel { UserId = userId, Token = token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var res = await _authService.ResetPasswordAsync(model.UserId, model.Token, model.NewPassword);

        if (!res.Success)
        {
            ModelState.AddModelError("", res.Message);
            return View(model);
        }

        TempData.Remove("CanAccessReset");
        return RedirectToAction("Login");
    }

}
