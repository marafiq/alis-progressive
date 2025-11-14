using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace Alis.Progressive.TagHelpers.Validation;

/// <summary>
/// Adapter for conditional validation attributes to generate client-side validation metadata
/// </summary>
public class ConditionalValidationAttributeAdapter<TAttribute> : AttributeAdapterBase<TAttribute>
    where TAttribute : ConditionalValidationAttribute
{
    public ConditionalValidationAttributeAdapter(TAttribute attribute, IStringLocalizer? stringLocalizer)
        : base(attribute, stringLocalizer)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // Get metadata from the attribute
        var metadata = Attribute.GetClientValidationMetadata();
        
        // Get validation type (requiredif or requiredunless)
        var validationType = metadata["validationtype"].ToString()!;
        
        // Add base data-val attribute
        MergeAttribute(context.Attributes, "data-val", "true");
        
        // Add validation rule type
        MergeAttribute(context.Attributes, $"data-val-{validationType}", GetErrorMessage(context));
        
        // Add dependent property
        MergeAttribute(context.Attributes, $"data-val-{validationType}-dependentproperty", 
            metadata["dependentproperty"].ToString()!);
        
        // Add expected value
        MergeAttribute(context.Attributes, $"data-val-{validationType}-expectedvalue", 
            metadata["expectedvalue"].ToString()!);
        
        // Add invert flag for RequiredUnless
        if (metadata.ContainsKey("invert") && (bool)metadata["invert"])
        {
            MergeAttribute(context.Attributes, $"data-val-{validationType}-invert", "true");
        }
    }

    public override string GetErrorMessage(ModelValidationContextBase validationContext)
    {
        if (validationContext == null)
        {
            throw new ArgumentNullException(nameof(validationContext));
        }

        return GetErrorMessage(
            validationContext.ModelMetadata, 
            validationContext.ModelMetadata.GetDisplayName());
    }
}

/// <summary>
/// Validation attribute adapter provider for conditional validation
/// </summary>
public class ConditionalValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

    public IAttributeAdapter? GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer? stringLocalizer)
    {
        // Handle our custom conditional validation attributes
        return attribute switch
        {
            RequiredIfAttribute requiredIf => 
                new ConditionalValidationAttributeAdapter<RequiredIfAttribute>(requiredIf, stringLocalizer),
            RequiredUnlessAttribute requiredUnless => 
                new ConditionalValidationAttributeAdapter<RequiredUnlessAttribute>(requiredUnless, stringLocalizer),
            _ => _baseProvider.GetAttributeAdapter(attribute, stringLocalizer)
        };
    }
}

