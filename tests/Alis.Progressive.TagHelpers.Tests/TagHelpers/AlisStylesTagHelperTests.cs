using Xunit;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Alis.Progressive.TagHelpers.TagHelpers.Core;
using System.Threading.Tasks;

namespace Alis.Progressive.TagHelpers.Tests.TagHelpers;

public class AlisStylesTagHelperTests
{
    [Fact]
    public async Task Process_RendersLinkTag()
    {
        // Arrange
        var tagHelper = new AlisStylesTagHelper();
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString());
        var output = new TagHelperOutput(
            "alis-styles",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("link", output.TagName);
        Assert.Equal(TagMode.SelfClosing, output.TagMode);
    }

    [Fact]
    public async Task Process_AddsRelStylesheet()
    {
        // Arrange
        var tagHelper = new AlisStylesTagHelper();
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString());
        var output = new TagHelperOutput(
            "alis-styles",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var rel = output.Attributes.FirstOrDefault(a => a.Name == "rel");
        Assert.NotNull(rel);
        Assert.Equal("stylesheet", rel.Value);
    }

    [Fact]
    public async Task Process_AddsHrefToBundle()
    {
        // Arrange
        var tagHelper = new AlisStylesTagHelper();
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString());
        var output = new TagHelperOutput(
            "alis-styles",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var href = output.Attributes.FirstOrDefault(a => a.Name == "href");
        Assert.NotNull(href);
        Assert.Equal("~/lib/alis/alis.bundle.css", href.Value);
    }

    [Fact]
    public async Task Process_AddsAspAppendVersion()
    {
        // Arrange
        var tagHelper = new AlisStylesTagHelper();
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString());
        var output = new TagHelperOutput(
            "alis-styles",
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

