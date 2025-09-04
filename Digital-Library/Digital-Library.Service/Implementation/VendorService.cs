using Digital_Library.Core.Constant;
using Digital_Library.Core.Enums;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Digital_Library.Service.Services
{
	public class VendorService : IVendorService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IFileService _fileService;
		private readonly ILogger<VendorService> _logger;
		private readonly IEmailSender _emailSender;

		public VendorService(IUnitOfWork unitOfWork, IFileService fileService, ILogger<VendorService> logger, IEmailSender emailSender)
		{
			_unitOfWork = unitOfWork;
			_fileService = fileService;
			_logger = logger;
			_emailSender = emailSender;
		}

		public async Task<Response> SubmitVendorRequestAsync(VendorRequest request, string userId)
		{
			try
			{
				var existingVendor = await _unitOfWork.Vendors.GetSingleAsync(v => v.UserId == userId);

				if (existingVendor != null)
				{
					if (existingVendor.Status == VendorStatus.Pending)
						return Response.Fail("You already have a pending vendor request. Please wait for review.");

					if (existingVendor.Status == VendorStatus.Approved)
						return Response.Fail("You are already registered as a vendor.");
				}

				var logoPath = await _fileService.AddFile(request.LibraryLogo, FileFoldersName.VendorLibraryLogo);

				var identityImageUrls = new List<VendorIdentityImagesUrl>();
				if (request.IdentityImages != null)
				{
					foreach (var img in request.IdentityImages)
					{
						var imgPath = await _fileService.AddFile(img, FileFoldersName.VendorIdentification);
						identityImageUrls.Add(new VendorIdentityImagesUrl { ImageUrl = imgPath });
					}
				}

				var vendor = new Vendor
				{
					LibraryName = request.LibraryName,
					LibraryLogoUrl = logoPath,
					City = request.City,
					State = request.State,
					ZipCode = request.ZipCode,
					ContactNumber = request.ContactNumber,
					UserId = userId,
					Status = VendorStatus.Pending,
					SubmittedAt = DateTime.UtcNow,
					VendorIdentityImagesUrls = identityImageUrls
				};

				await _unitOfWork.Vendors.AddAsync(vendor);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Vendor request submitted by User {UserId}", userId);
				return Response.Ok("Vendor request submitted successfully.", vendor);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while submitting vendor request by User {UserId}", userId);
				return Response.Fail("An error occurred while submitting vendor request.");
			}
		}

		public async Task<Vendor?> GetVendorByIdAsync(string vendorId, bool includeBooks = false)
		{
			if (includeBooks)
			{
				return await _unitOfWork.Vendors.GetSingleAsync(
								v => v.Id == vendorId,
								v => v.User,
								v => v.VendorIdentityImagesUrls,
								v => v.Books
				);
			}

			return await _unitOfWork.Vendors.GetSingleAsync(
							v => v.Id == vendorId,
							v => v.User,
							v => v.VendorIdentityImagesUrls
			);
		}


		public async Task<IEnumerable<Vendor>> GetVendorsAsync(VendorStatus? status = null)
		{
			IQueryable<Vendor> query = _unitOfWork.Vendors.GetAllQuery(
							includes: new Expression<Func<Vendor, object>>[] { v => v.User, v => v.VendorIdentityImagesUrls }
			);

			if (status.HasValue)
				query = query.Where(v => v.Status == status.Value);

			_logger.LogInformation("Retrieved vendor list with status: {Status}", status?.ToString() ?? "All");

			return await query.ToListAsync();
		}


		public async Task<Response> UpdateVendorProfileAsync(string vendorId, VendorUpdateRequest request)
		{
			try
			{
				var vendor = await _unitOfWork.Vendors.GetByIdAsync(vendorId);
				if (vendor == null)
					return Response.Fail("Vendor not found.");

				vendor.LibraryName = request.LibraryName ?? vendor.LibraryName;
				vendor.City = request.City ?? vendor.City;
				vendor.State = request.State ?? vendor.State;
				vendor.ZipCode = request.ZipCode ?? vendor.ZipCode;
				vendor.ContactNumber = request.ContactNumber ?? vendor.ContactNumber;

				if (request.LibraryLogo != null)
				{
					await _fileService.DeleteFile(vendor.LibraryLogoUrl);
					vendor.LibraryLogoUrl = await _fileService.AddFile(request.LibraryLogo, FileFoldersName.VendorLibraryLogo);
				}

				_unitOfWork.Vendors.Update(vendor);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Vendor {VendorId} profile updated", vendorId);
				return Response.Ok("Vendor profile updated successfully.", vendor);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while updating vendor {VendorId}", vendorId);
				return Response.Fail("An error occurred while updating vendor profile.");
			}
		}

		public async Task<Response> ChangeStatusAsync(string vendorId, VendorStatus status, string? reason = null)
		{
			try
			{
				var vendor = await _unitOfWork.Vendors.GetSingleAsync(
								v => v.Id == vendorId,
								v => v.User
				);
				if (vendor == null)
					return Response.Fail("Vendor not found.");

				vendor.Status = status;
				vendor.RejectionReason = status == VendorStatus.Rejected ? reason : null;
				vendor.ReviewedAt = DateTime.UtcNow;

				if (status == VendorStatus.Approved)
				{
					string htmlMessage = $"<p>Hello {vendor.User?.FullName},</p>" +
																										$"<p>Your vendor request for '<strong>{vendor.LibraryName}</strong>' has been <strong>approved</strong>.</p>" +
																										"<p>Thank you.</p>";

					await _emailSender.SendEmailAsync(vendor.User!.Email, "Vendor Request Approved", htmlMessage);
					_unitOfWork.Vendors.Update(vendor);
				}
				else if (status == VendorStatus.Rejected)
				{
					string htmlMessage = $"<p>Hello {vendor.User?.FullName},</p>" +
																										$"<p>Your vendor request for '<strong>{vendor.LibraryName}</strong>' has been <strong>rejected</strong>.</p>" +
																										$"<p>Reason: {reason ?? "Not specified"}</p>" +
																										"<p>You can submit a new request if you wish.</p>";

					await _emailSender.SendEmailAsync(vendor.User!.Email, "Vendor Request Rejected", htmlMessage);

					_unitOfWork.Vendors.Delete(vendor);
				}

				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Vendor {VendorId} status changed to {Status}", vendorId, status);
				return Response.Ok($"Vendor status updated to {status}.", vendor);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while changing status for vendor {VendorId}", vendorId);
				return Response.Fail("An error occurred while updating vendor status.");
			}
		}

	}
}
