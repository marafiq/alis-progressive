using Xunit;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Alis.Progressive.TagHelpers.TagHelpers.Core;
using System.Threading.Tasks;

namespace Alis.Progressive.TagHelpers.Tests.TagHelpers;

public class AlisScriptsTagHelperTests
{
    [Fact]
    public async Task Process_RendersScriptTag()
    {
        // Arrange
        var tagHelper = new AlisScriptsTagHelper();
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString());
        var output = new TagHelperOutput(
            "alis-scripts",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("script", output.TagName);
        Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
        
        var src = output.Attributes.FirstOrDefault(a => a.Name == "src");
        Assert.NotNull(src);
        Assert.Equal("~/lib/alis/alis.bundle.js", src.Value);
    }

    [Fact]
    public async Task Process_AddsTypeModule()
    {
        // Arrange
        var tagHelper = new AlisScriptsTagHelper();
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString());
        var output = new TagHelperOutput(
            "alis-scripts",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var type = output.Attributes.FirstOrDefault(a => a.Name == "type");
        Assert.NotNull(type);
        Assert.Equal("module", type.Value);
    }

    [Fact]
    public async Task Process_AddsAspAppendVersion()
    {
        // Arrange
        var tagHelper = new AlisScriptsTagHelper();
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString());
        var output = new TagHelperOutput(
            "alis-scripts",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var aspAppendVersion = output.Attributes.FirstOrDefault(a => a.Name == "asp-append-version");
        Assert.NotNull(aspAppendVersion);
        Assert.Equal("true", aspAppendVersion.Value);
    }
}

