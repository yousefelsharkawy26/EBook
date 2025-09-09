using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Models;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Digital_Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        public HomeController(ILogger<HomeController> logger,
                              ICategoryService categoryService,
                              IEmailSender emailSender,
                              IWebHostEnvironment webHostEnvironment,
                              IUrlHelperFactory urlHelperFactory,
                              IActionContextAccessor actionContextAccessor)
        {
            _logger = logger;
            _categoryService = categoryService;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {

            if (ModelState.IsValid)
            {
                string templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "html/ContactConfirmation.html");
                string userBody = await System.IO.File.ReadAllTextAsync(templatePath);
                var actionContext = _actionContextAccessor.ActionContext;
                var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);

                var siteLink = urlHelper.Action("Index", "Home", null, actionContext.HttpContext.Request.Scheme);

                userBody = userBody.Replace("[User's Name]", model.Name)
                                   .Replace("[App Name]", "E-BOOK")
                                   .Replace("[Your Website URL]", siteLink)
                                   .Replace("[Company Name]", "ITI")
                                   .Replace("[Company Address]", "Mansoura University");

                try
                {
                    await _emailSender.SendEmailAsync(model.Email, model.Subject, userBody);
                    _logger.LogInformation($"Message Send Successfully From {nameof(Contact)}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, $"in {nameof(Contact)}");
                    return View(model);
                }
            }
            return View();

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
