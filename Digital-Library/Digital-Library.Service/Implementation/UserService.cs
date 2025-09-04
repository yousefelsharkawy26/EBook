using Digital_Library.Core.Constant;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Digital_Library.Service.Implementation;
public class UserService : IUserService
{
	private readonly UserManager<User> _userManager;
	private readonly IFileService _fileService;
	private readonly ILogger<UserService> _logger;

	public UserService(UserManager<User> userManager, IFileService fileService, ILogger<UserService> logger)
	{
		_userManager = userManager;
		_fileService = fileService;
		_logger = logger;
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
}
