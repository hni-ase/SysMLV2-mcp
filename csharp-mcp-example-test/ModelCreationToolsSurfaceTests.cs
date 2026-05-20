using System.ComponentModel;
using mcp.Src.Services;
using SysMLV2.MCP.Models;

namespace csharp_mcp_example_test;

public class ModelCreationToolsSurfaceTests
{
    [Theory]
    [InlineData("CreateSignalDefinition", typeof(ElementCreationResult))]
    [InlineData("CreateSignal", typeof(ElementCreationResult))]
    [InlineData("CreateBlockDefinition", typeof(ElementCreationResult))]
    [InlineData("CreatePart", typeof(ElementCreationResult))]
    [InlineData("CreateInterfaceDefinition", typeof(ElementCreationResult))]
    [InlineData("CreateInterface", typeof(ElementCreationResult))]
    [InlineData("UpdateSignalDefinition", typeof(ElementUpdateResult))]
    [InlineData("UpdateSignal", typeof(ElementUpdateResult))]
    [InlineData("UpdateBlockDefinition", typeof(ElementUpdateResult))]
    [InlineData("UpdatePart", typeof(ElementUpdateResult))]
    [InlineData("UpdateInterfaceDefinition", typeof(ElementUpdateResult))]
    [InlineData("UpdateInterface", typeof(ElementUpdateResult))]
    public void ToolMethod_Exists_WithExpectedReturnType_AndAttributes(string methodName, Type expectedReturnType)
    {
        var method = typeof(ModelCreationTools).GetMethod(methodName);

        Assert.NotNull(method);
        Assert.Equal(expectedReturnType, method!.ReturnType);
        Assert.Contains(method.GetCustomAttributes(false), a => a.GetType().Name == "McpServerToolAttribute");

        var description = method.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>()
            .SingleOrDefault();

        Assert.NotNull(description);
        Assert.False(string.IsNullOrWhiteSpace(description!.Description));
    }

    [Fact]
    public void SignalBlockInterface_MappedMetamodelTypes_AreAvailable()
    {
        var factory = new SysMLMetaModelFactory(GetSysMlSchemasPath());
        var schemas = factory.GetAvailableSchemas().ToHashSet(StringComparer.Ordinal);

        Assert.Contains("ItemDefinition", schemas);
        Assert.Contains("ItemUsage", schemas);
        Assert.Contains("PartDefinition", schemas);
        Assert.Contains("PartUsage", schemas);
        Assert.Contains("InterfaceDefinition", schemas);
        Assert.Contains("InterfaceUsage", schemas);
    }

    private static string GetSysMlSchemasPath() =>
        Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "sysmlv2-api-spec", "metamodels");
}
