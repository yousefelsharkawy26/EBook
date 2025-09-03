using Digital_Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Service.Interface
{
    public interface IUserService
    {
        Task<User> ChangeName(Guid id , string name);

        Task<User> ChangeAddress(Guid id , string Address);

        Task<User> ChangePhoneNumber(Guid id, string PhoneNumber);
    }
}
