using System.Text.Json;
using System.Text.Json.Nodes;
using mcp.Src.Services;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Xunit;

namespace csharp_mcp_example_test;
public class SysMLOpenApiFactoryTests
{

    private string GetSysMlSchemasPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(),"..", "..", "..", "..", "sysmlv2-api-spec", "metamodels");
    }

    [Fact]
    public void Constructor_WithValidPath_LoadsSchemas()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        Assert.NotEmpty(factory._schemas);
    }

    [Fact]
    public void CreateJsonRequestBody_WithValidParameters_ReturnsCorrectJson()
    {
        // Arrange
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var parameters = new Dictionary<string, object?>
        {
            ["name"] = "simple-use-case",
            ["documentation"] = new List<string> { "A simple use case definition" },
            ["actor"] = "SomeActor"
        };

        // Act
        var result = factory.CreateJsonRequestBody("UseCaseDefinition", parameters);

        // Assert
        var jsonDoc = JsonDocument.Parse(result);
        var root = jsonDoc.RootElement;

        Assert.Equal("simple-use-case", root.GetProperty("name").GetString());

        var docProp = root.GetProperty("documentation");
        // Accept only an array for documentation
        Assert.Equal(JsonValueKind.Array, docProp.ValueKind);
        Assert.True(docProp.GetArrayLength() > 0);
        Assert.Equal("A simple use case definition", docProp[0].GetString());

        // Actor should not be present in the generated JSON because the schema defines it as an array of part usages, not a string
        Assert.False(root.TryGetProperty("actor", out _));
    }

    [Fact]
    public void CreateJsonRequestBody_WithMissingRequiredParameter_ThrowsException()
    {
        // Arrange
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var parameters = new Dictionary<string, object?>
        {
            // "name" parameter is missing
            ["documentation"] = new List<string> { "A simple use case definition" }
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => factory.CreateJsonRequestBody("UseCaseDefinition", parameters));

    }

}