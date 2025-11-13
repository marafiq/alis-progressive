using System.ComponentModel.DataAnnotations;

namespace Jamidon.Validation;

/// <summary>
/// Validates that a property is required UNLESS another property has a specific value.
/// This is the inverse of RequiredIf.
/// 
/// Examples:
/// [RequiredUnless(nameof(HasExistingAccount), true, ErrorMessage = "Password required for new accounts")]
/// [RequiredUnless(nameof(PaymentMethod), "Cash", ErrorMessage = "Card details required for non-cash payments")]
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class RequiredUnlessAttribute : ConditionalValidationAttribute
{
    public RequiredUnlessAttribute(string dependentProperty, object? expectedValue = null)
        : base(dependentProperty, expectedValue, invert: true) // Invert the condition
    {
    }

    protected override ValidationResult? ValidateConditionally(object? value, ValidationContext validationContext)
    {
        // Check if the value is provided when required
        if (value == null)
        {
            return new ValidationResult(
                ErrorMessage ?? $"{validationContext.DisplayName} is required unless {DependentProperty} is {ExpectedValue}",
                new[] { validationContext.MemberName! });
        }

        // For strings, check if empty or whitespace
        if (value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
        {
            return new ValidationResult(
                ErrorMessage ?? $"{validationContext.DisplayName} is required unless {DependentProperty} is {ExpectedValue}",
                new[] { validationContext.MemberName! });
        }

        return ValidationResult.Success;
    }

    public override Dictionary<string, object> GetClientValidationMetadata()
    {
        var metadata = base.GetClientValidationMetadata();
        metadata["validationtype"] = "requiredunless";
        return metadata;
    }
}

