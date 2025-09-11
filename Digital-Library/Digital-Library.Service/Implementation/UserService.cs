using Digital_Library.Core.Constant;
using Digital_Library.Core.Enum;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Digital_Library.Service.Implementation;
public class UserService : IUserService
{
	private readonly UserManager<User> _userManager;
	private readonly IFileService _fileService;
	private readonly ILogger<UserService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(UserManager<User> userManager, 
                       IFileService fileService, 
                       ILogger<UserService> logger, 
                       IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _fileService = fileService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Response> GetProfileAsync(string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);

		if (user == null)
			return Response.Fail("User not found");

		var profile = new
		{
			user.Id,
			user.UserName,
			user.Email,
			user.FullName,
			user.ImageUrl
		};

		_logger.LogInformation("Profile retrieved for user {UserId}", userId);
		return Response.Ok("Profile retrieved successfully", profile);
	}

	public async Task<Response> UpdateProfileAsync(string userId, UserRequest request)
	{
		var user = await _userManager.FindByIdAsync(userId);

		if (user == null)
			return Response.Fail("User not found");

		if (!string.IsNullOrEmpty(request.FullName))
			user.FullName = request.FullName;

		if (request.ImageProfile != null)
		{
			if (!string.IsNullOrEmpty(user.ImageUrl))
				await _fileService.DeleteFile(user.ImageUrl);

			user.ImageUrl = await _fileService.AddFile(request.ImageProfile, FileFoldersName.UserProfileImage);
		}

		var result = await _userManager.UpdateAsync(user);

		if (!result.Succeeded)
		{
			var errors = string.Join(", ", result.Errors.Select(e => e.Description));
			_logger.LogError("Failed to update profile for user {UserId}: {Errors}", userId, errors);
			return Response.Fail("Failed to update profile: " + errors);
		}

		_logger.LogInformation("Profile updated successfully for user {UserId}", userId);
		return Response.Ok("Profile updated successfully", user);
	}

    public async Task<IEnumerable<CustomerSummaryViewModel>> GetAllCustomersAsync()
    {

        var customers = await _userManager.GetUsersInRoleAsync("Customer");

        return customers.Select(user => new CustomerSummaryViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            RegistrationDate = user.CreationDate,
            IsActive = !user.LockoutEnd.HasValue || user.LockoutEnd.Value <= DateTimeOffset.UtcNow,
            TotalOrders = _unitOfWork.OrderHeaders.GetManyQuery(o => o.Order.UserId == user.Id, 
            new System.Linq.Expressions.Expression<Func<OrderHeader, object>>[]
            {
                o => o.Order    
            }).Count() 
        });
    }

    public async Task<CustomerDetailsViewModel?> GetCustomerDetailsAsync(string customerId)
    {
        var user = await _userManager.FindByIdAsync(customerId);
        if (user == null) return null;

        var recentOrders = (await _unitOfWork.Orders
            .GetManyAsync(o => o.UserId == customerId, 
            new Expression<Func<Order, object>>[]
            {
                o => o.OrderHeaders
            }))
            .OrderByDescending(o => o.OrderDate)
            .Take(10) // جلب آخر 10 طلبات فقط
            .Select(o => new OrderSummaryViewModel
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                OrderTotal = o.OrderHeaders.Sum(u => u.TotalAmount),
                OrderStatus = o.OrderHeaders.All(u => u.Status == Status.Pending) ? "Pending" :
                              o.OrderHeaders.All(u => u.Status == Status.Complete)? "Completed":
                              o.OrderHeaders.All(u => u.Status == Status.Cancelled)? "Cancelled": "In Progress",
            }).ToList();

        return new CustomerDetailsViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RegistrationDate = user.CreationDate,
            IsActive = !user.LockoutEnd.HasValue || user.LockoutEnd.Value <= DateTimeOffset.UtcNow,
            RecentOrders = recentOrders
        };
    }

	public async Task<bool> ToggleUserStatusAsync(string customerId)
	{
		var user = await _userManager.FindByIdAsync(customerId);
		if (user == null) return false;

		if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
		{
			await _userManager.SetLockoutEndDateAsync(user, null);

			await _userManager.UpdateSecurityStampAsync(user);
		}
		else
		{
			await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

			await _userManager.UpdateSecurityStampAsync(user);
		}

		return true;
	}

}
