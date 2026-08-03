using System.ComponentModel;
using Moq;
using Src.Services;
using SysMLV2.MCP.Models;
using Tools.Structure;

namespace SysMLv2Mcp.Tests;

public class StructureToolsTests
{
    private static (Mock<ElementMutationService> mutation, StructureTools tools) Make()
    {
        var mutation = new Mock<ElementMutationService>(
            MockBehavior.Loose,
            new Mock<ISysMLApiService>().Object,
            new mcp.Src.Services.SysMLMetaModelFactory(GetSchemasPath()),
            new Mock<ProjectContextResolver>(new Mock<ISysMLApiService>().Object).Object);
        return (mutation, new StructureTools(mutation.Object));
    }

    private static string GetSchemasPath() =>
        Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "sysmlv2-api-spec", "metamodels");

    [Theory]
    [InlineData("CreateSignalDefinition", "ItemDefinition")]
    [InlineData("CreateSignal", "ItemUsage")]
    [InlineData("CreateBlockDefinition", "PartDefinition")]
    [InlineData("CreatePart", "PartUsage")]
    [InlineData("CreateInterfaceDefinition", "InterfaceDefinition")]
    [InlineData("CreateInterface", "InterfaceUsage")]
    public void CreateNamed_DelegatesToMutationService(string methodName, string mappedType)
    {
        var (mutation, tools) = Make();
        var expected = new ElementCreationResult { Type = mappedType };
        mutation.Setup(m => m.CreateNamedElementOfType("P", mappedType, "X", null))
               .Returns(expected);
        var method = typeof(StructureTools).GetMethod(methodName)!;
        var result = method.Invoke(tools, new object[] { "P", "X", null });

        Assert.Same(expected, result);
        mutation.Verify(m => m.CreateNamedElementOfType("P", mappedType, "X", null), Times.Once);
    }

    [Theory]
    [InlineData("UpdateSignalDefinition")]
    [InlineData("UpdateSignal")]
    [InlineData("UpdateBlockDefinition")]
    [InlineData("UpdatePart")]
    [InlineData("UpdateInterfaceDefinition")]
    [InlineData("UpdateInterface")]
    public void Update_DelegatesToMutationService(string methodName)
    {
        var (mutation, tools) = Make();
        var id = Guid.NewGuid();
        var expected = new ElementUpdateResult { ElementId = id.ToString() };
        mutation.Setup(m => m.UpdateElementAttributes("P", id, "{}"))
               .Returns(expected);
        var method = typeof(StructureTools).GetMethod(methodName)!;
        var result = method.Invoke(tools, new object[] { "P", id, "{}" });

        Assert.Same(expected, result);
        mutation.Verify(m => m.UpdateElementAttributes("P", id, "{}"), Times.Once);
    }

    [Fact]
    public void StructureTools_IsInstanceClass_WithSingleServiceConstructor()
    {
        var ctor = typeof(StructureTools).GetConstructor(new[] { typeof(ElementMutationService) });
        Assert.NotNull(ctor);
        Assert.False(typeof(StructureTools).IsAbstract && typeof(StructureTools).IsSealed);
    }

    [Theory]
    [InlineData("CreateSignalDefinition")]
    [InlineData("CreateSignal")]
    [InlineData("CreateBlockDefinition")]
    [InlineData("CreatePart")]
    [InlineData("CreateInterfaceDefinition")]
    [InlineData("CreateInterface")]
    [InlineData("UpdateSignalDefinition")]
    [InlineData("UpdateSignal")]
    [InlineData("UpdateBlockDefinition")]
    [InlineData("UpdatePart")]
    [InlineData("UpdateInterfaceDefinition")]
    [InlineData("UpdateInterface")]
    public void StructureTools_MethodsHaveMcpServerToolAttribute(string methodName)
    {
        var method = typeof(StructureTools).GetMethod(methodName);
        Assert.NotNull(method);
        Assert.Contains(method!.GetCustomAttributes(false), a => a.GetType().Name == "McpServerToolAttribute");
        var desc = method.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>().SingleOrDefault();
        Assert.NotNull(desc);
        Assert.False(string.IsNullOrWhiteSpace(desc!.Description));
    }
}