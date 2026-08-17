using System.ComponentModel;
using mcp.Src.Services;
using Moq;
using Src.Services;
using SysMLV2.MCP.Models;
using Tools.Projects;

namespace SysMLv2Mcp.Tests;

public class ProjectToolsTests
{
    private static SysMLProject MakeProject(Guid id, string name, Guid defaultBranchId = default, string description = "")
        => new()
        {
            Id = id,
            Name = name,
            DefaultBranch = defaultBranchId == Guid.Empty ? null : new SysMLRef { Id = defaultBranchId },
            Description = description
        };

    [Fact]
    public void GetProjects_MapsAllProjects_ToLookupResults()
    {
        var projects = new List<SysMLProject>
        {
            MakeProject(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Alpha", Guid.Parse("00000000-0000-0000-0000-0000000000AA"), "alpha desc"),
            MakeProject(Guid.Parse("00000000-0000-0000-0000-000000000002"), "Beta")
        };
        var api = new Mock<ISysMLApiService>();
        api.Setup(a => a.GetProjects()).ReturnsAsync(projects);

        var tools = new ProjectTools(api.Object);

        var result = tools.GetProjects();

        Assert.Equal(2, result.Count);
        Assert.Equal("Alpha", result[0].Name);
        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-0000000000AA"), result[0].DefaultBranchId);
        Assert.Equal("alpha desc", result[0].Description);
        Assert.Equal("Beta", result[1].Name);
        Assert.Equal(Guid.Empty, result[1].DefaultBranchId);
    }

    [Fact]
    public void GetProjectByName_ReturnsMatch_CaseInsensitive()
    {
        var projects = new List<SysMLProject>
        {
            MakeProject(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Alpha")
        };
        var api = new Mock<ISysMLApiService>();
        api.Setup(a => a.GetProjects()).ReturnsAsync(projects);

        var tools = new ProjectTools(api.Object);

        var result = tools.GetProjectByName("alpha");

        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000001"), result.Id);
        Assert.Equal("Alpha", result.Name);
    }

    [Fact]
    public void GetProjectByName_ThrowsWhenNotFound()
    {
        var api = new Mock<ISysMLApiService>();
        api.Setup(a => a.GetProjects()).ReturnsAsync(new List<SysMLProject>());

        var tools = new ProjectTools(api.Object);

        Assert.Throws<Exception>(() => tools.GetProjectByName("missing"));
    }

    [Fact]
    public void CreateProject_CallsApiAndFormatsResult()
    {
        var created = MakeProject(Guid.Parse("00000000-0000-0000-0000-0000000000AB"), "NewProj");
        var api = new Mock<ISysMLApiService>();
        api.Setup(a => a.CreateNewProjectAsync("NewProj", "Created via MCP Tool"))
           .ReturnsAsync(created);

        var tools = new ProjectTools(api.Object);

        var result = tools.CreateProject("NewProj");

        Assert.Contains("NewProj", result);
        Assert.Contains("00000000-0000-0000-0000-0000000000ab", result);
        api.Verify(a => a.CreateNewProjectAsync("NewProj", "Created via MCP Tool"), Times.Once);
    }

    [Fact]
    public void ProjectLookupResult_From_PreservesFields_AndHandlesNulls()
    {
        var withNulls = new SysMLProject { Id = null, Name = null, DefaultBranch = null, Description = null };
        var result = ProjectLookupResult.From(withNulls);
        Assert.Equal(Guid.Empty, result.Id);
        Assert.Equal(string.Empty, result.Name);
        Assert.Equal(Guid.Empty, result.DefaultBranchId);
        Assert.Equal(string.Empty, result.Description);
    }

    [Theory]
    [InlineData("CreateProject")]
    [InlineData("GetProjects")]
    [InlineData("GetProjectByName")]
    public void ProjectTools_MethodExists_WithMcpServerToolAttribute(string methodName)
    {
        var method = typeof(ProjectTools).GetMethod(methodName);
        Assert.NotNull(method);
        Assert.Contains(method!.GetCustomAttributes(false), a => a.GetType().Name == "McpServerToolAttribute");

        var description = method.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>()
            .SingleOrDefault();
        Assert.NotNull(description);
        Assert.False(string.IsNullOrWhiteSpace(description!.Description));
    }

    [Fact]
    public void ProjectTools_IsInstanceClass_NotStatic_AndHasMcpServerToolTypeAttribute()
    {
        var type = typeof(ProjectTools);
        Assert.False(type.IsAbstract && type.IsSealed);
        Assert.Contains(type.GetCustomAttributes(false), a => a.GetType().Name == "McpServerToolTypeAttribute");
        var ctor = type.GetConstructor(new[] { typeof(ISysMLApiService) });
        Assert.NotNull(ctor);
    }
}