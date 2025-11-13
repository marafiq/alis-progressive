using System.ComponentModel.DataAnnotations;

namespace Jamidon.Models;

public class ComprehensiveValidatorModel
{
    [Required(ErrorMessage = "Required field is required")]
    public string RequiredField { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
    public string EmailWithMultipleValidators { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one number")]
    public string PasswordWithMultipleValidators { get; set; } = string.Empty;

    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
    public int? Age { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? WebsiteUrl { get; set; }

    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? PhoneNumber { get; set; }

    [CreditCard(ErrorMessage = "Please enter a valid credit card number")]
    public string? CreditCard { get; set; }

    [MinLength(3, ErrorMessage = "Tags must have at least 3 items")]
    [MaxLength(10, ErrorMessage = "Tags cannot exceed 10 items")]
    public List<string> Tags { get; set; } = new();

    // MinSelection validation removed per requirements - using standard Required only
    [Required(ErrorMessage = "At least one option must be selected")]
    public List<string> CheckboxOptions { get; set; } = new();
}

