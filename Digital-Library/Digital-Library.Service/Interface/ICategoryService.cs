using Digital_Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Interface
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(Category category);

        Task DeleteCategoryAsync(string id);

        Task EditCartegoryAsync(Category category);
    }
}
