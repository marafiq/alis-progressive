using System.ComponentModel.DataAnnotations;

namespace Alis.Progressive.TagHelpers.Validation;

/// <summary>
/// Validates that a property is required if another property has a specific value.
/// 
/// Examples:
/// [RequiredIf(nameof(AcceptTerms), true, ErrorMessage = "Phone required when terms accepted")]
/// [RequiredIf(nameof(Country), "USA", ErrorMessage = "State required for USA")]
/// [RequiredIf(nameof(UserType), "Premium", ErrorMessage = "Credit card required for premium")]
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class RequiredIfAttribute : ConditionalValidationAttribute
{
    public RequiredIfAttribute(string dependentProperty, object? expectedValue = null)
        : base(dependentProperty, expectedValue, invert: false)
    {
    }

    protected override ValidationResult? ValidateConditionally(object? value, ValidationContext validationContext)
    {
        // Check if the value is provided when required
        if (value == null)
        {
            return new ValidationResult(
                ErrorMessage ?? $"{validationContext.DisplayName} is required when {DependentProperty} is {ExpectedValue}",
                new[] { validationContext.MemberName! });
        }

        // For strings, check if empty or whitespace
        if (value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
        {
            return new ValidationResult(
                ErrorMessage ?? $"{validationContext.DisplayName} is required when {DependentProperty} is {ExpectedValue}",
                new[] { validationContext.MemberName! });
        }

        return ValidationResult.Success;
    }

    public override Dictionary<string, object> GetClientValidationMetadata()
    {
        var metadata = base.GetClientValidationMetadata();
        metadata["validationtype"] = "requiredif";
        return metadata;
    }
}

