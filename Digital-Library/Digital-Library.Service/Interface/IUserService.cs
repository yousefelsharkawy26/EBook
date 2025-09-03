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
        Task ChangeName(string id , string name);

        Task ChangeAddress(string id , string City , string State , string ZipCode);

        Task ChangePhoneNumber(string id, string PhoneNumber);
    }
}
