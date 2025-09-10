
// في Web/Controllers/CustomersController.cs
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Digital_Library.AdminPanel.Controllers;
public class CustomersController : Controller
{
    private readonly IUserService _userService;

    public CustomersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: /Customers
    public async Task<IActionResult> Index()
    {
        var customers = await _userService.GetAllCustomersAsync();
        return View(customers);
    }

    // GET: /Customers/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var customerDetails = await _userService.GetCustomerDetailsAsync(id);
        if (customerDetails == null)
        {
            return NotFound();
        }

        return View(customerDetails);
    }

    // POST: /Customers/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        await _userService.ToggleUserStatusAsync(id);

        // أعد المستخدم إلى صفحة القائمة
        return RedirectToAction(nameof(Index));
    }
}
