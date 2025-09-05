using Digital_Library.Core.Enum;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Digital_Library.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Orders (بتجيب طلبات المستخدم الحالي)
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // لو عندك Identity
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);
            if (!response.Success) return NotFound();

            return View(response.Data);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View(new List<OrderDetailRequest>());
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<OrderDetailRequest> items)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await _orderService.CreateOrderAsync(userId, items);

            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", response.Message);
            return View(items);
        }

        // GET: Orders/Edit/5 (لتغيير الحالة مثلاً)
        public async Task<IActionResult> Edit(string id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);
            if (!response.Success) return NotFound();

            return View(response.Data);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Status status)
        {
            var response = await _orderService.UpdateOrderStatusAsync(id, status);

            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", response.Message);
            return View();
        }
    }
}
