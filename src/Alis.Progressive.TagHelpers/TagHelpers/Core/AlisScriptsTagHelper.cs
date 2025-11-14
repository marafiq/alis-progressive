using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Alis.Progressive.TagHelpers.TagHelpers.Core;

/// <summary>
/// Tag helper that renders the Alis framework JavaScript bundle.
/// Includes HTMX, Alpine.js, validation, and framework extension.
/// </summary>
[HtmlTargetElement("alis-scripts")]
public class AlisScriptsTagHelper : TagHelper
{
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "script";
        output.TagMode = TagMode.StartTagAndEndTag;
        
        output.Attributes.SetAttribute("src", "~/lib/alis/alis.bundle.js");
        output.Attributes.SetAttribute("type", "module");
        output.Attributes.SetAttribute("asp-append-version", "true");

        return Task.CompletedTask;
    }
}

