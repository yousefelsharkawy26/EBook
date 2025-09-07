using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Digital_Library.ViewComponents
{
    public class ToFiveSellingItemsViewComponent: ViewComponent
    {
        private readonly IBookService _bookService;

        public ToFiveSellingItemsViewComponent(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var topFiveBooks = await _bookService.GetBestTenSellingBook();

            return View(topFiveBooks);
        }
    }
}
