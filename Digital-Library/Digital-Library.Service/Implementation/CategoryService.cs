using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Digital_Library.Service.Implementation
{
	public class CategoryService : ICategoryService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<CategoryService> _logger;

		public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<Response> AddCategory(CategoryRequest request)
		{
			try
			{
				var category = new Category
				{
					CategoryName = request.CategoryName,
					Description = request.Description,
					IsApproved = true 
				};

				await _unitOfWork.Categories.AddAsync(category);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Category '{CategoryName}' added successfully", request.CategoryName);
				return Response.Ok("Category added successfully", category);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while adding category '{CategoryName}'", request.CategoryName);
				return Response.Fail("An error occurred while adding category.");
			}
		}

		public async Task<Response> UpdateCategory(string categoryId, CategoryRequest request)
		{
			try
			{
				var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
				if (category == null)
					return Response.Fail("Category not found.");

				category.CategoryName = request.CategoryName;
				category.Description = request.Description;

				_unitOfWork.Categories.Update(category);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Category '{CategoryId}' updated successfully", categoryId);
				return Response.Ok("Category updated successfully", category);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while updating category '{CategoryId}'", categoryId);
				return Response.Fail("An error occurred while updating category.");
			}
		}

		public async Task<Response> DeleteCategory(string categoryId)
		{
			try
			{
				var category = await _unitOfWork.Categories
								.GetSingleAsync(c => c.Id == categoryId, c => c.Books);

				if (category == null)
					return Response.Fail("Category not found.");

				if (category.Books != null && category.Books.Any())
				{
					_logger.LogWarning("Cannot delete category '{CategoryId}' because it has {BooksCount} books", categoryId, category.Books.Count);
					return Response.Fail("Cannot delete category because it has related books.");
				}

				_unitOfWork.Categories.Delete(category);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Category '{CategoryId}' deleted successfully", categoryId);
				return Response.Ok("Category deleted successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while deleting category '{CategoryId}'", categoryId);
				return Response.Fail("An error occurred while deleting category.");
			}
		}


		public async Task<Response> GetCategoryById(string categoryId)
		{
			var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
			if (category == null)
			{
				_logger.LogWarning("Category not found with Id: {CategoryId}", categoryId);
				return Response.Fail("Category not found.");
			}

			_logger.LogInformation("Category '{CategoryId}' retrieved successfully", categoryId);
			return Response.Ok("Category retrieved successfully", category);
		}

		public async Task<IEnumerable<Category>> GetAllCategories()
		{
			var categories = _unitOfWork.Categories.GetAllQuery();
			_logger.LogInformation("Retrieved {Count} categories", categories.Count());
			return await categories.ToListAsync();
		}

	}
}

