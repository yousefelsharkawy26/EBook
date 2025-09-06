using Digital_Library.Core.Constant;
using Digital_Library.Core.Enum;
using Digital_Library.Core.Enums;
using Digital_Library.Core.Models;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Core.ViewModels.Responses;
using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Digital_Library.Service.Services
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<OrderService> _logger;

		public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}


	}
}
