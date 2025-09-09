using System.ComponentModel.DataAnnotations;

namespace Digital_Library.Core.ViewModels;
public class ContactViewModel
{
    [Required(ErrorMessage = "Please enter your name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please enter your email address")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please enter a subject")]
    [StringLength(100)]
    public string Subject { get; set; }

    [Required(ErrorMessage = "Please enter your message")]
    [StringLength(1000, ErrorMessage = "Your message cannot be longer than 1000 characters")]
    public string Message { get; set; }
}