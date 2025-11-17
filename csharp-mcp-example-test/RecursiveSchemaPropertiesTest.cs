using System.Text.Json;
using System.Text.Json.Nodes;
using mcp.Src.Services;
using Xunit;

namespace csharp_mcp_example_test;

public class RecursiveSchemaPropertiesTest
{
    private string GetSysMlSchemasPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "sysmlv2-api-spec", "metamodels");
    }

    [Fact]
    public void GetSchemaPropertiesRecursive_WithElementSchema_ReturnsAllProperties()
    {
        // Arrange
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var elementSchema = factory.GetSchema("Element");

        // Act
        var properties = factory.GetSchemaPropertiesRecursive(elementSchema);

        // Assert
        Assert.NotEmpty(properties);
        
        // Element should have basic properties like @id, @type, name, etc.
        Assert.True(properties.ContainsKey("@id"), "Element schema should contain @id property");
        Assert.True(properties.ContainsKey("@type"), "Element schema should contain @type property");
        Assert.True(properties.ContainsKey("name"), "Element schema should contain name property");
        Assert.True(properties.ContainsKey("elementId"), "Element schema should contain elementId property");
        
        // Check some expected property types
        Assert.Equal("string", properties["@id"]);
        Assert.Equal("const:Element", properties["@type"]);
    }

    [Fact]
    public void GetSchemaPropertiesRecursive_WithClassSchema_ReturnsInheritedProperties()
    {
        // Arrange
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var classSchema = factory.GetSchema("Class");

        // Act
        var properties = factory.GetSchemaPropertiesRecursive(classSchema);

        // Assert
        Assert.NotEmpty(properties);
        
        // Class inherits from Classifier, which inherits from Type, which inherits from Namespace, which inherits from Element
        // So it should have properties from all these types
        Assert.True(properties.ContainsKey("@id"), "Class should inherit @id from Element");
        Assert.True(properties.ContainsKey("name"), "Class should inherit name from Element");
        Assert.True(properties.ContainsKey("isAbstract"), "Class should inherit isAbstract from Type");
        
        Console.WriteLine($"Class schema has {properties.Count} total properties");
        
        // Class should have significantly more properties than just direct ones due to inheritance
        Assert.True(properties.Count > 10, $"Class should have many properties due to inheritance, but found only {properties.Count}");
    }

    [Fact]
    public void GetSchemaProperties_WithPackageSchema_ReturnsCorrectProperties()
    {
        // Arrange
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());

        // Act
        var properties = factory.GetSchemaProperties("Package");

        // Assert
        Assert.NotEmpty(properties);
        
        // Package inherits from Namespace which inherits from Element
        Assert.True(properties.ContainsKey("@id"), "Package should inherit @id from Element");
        Assert.True(properties.ContainsKey("@type"), "Package should have @type property");
        Assert.True(properties.ContainsKey("name"), "Package should inherit name from Element");
        
        Console.WriteLine($"Package schema has {properties.Count} total properties");
        
        // Log some properties for debugging
        foreach (var prop in properties.Take(5))
        {
            Console.WriteLine($"  {prop.Key}: {prop.Value}");
        }
    }

    [Fact]
    public void GetSchemaPropertiesRecursive_WithNullSchema_ReturnsEmptyDictionary()
    {
        // Arrange
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());

        // Act
        var properties = factory.GetSchemaPropertiesRecursive(null);

        // Assert
        Assert.Empty(properties);
    }

    [Fact]
    public void GetSchemaPropertiesRecursive_DetectsArrayAndReferenceTypes()
    {
        // Arrange
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var elementSchema = factory.GetSchema("Element");

        // Act
        var properties = factory.GetSchemaPropertiesRecursive(elementSchema);

        // Assert
        // aliasIds should be detected as an array of strings
        if (properties.ContainsKey("aliasIds"))
        {
            Assert.Contains("array", properties["aliasIds"]);
        }
        
        // Properties with references should be detected
        var referenceProperties = properties.Where(p => p.Value.StartsWith("ref:")).ToList();
        Assert.NotEmpty(referenceProperties);
        
        Console.WriteLine($"Found {referenceProperties.Count} reference properties:");
        foreach (var refProp in referenceProperties.Take(3))
        {
            Console.WriteLine($"  {refProp.Key}: {refProp.Value}");
        }
    }
}