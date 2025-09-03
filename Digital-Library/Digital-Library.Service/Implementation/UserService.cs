using Digital_Library.Infrastructure.UnitOfWork.Interface;
using Digital_Library.Service.Interface;

namespace Digital_Library.Service.Implementation;
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task ChangeAddress(string id, string City, string State, string ZipCode)
    {
        var vendor = await _unitOfWork.Vendors.GetByIdAsync(id);
        if (vendor == null)
        {
            throw new KeyNotFoundException($"User with this {id} not found");
        }

        vendor.City = City;
        vendor.State = State;
        vendor.ZipCode = ZipCode;

        await _unitOfWork.SaveChangesAsync();

    }

    public async Task ChangeName(string id, string name)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with this {id} not found");
        }

        user.FullName = name;
    }

    public async Task ChangePhoneNumber(string id, string PhoneNumber)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with this {id} not found");
        }

        user.PhoneNumber = PhoneNumber;
    }
}
