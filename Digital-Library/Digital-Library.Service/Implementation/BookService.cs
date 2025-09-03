using Digital_Library.Core.Models;
using Digital_Library.Infrastructure.Repositories.Interface;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Digital_Library.Service.Implementation
{
    public class BookService : IBookService
    {
      
        private readonly IUnitOfWork _unitOfWork;

        public BookService( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> DeleteBook(Guid id)
        {
            var getBook = await _unitOfWork.Books.GetByIdAsync(id.ToString());
            if (getBook != null)
            {
                _unitOfWork.Books.Delete(getBook);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }

        public async Task<Book> UpdateBook(Book model)
        {
            var GetBook = await _unitOfWork.Books.GetByIdAsync(model.Id.ToString());

            if (GetBook == null)
            {
                return null;
            }
            GetBook.Title = model.Title;
            GetBook.Description = model.Description;
            GetBook.Author = model.Author;
            GetBook.PDFFilePath = model.PDFFilePath;
            GetBook.CategoryID = model.CategoryID;
            GetBook.PricePhysical = model.PricePhysical;
            if(model.IsBorrowable == true)
                GetBook.PricePDFPerDay = model.PricePDFPerDay;
            
            GetBook.HasPDF = model.HasPDF;
            GetBook.Stock = model.Stock;
            GetBook.UploadDate = model.UploadDate;

            _unitOfWork.Books.Update(GetBook);
            await _unitOfWork.SaveChangesAsync();
            
            return GetBook;

        }

        public async Task<Book> UploadBook(Book model)
        {
            Book book = new Book()
            {
                Title = model.Title,
                Author = model.Author,
                Description = model.Description,
                Stock = model.Stock,
                CategoryID = model.CategoryID,
                HasPDF = model.HasPDF,
                IsBorrowable = model.IsBorrowable,
                PDFFilePath = model.PDFFilePath,
                PricePhysical = model.PricePhysical,
                UploadDate = model.UploadDate,
            };
            if(book.IsBorrowable == true)
            {
                book.PricePDFPerDay = model.PricePDFPerDay;
            }
            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();
            return book;
        }
    }
}
