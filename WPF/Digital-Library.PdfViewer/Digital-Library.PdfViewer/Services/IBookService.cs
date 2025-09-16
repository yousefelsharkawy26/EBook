using Digital_Library.PdfViewer.Enum;
using Digital_Library.PdfViewer.Models;

namespace Digital_Library.PdfViewer.Services
{
    public interface IBookService
    {
        Task<PagedResult<UserBookDto>> GetMyBooksAsync(int page, int pageSize);
        Task<(byte[] DecryptedPdf, string Email, FormatType Type)> GetAndDecryptPdfAsync(string bookId);
    }
}