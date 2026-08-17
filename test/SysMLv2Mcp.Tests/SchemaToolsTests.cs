using System.ComponentModel;
using mcp.Src.Services;
using Moq;
using Src.Services;
using SysMLV2.MCP.Models;
using Tools.Schema;

namespace SysMLv2Mcp.Tests;

public class SchemaToolsTests
{
    private static string GetSchemasPath() =>
        Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "sysmlv2-api-spec", "metamodels");

    private static SchemaTools MakeTools(out Mock<ISysMLApiService> apiMock)
    {
        apiMock = new Mock<ISysMLApiService>();
        var metamodel = new SysMLMetaModelFactory(GetSchemasPath());
        var projectContext = new ProjectContextResolver(apiMock.Object);
        return new SchemaTools(apiMock.Object, metamodel, projectContext);
    }

    [Fact]
    public void DescribeTypeSchema_ReturnsRequiredAndOptional_ForKnownType()
    {
        var tools = MakeTools(out _);

        var info = tools.DescribeTypeSchema("ItemDefinition");

        Assert.Equal("ItemDefinition", info.Type);
        Assert.NotEmpty(info.RequiredAttributes);
        Assert.NotEmpty(info.OptionalAttributes);
    }

    [Fact]
    public void DescribeTypeSchema_ThrowsForUnknownType_ListsAvailable()
    {
        var tools = MakeTools(out _);

        var ex = Assert.Throws<ArgumentException>(() => tools.DescribeTypeSchema("NotAType"));
        Assert.Contains("Available types:", ex.Message);
    }

    [Fact]
    public void DescribeElementSchema_ThrowsWhenProjectHasNoCommits()
    {
        var tools = MakeTools(out var apiMock);
        var project = new SysMLProject { Id = Guid.NewGuid(), Name = "P", DefaultBranch = new SysMLRef { Id = Guid.NewGuid() } };
        apiMock.Setup(a => a.GetProjects()).ReturnsAsync(new List<SysMLProject> { project });
        var branch = new SysMLBranch { Head = null };
        apiMock.Setup(a => a.GetBranchAsync(project.Id!.Value, project.DefaultBranch!.Id)).ReturnsAsync(branch);

        Assert.Throws<Exception>(() => tools.DescribeElementSchema("P", Guid.NewGuid()));
    }

    [Theory]
    [InlineData("DescribeTypeSchema")]
    [InlineData("DescribeElementSchema")]
    public void SchemaTools_MethodsHaveMcpServerToolAttribute(string methodName)
    {
        var method = typeof(SchemaTools).GetMethod(methodName);
        Assert.NotNull(method);
        Assert.Contains(method!.GetCustomAttributes(false), a => a.GetType().Name == "McpServerToolAttribute");
        var desc = method.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>().SingleOrDefault();
        Assert.NotNull(desc);
        Assert.False(string.IsNullOrWhiteSpace(desc!.Description));
    }

    [Fact]
    public void SchemaTools_IsInstanceClass_WithThreeServiceConstructor()
    {
        var ctor = typeof(SchemaTools).GetConstructor(new[]
        {
            typeof(ISysMLApiService), typeof(SysMLMetaModelFactory), typeof(ProjectContextResolver)
        });
        Assert.NotNull(ctor);
        Assert.False(typeof(SchemaTools).IsAbstract && typeof(SchemaTools).IsSealed);
    }
}