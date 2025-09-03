using Digital_Library.Core.Models;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public Task<User> ChangeAddress(Guid id, string Address)
        {
            throw new NotImplementedException();
        }

        public Task<User> ChangeName(Guid id, string name)
        {
            throw new NotImplementedException();
        }

        public Task<User> ChangePhoneNumber(Guid id, string PhoneNumber)
        {
            throw new NotImplementedException();
        }
    }
}
