using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Jamidon.Validation;

/// <summary>
/// Base class for conditional validation attributes.
/// Supports complex conditional logic using property values and expressions.
/// </summary>
public abstract class ConditionalValidationAttribute : ValidationAttribute
{
    protected string DependentProperty { get; }
    protected object? ExpectedValue { get; }
    protected bool Invert { get; }
    
    /// <summary>
    /// Cache for property info to avoid reflection overhead
    /// </summary>
    private static readonly Dictionary<(Type, string), PropertyInfo?> PropertyCache = new();
    private static readonly object CacheLock = new();

    protected ConditionalValidationAttribute(string dependentProperty, object? expectedValue = null, bool invert = false)
    {
        DependentProperty = dependentProperty ?? throw new ArgumentNullException(nameof(dependentProperty));
        ExpectedValue = expectedValue;
        Invert = invert;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.ObjectInstance == null)
        {
            return ValidationResult.Success;
        }

        // Get the dependent property value
        var dependentValue = GetDependentPropertyValue(validationContext.ObjectInstance, DependentProperty);
        
        // Check if the condition is met
        bool conditionMet = EvaluateCondition(dependentValue);
        
        // Invert if needed (for RequiredUnless scenarios)
        if (Invert)
        {
            conditionMet = !conditionMet;
        }

        // If condition is not met, validation passes (field is not required)
        if (!conditionMet)
        {
            return ValidationResult.Success;
        }

        // Condition is met, perform the actual validation
        return ValidateConditionally(value, validationContext);
    }

    /// <summary>
    /// Override this to implement specific validation logic when condition is met
    /// </summary>
    protected abstract ValidationResult? ValidateConditionally(object? value, ValidationContext validationContext);

    /// <summary>
    /// Evaluates whether the condition is met based on the dependent property value
    /// </summary>
    protected virtual bool EvaluateCondition(object? dependentValue)
    {
        // If ExpectedValue is null, check if dependent property is truthy
        if (ExpectedValue == null)
        {
            return IsTruthy(dependentValue);
        }

        // Compare with expected value
        return AreEqual(dependentValue, ExpectedValue);
    }

    /// <summary>
    /// Gets the value of a dependent property using cached reflection
    /// </summary>
    protected object? GetDependentPropertyValue(object container, string propertyName)
    {
        var containerType = container.GetType();
        var cacheKey = (containerType, propertyName);

        // Try to get from cache
        if (!PropertyCache.TryGetValue(cacheKey, out var propertyInfo))
        {
            lock (CacheLock)
            {
                if (!PropertyCache.TryGetValue(cacheKey, out propertyInfo))
                {
                    propertyInfo = containerType.GetProperty(propertyName);
                    PropertyCache[cacheKey] = propertyInfo;
                }
            }
        }

        if (propertyInfo == null)
        {
            throw new InvalidOperationException(
                $"Property '{propertyName}' not found on type '{containerType.Name}'");
        }

        return propertyInfo.GetValue(container);
    }

    /// <summary>
    /// Determines if a value is "truthy" (non-null, non-empty, non-false)
    /// </summary>
    protected bool IsTruthy(object? value)
    {
        return value switch
        {
            null => false,
            bool b => b,
            string s => !string.IsNullOrWhiteSpace(s),
            int i => i != 0,
            long l => l != 0,
            decimal d => d != 0,
            double db => db != 0,
            float f => f != 0,
            _ => true // Non-null object is truthy
        };
    }

    /// <summary>
    /// Compares two values for equality, handling different types
    /// </summary>
    protected bool AreEqual(object? value1, object? value2)
    {
        if (value1 == null && value2 == null) return true;
        if (value1 == null || value2 == null) return false;

        // Handle string comparison
        if (value1 is string s1 && value2 is string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        // Handle enum comparison
        if (value1.GetType().IsEnum || value2.GetType().IsEnum)
        {
            return string.Equals(value1.ToString(), value2.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        // Try direct equality
        return Equals(value1, value2);
    }

    /// <summary>
    /// Gets metadata for client-side validation
    /// </summary>
    public virtual Dictionary<string, object> GetClientValidationMetadata()
    {
        return new Dictionary<string, object>
        {
            { "dependentproperty", DependentProperty },
            { "expectedvalue", ExpectedValue?.ToString() ?? "true" },
            { "invert", Invert }
        };
    }
}

