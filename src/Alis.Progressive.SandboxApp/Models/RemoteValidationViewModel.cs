using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Jamidon.Models;

public class RemoteValidationViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [Remote(action: "CheckUsername", controller: "Samples", ErrorMessage = "Username is already taken")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Remote(action: "CheckEmail", controller: "Samples", ErrorMessage = "Email is already registered")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters")]
    public string Password { get; set; } = string.Empty;
}

