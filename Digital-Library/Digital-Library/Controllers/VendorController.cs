using Digital_Library.Core.Models;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Digital_Library.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendorService _vendorService;
        private readonly UserManager<User> _userManager;

        public VendorController(IVendorService vendorService, UserManager<User> userManager)
        {
            _vendorService = vendorService;
            _userManager = userManager;
        }

        // GET Vendor
        public async Task<IActionResult> Index()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            return View(vendors);
        }

        // GET VendorDetails
        public async Task<IActionResult> Details(string id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null) return NotFound();
            return View(vendor);
        }

        // GET VendorCreate
        public IActionResult Create()
        {
            return View();
        }

        // POST VendorCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                await _vendorService.AddVendorAsync(vendor);
                return RedirectToAction(nameof(Index));
            }
            return View(vendor);
        }

        // GET VendorEdit
        public async Task<IActionResult> Edit(string id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null) return NotFound();
            return View(vendor);
        }

        // POST VendorEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Vendor vendor)
        {
            if (id != vendor.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _vendorService.UpdateVendorAsync(id, vendor);
                return RedirectToAction(nameof(Index));
            }
            return View(vendor);
        }

        // GET VendorDelete
        public async Task<IActionResult> Delete(string id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null) return NotFound();
            return View(vendor);
        }

        // POST VendorDelete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var success = await _vendorService.DeleteVendorAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET VendorDashboard for current user instead of enter vendor id =>GUID string
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var dashboard = await _vendorService.GetVendorDashboardAsync(currentUser.Id);
            if (dashboard == null) return NotFound("Vendor not found for this user.");

            return View(dashboard);
        }
    }
}
