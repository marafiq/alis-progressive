using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Alis.Progressive.TagHelpers.TagHelpers.Core;

/// <summary>
/// Tag helper that renders the Alis framework CSS bundle.
/// Includes Tailwind CSS with shadcn/ui inspired design system.
/// </summary>
[HtmlTargetElement("alis-styles")]
public class AlisStylesTagHelper : TagHelper
{
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "link";
        output.TagMode = TagMode.SelfClosing;
        
        output.Attributes.SetAttribute("rel", "stylesheet");
        output.Attributes.SetAttribute("href", "~/lib/alis/alis.bundle.css");
        output.Attributes.SetAttribute("asp-append-version", "true");

        return Task.CompletedTask;
    }
}

