using Digital_Library.Core.Models;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;

namespace Digital_Library.Service.Implementation
{
    public class SearchEngine : ISearchEngine
    {
        private readonly IUnitOfWork _unitOfWork;
        public SearchEngine(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return (Task<IEnumerable<Book>>)Enumerable.Empty<Book>();

            searchTerm = searchTerm.ToLower();

            return _unitOfWork.Books.
                GetManyAsync(b => b.Title.ToLower().Contains(searchTerm)
                || b.Author.ToLower().Contains(searchTerm)
                , b => b.Category , b => b.Vendor);
        }
    }
}
