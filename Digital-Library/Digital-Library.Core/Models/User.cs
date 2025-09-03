using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digital_Library.Core.Models;

public class User: IdentityUser
{
 [Key]
 public override	string Id { get; set; } = Guid.NewGuid().ToString();
 [Required]
	public string FullName { get; set; }
 public string? ImageUrl { get; set; }
 public Cart? Cart { get; set; }
 public Vendor? Vendor { get; set; }

 public ICollection<Borrowing>? borrowings { get; set; }
 public ICollection<Order>? Orders { get; set; }



}

