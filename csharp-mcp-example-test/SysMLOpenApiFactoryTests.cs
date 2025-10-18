using System.Text.Json;
using System.Text.Json.Nodes;
using mcp.Src.Services;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Xunit;

namespace csharp_mcp_example_test;
public class SysMLOpenApiFactoryTests
{
    private readonly string _testSchemasPath;

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
            ["documentation"] =  new List<string> { "A simple use case definition" },
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

    // private void CreateTestSchemas()
    // {
    //     // Create a simple test schema
    //     var testElementSchema = new JsonObject
    //     {
    //         ["type"] = "object",
    //         ["properties"] = new JsonObject
    //         {
    //             ["elementId"] = new JsonObject { ["type"] = "string" },
    //             ["name"] = new JsonObject { ["type"] = "string" },
    //             ["isAbstract"] = new JsonObject { ["type"] = "boolean" },
    //             ["count"] = new JsonObject { ["type"] = "integer" },
    //             ["ratio"] = new JsonObject { ["type"] = "number" },
    //             ["tags"] = new JsonObject
    //             {
    //                 ["type"] = "array",
    //                 ["items"] = new JsonObject { ["type"] = "string" }
    //             },
    //             ["metadata"] = new JsonObject
    //             {
    //                 ["type"] = "object",
    //                 ["properties"] = new JsonObject
    //                 {
    //                     ["version"] = new JsonObject { ["type"] = "string" },
    //                     ["author"] = new JsonObject { ["type"] = "string" }
    //                 }
    //             }
    //         }
    //     };

    //     File.WriteAllText(
    //         Path.Combine(_testSchemasPath, "TestElement.json"),
    //         testElementSchema.ToJsonString()
    //     );

        // Create another test schema
    //     var simpleSchema = new JsonObject
    //     {
    //         ["type"] = "object",
    //         ["properties"] = new JsonObject
    //         {
    //             ["id"] = new JsonObject { ["type"] = "string" },
    //             ["value"] = new JsonObject { ["type"] = "integer" }
    //         }
    //     };

    //     File.WriteAllText(
    //         Path.Combine(_testSchemasPath, "SimpleElement.json"),
    //         simpleSchema.ToJsonString()
    //     );

    //     // Create an empty schema
    //     var emptySchema = new JsonObject
    //     {
    //         ["type"] = "object"
    //     };

    //     File.WriteAllText(
    //         Path.Combine(_testSchemasPath, "EmptyElement.json"),
    //         emptySchema.ToJsonString()
    //     );

    //     // Create an invalid JSON file for error testing
    //     File.WriteAllText(
    //         Path.Combine(_testSchemasPath, "InvalidSchema.json"),
    //         "{ invalid json"
    //     );
    // }

    // [Fact]
    // public void Constructor_WithValidPath_LoadsSchemas()
    // {
    //     // Act & Assert
    //     var schemas = _factory.GetAvailableSchemas().ToList();

    //     Assert.Contains("TestElement", schemas);
    //     Assert.Contains("SimpleElement", schemas);
    //     Assert.Contains("EmptyElement", schemas);
    //     // InvalidSchema should not be loaded due to parsing error
    //     Assert.DoesNotContain("InvalidSchema", schemas);
    // }

    // [Fact]
    // public void Constructor_WithInvalidPath_ThrowsDirectoryNotFoundException()
    // {
    //     // Arrange
    //     var invalidPath = "/non/existent/path";

    //     // Act & Assert
    //     Assert.Throws<DirectoryNotFoundException>(() => new SysMLOpenApiFactory(invalidPath));
    // }

    // [Fact]
    // public void Constructor_WithNullPath_UsesDefaultPath()
    // {
    //     // Act
    //     var factory = new SysMLOpenApiFactory(null);

    //     // Assert - should not throw if default path exists, or throw DirectoryNotFoundException if it doesn't
    //     var exception = Record.Exception(() => factory.GetAvailableSchemas().ToList());
    //     Assert.True(exception == null || exception is DirectoryNotFoundException);
    // }

    // [Fact]
    // public void CreateJsonRequestBody_WithValidParameters_ReturnsCorrectJson()
    // {
    //     // Arrange
    //     var parameters = new Dictionary<string, object?>
    //     {
    //         ["elementId"] = "test-001",
    //         ["name"] = "Test Element",
    //         ["isAbstract"] = true,
    //         ["count"] = 42
    //     };

    //     // Act
    //     var result = _factory.CreateJsonRequestBody("TestElement", parameters);

    //     // Assert
    //     var jsonDoc = JsonDocument.Parse(result);
    //     var root = jsonDoc.RootElement;

    //     Assert.Equal("test-001", root.GetProperty("elementId").GetString());
    //     Assert.Equal("Test Element", root.GetProperty("name").GetString());
    //     Assert.True(root.GetProperty("isAbstract").GetBoolean());
    //     Assert.Equal(42, root.GetProperty("count").GetInt32());

    //     // Properties not provided should not be in output
    //     Assert.False(root.TryGetProperty("ratio", out _));
    //     Assert.False(root.TryGetProperty("tags", out _));
    // }

    // [Fact]
    // public void CreateJsonRequestBody_WithTupleParameters_ReturnsCorrectJson()
    // {
    //     // Act
    //     var result = _factory.CreateJsonRequestBody("SimpleElement",
    //         ("id", "simple-001"),
    //         ("value", 123)
    //     );

    //     // Assert
    //     var jsonDoc = JsonDocument.Parse(result);
    //     var root = jsonDoc.RootElement;

    //     Assert.Equal("simple-001", root.GetProperty("id").GetString());
    //     Assert.Equal(123, root.GetProperty("value").GetInt32());
    // }

    // [Fact]
    // public void CreateJsonRequestBody_WithArrayParameter_ReturnsCorrectJson()
    // {
    //     // Arrange
    //     var parameters = new Dictionary<string, object?>
    //     {
    //         ["elementId"] = "array-test",
    //         ["tags"] = new[] { "tag1", "tag2", "tag3" }
    //     };

    //     // Act
    //     var result = _factory.CreateJsonRequestBody("TestElement", parameters);

    //     // Assert
    //     var jsonDoc = JsonDocument.Parse(result);
    //     var root = jsonDoc.RootElement;

    //     Assert.Equal("array-test", root.GetProperty("elementId").GetString());

    //     var tagsArray = root.GetProperty("tags");
    //     Assert.Equal(3, tagsArray.GetArrayLength());
    //     Assert.Equal("tag1", tagsArray[0].GetString());
    //     Assert.Equal("tag2", tagsArray[1].GetString());
    //     Assert.Equal("tag3", tagsArray[2].GetString());
    // }

    // [Fact]
    // public void CreateJsonRequestBody_WithObjectParameter_ReturnsCorrectJson()
    // {
    //     // Arrange
    //     var metadata = new Dictionary<string, object?>
    //     {
    //         ["version"] = "1.0",
    //         ["author"] = "Test Author"
    //     };

    //     var parameters = new Dictionary<string, object?>
    //     {
    //         ["elementId"] = "object-test",
    //         ["metadata"] = metadata
    //     };

    //     // Act
    //     var result = _factory.CreateJsonRequestBody("TestElement", parameters);

    //     // Assert
    //     var jsonDoc = JsonDocument.Parse(result);
    //     var root = jsonDoc.RootElement;

    //     Assert.Equal("object-test", root.GetProperty("elementId").GetString());

    //     var metadataObj = root.GetProperty("metadata");
    //     Assert.Equal("1.0", metadataObj.GetProperty("version").GetString());
    //     Assert.Equal("Test Author", metadataObj.GetProperty("author").GetString());
    // }

    // [Fact]
    // public void CreateJsonRequestBody_WithNullValues_ExcludesNullProperties()
    // {
    //     // Arrange
    //     var parameters = new Dictionary<string, object?>
    //     {
    //         ["elementId"] = "null-test",
    //         ["name"] = null,
    //         ["isAbstract"] = true
    //     };

    //     // Act
    //     var result = _factory.CreateJsonRequestBody("TestElement", parameters);

    //     // Assert
    //     var jsonDoc = JsonDocument.Parse(result);
    //     var root = jsonDoc.RootElement;

    //     Assert.Equal("null-test", root.GetProperty("elementId").GetString());
    //     Assert.True(root.GetProperty("isAbstract").GetBoolean());

    //     // Null properties should not appear in output
    //     Assert.False(root.TryGetProperty("name", out _));
    // }

    // [Fact]
    // public void CreateJsonRequestBody_WithUnknownElement_ThrowsArgumentException()
    // {
    //     // Arrange
    //     var parameters = new Dictionary<string, object?> { ["id"] = "test" };

    //     // Act & Assert
    //     var exception = Assert.Throws<ArgumentException>(() =>
    //         _factory.CreateJsonRequestBody("NonExistentElement", parameters));

    //     Assert.Contains("Schema for element 'NonExistentElement' not found", exception.Message);
    //     Assert.Contains("Available schemas:", exception.Message);
    // }

    // [Fact]
    // public void CreateJsonRequestBody_WithEmptySchema_ReturnsEmptyJson()
    // {
    //     // Arrange
    //     var parameters = new Dictionary<string, object?> { ["someProperty"] = "value" };

    //     // Act
    //     var result = _factory.CreateJsonRequestBody("EmptyElement", parameters);

    //     // Assert
    //     Assert.Equal("{}", result);
    // }

    // [Fact]
    // public void CreateJsonRequestBody_WithEmptyParameters_ReturnsEmptyJson()
    // {
    //     // Arrange
    //     var parameters = new Dictionary<string, object?>();

    //     // Act
    //     var result = _factory.CreateJsonRequestBody("TestElement", parameters);

    //     // Assert
    //     Assert.Equal("{}", result);
    // }

    // [Theory]
    // [InlineData("string", "test", "\"test\"")]
    // [InlineData("integer", 42, "42")]
    // [InlineData("integer", "42", "42")] // String that can be converted to int
    // [InlineData("number", 3.14, "3.14")]
    // [InlineData("number", "3.14", "3.14")] // String that can be converted to double
    // [InlineData("boolean", true, "true")]
    // [InlineData("boolean", "true", "true")] // String that can be converted to bool
    // public void CreateJsonRequestBody_WithDifferentTypes_ConvertsCorrectly(string schemaType, object value, string expectedJson)
    // {
    //     // Arrange - Create a dynamic schema for this test
    //     var testSchema = new JsonObject
    //     {
    //         ["type"] = "object",
    //         ["properties"] = new JsonObject
    //         {
    //             ["testProperty"] = new JsonObject { ["type"] = schemaType }
    //         }
    //     };

    //     var dynamicSchemaPath = Path.Combine(_testSchemasPath, "DynamicTest.json");
    //     File.WriteAllText(dynamicSchemaPath, testSchema.ToJsonString());

    //     // Reload schemas to include the new one
    //     var factory = new SysMLOpenApiFactory(_testSchemasPath);

    //     var parameters = new Dictionary<string, object?> { ["testProperty"] = value };

    //     // Act
    //     var result = factory.CreateJsonRequestBody("DynamicTest", parameters);

    //     // Assert
    //     var jsonDoc = JsonDocument.Parse(result);
    //     var root = jsonDoc.RootElement;
    //     var actualValue = root.GetProperty("testProperty");

    //     Assert.Equal(expectedJson.Trim('"'), actualValue.ToString());
    // }

    // [Fact]
    // public void GetAvailableSchemas_ReturnsAllLoadedSchemas()
    // {
    //     // Act
    //     var schemas = _factory.GetAvailableSchemas().ToList();

    //     // Assert
    //     Assert.Contains("TestElement", schemas);
    //     Assert.Contains("SimpleElement", schemas);
    //     Assert.Contains("EmptyElement", schemas);
    //     Assert.Equal(3, schemas.Count); // Only valid schemas should be loaded
    // }

    // [Fact]
    // public void GetSchema_WithValidElement_ReturnsSchema()
    // {
    //     // Act
    //     var schema = _factory.GetSchema("TestElement");

    //     // Assert
    //     Assert.NotNull(schema);
    //     Assert.Equal("object", schema["type"]?.GetValue<string>());
    //     Assert.NotNull(schema["properties"]);
    // }

    // [Fact]
    // public void GetSchema_WithInvalidElement_ReturnsNull()
    // {
    //     // Act
    //     var schema = _factory.GetSchema("NonExistentElement");

    //     // Assert
    //     Assert.Null(schema);
    // }

    // [Fact]
    // public void GetSchemaProperties_WithValidElement_ReturnsProperties()
    // {
    //     // Act
    //     var properties = _factory.GetSchemaProperties("TestElement");

    //     // Assert
    //     Assert.Equal("string", properties["elementId"]);
    //     Assert.Equal("string", properties["name"]);
    //     Assert.Equal("boolean", properties["isAbstract"]);
    //     Assert.Equal("integer", properties["count"]);
    //     Assert.Equal("number", properties["ratio"]);
    //     Assert.Equal("array", properties["tags"]);
    //     Assert.Equal("object", properties["metadata"]);
    // }

    // [Fact]
    // public void GetSchemaProperties_WithInvalidElement_ReturnsEmptyDictionary()
    // {
    //     // Act
    //     var properties = _factory.GetSchemaProperties("NonExistentElement");

    //     // Assert
    //     Assert.Empty(properties);
    // }

    // [Fact]
    // public void GetSchemaProperties_WithEmptySchema_ReturnsEmptyDictionary()
    // {
    //     // Act
    //     var properties = _factory.GetSchemaProperties("EmptyElement");

    //     // Assert
    //     Assert.Empty(properties);
    // }
}