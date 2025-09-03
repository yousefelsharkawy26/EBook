using Digital_Library.Core.Models;
using Digital_Library.Infrastructure.UnitOfWork.Implementation;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            Category category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null )
            {
                throw new KeyNotFoundException($"Category with ID : {id} not found");
            }

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EditCartegoryAsync(Category category)
        {
            var category1 = await _unitOfWork.Categories.GetByIdAsync(category.Id);
            if (category1 == null)
            {
                throw new KeyNotFoundException($"Category with ID : {category.Id} not found");
            }

            category1.CategoryName = category.CategoryName;
            category1.Description = category.Description;
            category1.IsApproved = category.IsApproved;
            category1.Books = category.Books;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
