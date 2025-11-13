using System.ComponentModel.DataAnnotations;
using Jamidon.Validation;

namespace Jamidon.Models;

/// <summary>
/// Simple conditional validation example - Phone required when terms accepted
/// </summary>
public class SimpleConditionalViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;
    
    [Display(Name = "Accept Terms and Conditions")]
    public bool AcceptTerms { get; set; }
    
    [Display(Name = "Phone Number")]
    [RequiredIf(nameof(AcceptTerms), true, ErrorMessage = "Phone number is required when terms are accepted")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? PhoneNumber { get; set; }
}

