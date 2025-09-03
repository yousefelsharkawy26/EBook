using Microsoft.AspNetCore.Identity;

namespace Digital_Library.Core.Models;

public class User: IdentityUser
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }
}

