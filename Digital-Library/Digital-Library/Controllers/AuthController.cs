using Digital_Library.Core.ViewModels;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Interface;
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

    public async Task<IActionResult> Login()
    {
        await Task.CompletedTask;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var res = await _authService.SignInAsync(model.Email, model.Password);

                if (res.Success)
                    return RedirectToAction("Index", "Home", null);
                 
                _logger.LogWarning(res.Message, $"in {nameof(Login)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"in {nameof(Login)}");

                return View();
            }
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        await Task.CompletedTask;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var res = await _authService.SignUpAsync(model.UserName, model.Email, model.Password);

                if (res.Success)
                    return await Login(new LoginViewModel() { Email =  model.Email, Password = model.Password });

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

    public async Task<IActionResult> ForgetPassword()
    {
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgetPassword(string email)
    {
        var res = await _authService.ForgetPasswordAsync(email);

        return View();
    }

    public async Task<IActionResult> ChangePassword()
    {
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest model)
    {
        var res = await _authService.ChangePasswordAsync(model.UserId, model.OldPassword, model.NewPassword);

        if (res.Success) return RedirectToAction("Login");

        return View();
    }
}
