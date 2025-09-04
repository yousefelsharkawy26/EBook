using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Interface
{
    public interface ICategoryService
    {
		Task<Response> AddCategory(CategoryRequest request);
		Task<Response> UpdateCategory(string categoryId, CategoryRequest request);
		Task<Response> DeleteCategory(string categoryId);
		Task<Response> GetCategoryById(string categoryId);
		Task<IEnumerable<Category>> GetAllCategories();
	}
}
