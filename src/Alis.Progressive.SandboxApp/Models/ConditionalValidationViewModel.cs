using System.ComponentModel.DataAnnotations;
using Alis.Progressive.TagHelpers.Validation;

namespace Alis.Progressive.SandboxApp.Models;

/// <summary>
/// Comprehensive conditional validation examples demonstrating various scenarios
/// </summary>
public class ConditionalValidationViewModel
{
    // ============================================================================
    // Example 1: Checkbox-based conditional (RequiredIf)
    // ============================================================================
    
    [Display(Name = "Accept Terms and Conditions")]
    public bool AcceptTerms { get; set; }
    
    [Display(Name = "Phone Number")]
    [RequiredIf(nameof(AcceptTerms), true, ErrorMessage = "Phone number is required when terms are accepted")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? PhoneNumber { get; set; }
    
    // ============================================================================
    // Example 2: Dropdown-based conditional (RequiredIf with different values)
    // ============================================================================
    
    [Required(ErrorMessage = "Country is required")]
    [Display(Name = "Country")]
    public string Country { get; set; } = string.Empty;
    
    [Display(Name = "State")]
    [RequiredIf(nameof(Country), "USA", ErrorMessage = "State is required for USA")]
    public string? State { get; set; }
    
    [Display(Name = "Province")]
    [RequiredIf(nameof(Country), "Canada", ErrorMessage = "Province is required for Canada")]
    public string? Province { get; set; }
    
    // ============================================================================
    // Example 3: User type conditional
    // ============================================================================
    
    [Required(ErrorMessage = "User type is required")]
    [Display(Name = "User Type")]
    public string UserType { get; set; } = "Standard";
    
    [Display(Name = "Credit Card")]
    [RequiredIf(nameof(UserType), "Premium", ErrorMessage = "Credit card is required for Premium accounts")]
    [CreditCard(ErrorMessage = "Please enter a valid credit card number")]
    public string? CreditCard { get; set; }
    
    // ============================================================================
    // Example 4: RequiredUnless (inverse logic)
    // ============================================================================
    
    [Display(Name = "I have an existing account")]
    public bool HasExistingAccount { get; set; }
    
    [Display(Name = "New Password")]
    [RequiredUnless(nameof(HasExistingAccount), true, ErrorMessage = "Password is required for new accounts")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be 8-20 characters")]
    public string? NewPassword { get; set; }
    
    // ============================================================================
    // Always required field for comparison
    // ============================================================================
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;
}

