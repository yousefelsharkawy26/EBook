using Digital_Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Interface
{
    public interface ISearchEngine
    {
        public Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
    }
}
