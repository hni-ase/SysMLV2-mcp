using System.ComponentModel;
using System.Text.Json;
using Moq;
using Src.Services;
using SysMLV2.MCP.Models;
using Tools.ElementQuery;

namespace SysMLv2Mcp.Tests;

public class ElementQueryToolsTests
{
    private static SysMLProject MakeProject(Guid id, string name, Guid branchId)
        => new()
        {
            Id = id,
            Name = name,
            DefaultBranch = new SysMLRef { Id = branchId }
        };

    private static SysMLElement MakeElement(Guid id, string type, string name)
    {
        var el = new SysMLElement { Id = id, Type = type };
        el.AdditionalProperties = new Dictionary<string, JsonElement>
        {
            ["name"] = JsonSerializer.SerializeToElement(name)
        };
        return el;
    }

    [Fact]
    public void GetElementsFromProjectHead_FiltersByType()
    {
        var api = new Mock<ISysMLApiService>();
        var project = MakeProject(Guid.NewGuid(), "Alpha", Guid.NewGuid());
        api.Setup(a => a.GetProjects()).ReturnsAsync(new List<SysMLProject> { project });
        api.Setup(a => a.GetBranchAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync(new SysMLBranch { Head = new SysMLRef(Guid.NewGuid()) });
        api.Setup(a => a.GetElementsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync(new List<SysMLElement>
           {
               MakeElement(Guid.NewGuid(), "Package", "P1"),
               MakeElement(Guid.NewGuid(), "RequirementUsage", "R1")
           });
        var ctx = new ProjectContextResolver(api.Object);
        var t = new ElementQueryTools(api.Object, ctx);

        var result = t.GetElementsFromProjectHead("Alpha", "Package");

        Assert.Single(result);
        Assert.Equal("Package", result[0].Type);
    }

    [Fact]
    public void GetAllElementsFromProjectHead_ReturnsEverything()
    {
        var api = new Mock<ISysMLApiService>();
        var project = MakeProject(Guid.NewGuid(), "Alpha", Guid.NewGuid());
        api.Setup(a => a.GetProjects()).ReturnsAsync(new List<SysMLProject> { project });
        api.Setup(a => a.GetBranchAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync(new SysMLBranch { Head = new SysMLRef(Guid.NewGuid()) });
        api.Setup(a => a.GetElementsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync(new List<SysMLElement>
           {
               MakeElement(Guid.NewGuid(), "Package", "P1"),
               MakeElement(Guid.NewGuid(), "RequirementUsage", "R1")
           });
        var ctx = new ProjectContextResolver(api.Object);
        var t = new ElementQueryTools(api.Object, ctx);

        var result = t.GetAllElementsFromProjectHead("Alpha");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetElementsFromProjectHead_EmptyWhenNoHeadCommit()
    {
        var api = new Mock<ISysMLApiService>();
        var project = MakeProject(Guid.NewGuid(), "Alpha", Guid.NewGuid());
        api.Setup(a => a.GetProjects()).ReturnsAsync(new List<SysMLProject> { project });
        api.Setup(a => a.GetBranchAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync(new SysMLBranch { Head = null });
        var ctx = new ProjectContextResolver(api.Object);
        var t = new ElementQueryTools(api.Object, ctx);

        var result = t.GetElementsFromProjectHead("Alpha");

        Assert.Empty(result);
    }

    [Fact]
    public void GetPackagesFromProjectHead_FiltersToPackageTypes()
    {
        var api = new Mock<ISysMLApiService>();
        var project = MakeProject(Guid.NewGuid(), "Alpha", Guid.NewGuid());
        api.Setup(a => a.GetProjects()).ReturnsAsync(new List<SysMLProject> { project });
        api.Setup(a => a.GetBranchAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync(new SysMLBranch { Head = new SysMLRef(Guid.NewGuid()) });
        api.Setup(a => a.GetElementsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync(new List<SysMLElement>
           {
               MakeElement(Guid.NewGuid(), "Package", "P1"),
               MakeElement(Guid.NewGuid(), "LibraryPackage", "L1"),
               MakeElement(Guid.NewGuid(), "RequirementUsage", "R1")
           });
        var ctx = new ProjectContextResolver(api.Object);
        var t = new ElementQueryTools(api.Object, ctx);

        var result = t.GetPackagesFromProjectHead("Alpha");

        Assert.Equal(2, result.Count);
        Assert.True(result.All(r => r.Type is "Package" or "LibraryPackage"));
    }

    [Theory]
    [InlineData("GetElementsFromProjectHead")]
    [InlineData("GetAllElementsFromProjectHead")]
    [InlineData("GetElementsByTypeFromProjectHead")]
    [InlineData("GetPackagesFromProjectHead")]
    [InlineData("GetElementByIdFromProjectHead")]
    public void ElementQueryTools_MethodsHaveMcpServerToolAttribute(string methodName)
    {
        var method = typeof(ElementQueryTools).GetMethod(methodName);
        Assert.NotNull(method);
        Assert.Contains(method!.GetCustomAttributes(false), a => a.GetType().Name == "McpServerToolAttribute");
    }

    [Fact]
    public void ElementQueryTools_IsInstanceClass_WithTwoServiceConstructor()
    {
        var ctor = typeof(ElementQueryTools).GetConstructor(new[] { typeof(ISysMLApiService), typeof(ProjectContextResolver) });
        Assert.NotNull(ctor);
        Assert.False(typeof(ElementQueryTools).IsAbstract && typeof(ElementQueryTools).IsSealed);
    }
}