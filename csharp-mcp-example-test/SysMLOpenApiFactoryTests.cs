using System.Text.Json;
using System.Text.Json.Nodes;
using mcp.Src.Services;
using Xunit;

namespace csharp_mcp_example_test;

public class SysMLOpenApiFactoryTests
{
    private string GetSysMlSchemasPath() =>
        Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "sysmlv2-api-spec", "metamodels");

    // ── Constructor ───────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidPath_LoadsSchemas()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        Assert.NotEmpty(factory._schemas);
    }

    [Fact]
    public void Constructor_WithInvalidPath_ThrowsDirectoryNotFoundException()
    {
        Assert.Throws<DirectoryNotFoundException>(
            () => new SysMLMetaModelFactory("/nonexistent/path/to/schemas"));
    }

    // ── GetAvailableSchemas ────────────────────────────────────────────────────

    [Fact]
    public void GetAvailableSchemas_ReturnsNonEmptyList()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var schemas = factory.GetAvailableSchemas().ToList();
        Assert.NotEmpty(schemas);
        Assert.Contains("Package", schemas);
        Assert.Contains("UseCaseDefinition", schemas);
    }

    // ── GetSchema ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetSchema_WithKnownType_ReturnsNonNullNode()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var schema = factory.GetSchema("Package");
        Assert.NotNull(schema);
    }

    [Fact]
    public void GetSchema_WithUnknownType_ReturnsNull()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var schema = factory.GetSchema("DoesNotExist");
        Assert.Null(schema);
    }

    // ── CreateJsonRequestBody ─────────────────────────────────────────────────

    [Fact]
    public void CreateJsonRequestBody_WithValidParameters_ReturnsCorrectJson()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var parameters = new Dictionary<string, object?>
        {
            ["name"] = "simple-use-case",
            ["documentation"] = new List<string> { "A simple use case definition" },
            ["actor"] = "SomeActor"
        };

        var result = factory.CreateJsonRequestBody("UseCaseDefinition", parameters);

        var jsonDoc = JsonDocument.Parse(result);
        var root = jsonDoc.RootElement;

        Assert.Equal("simple-use-case", root.GetProperty("name").GetString());

        var docProp = root.GetProperty("documentation");
        Assert.Equal(JsonValueKind.Array, docProp.ValueKind);
        Assert.True(docProp.GetArrayLength() > 0);
        Assert.Equal("A simple use case definition", docProp[0].GetString());

        // "actor" is defined as an array of part-usage refs, not a string — it should be excluded
        Assert.False(root.TryGetProperty("actor", out _));
    }

    [Fact]
    public void CreateJsonRequestBody_WhenParameterNotProvided_OmitsFieldFromOutput()
    {
        // The factory only includes parameters that are explicitly supplied;
        // it does not validate required fields — callers are responsible for completeness.
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var parameters = new Dictionary<string, object?>
        {
            ["documentation"] = new List<string> { "A use case without a name" }
        };

        var result = factory.CreateJsonRequestBody("UseCaseDefinition", parameters);

        var jsonDoc = JsonDocument.Parse(result);
        var root = jsonDoc.RootElement;

        // "name" was not supplied, so it must not appear in the output
        Assert.False(root.TryGetProperty("name", out _),
            "factory must not emit a field the caller did not supply");
        // "documentation" was supplied and should be present
        Assert.True(root.TryGetProperty("documentation", out _));
    }

    [Fact]
    public void CreateJsonRequestBody_WithUnknownElementName_ThrowsArgumentException()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        Assert.Throws<ArgumentException>(
            () => factory.CreateJsonRequestBody("NonExistentElement",
                new Dictionary<string, object?> { ["name"] = "x" }));
    }

    // ── GetTypeAttributeInfo ──────────────────────────────────────────────────

    [Fact]
    public void GetTypeAttributeInfo_WithValidType_ReturnsSplitAttributes()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());

        var (required, allProperties) = factory.GetTypeAttributeInfo("Package");

        Assert.NotEmpty(required);
        Assert.NotEmpty(allProperties);

        // @id and @type are required by every SysML schema
        Assert.Contains("@id", required);
        Assert.Contains("@type", required);

        // Every required key must also appear in allProperties
        foreach (var key in required)
            Assert.True(allProperties.ContainsKey(key),
                $"Required attribute '{key}' is missing from allProperties");

        // Optional attributes = allProperties minus required
        var optional = allProperties.Keys.Except(required).ToList();
        Assert.True(optional.Count > 0, "Package should have optional attributes beyond @id/@type");
    }

    [Fact]
    public void GetTypeAttributeInfo_WithUnknownType_ReturnsEmptyRequiredAndEmptyProperties()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());

        var (required, allProperties) = factory.GetTypeAttributeInfo("DoesNotExist");

        Assert.Empty(required);
        Assert.Empty(allProperties);
    }

    [Fact]
    public void GetTypeAttributeInfo_RequiredIsSubsetOfAllProperties()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());

        // Test with a type that has a rich schema
        var (required, allProperties) = factory.GetTypeAttributeInfo("RequirementUsage");

        Assert.NotEmpty(allProperties);
        foreach (var key in required)
            Assert.True(allProperties.ContainsKey(key),
                $"Required field '{key}' must be present in allProperties");
    }

    [Fact]
    public void GetTypeAttributeInfo_AtTypeIsConstValue()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());

        var (_, allProperties) = factory.GetTypeAttributeInfo("Package");

        Assert.True(allProperties.TryGetValue("@type", out var atTypeValue));
        Assert.True(atTypeValue.StartsWith("const:"),
            "@type property should be described as a const value");
    }
}