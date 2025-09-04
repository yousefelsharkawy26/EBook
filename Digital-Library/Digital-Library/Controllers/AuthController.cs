using Digital_Library.Core.ViewModels;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Digital_Library.Controllers;
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> Login()
    {

        await _authService.SignUp("Yousef", "yousef@gmail.com", "123midoA@");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _authService.SignIn(model.Email, model.Password);

                return RedirectToAction("Index", "Home", null);
            }
            catch
            {
                return View();
            }
        }

        return View();
    }
}
