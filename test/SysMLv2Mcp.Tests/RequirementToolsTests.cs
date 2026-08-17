using System.ComponentModel;
using MCP.Src.Services.FactoryServices;
using Moq;
using Src.Services;
using SysMLV2.MCP.Models;
using Tools.Requirements;

namespace SysMLv2Mcp.Tests;

public class RequirementToolsTests
{
    private static (Mock<ISysMLApiService> api, Mock<SysMLRequirementFactory> reqFactory, Mock<SysMLUseCaseFactory> ucFactory, RequirementTools tools) Make()
    {
        var project = new SysMLProject { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Alpha" };
        var api = new Mock<ISysMLApiService>();
        api.Setup(a => a.GetProjects()).ReturnsAsync(new List<SysMLProject> { project });
        var reqFactory = new Mock<SysMLRequirementFactory>(MockBehavior.Loose, api.Object);
        var ucFactory = new Mock<SysMLUseCaseFactory>(MockBehavior.Loose, api.Object);
        var ctx = new ProjectContextResolver(api.Object);
        return (api, reqFactory, ucFactory, new RequirementTools(api.Object, reqFactory.Object, ucFactory.Object, ctx));
    }

    [Fact]
    public void CreateUseCase_ResolvesProjectAndCallsFactory()
    {
        var (_, _, ucFactory, tools) = Make();
        var elementId = Guid.Parse("00000000-0000-0000-0000-0000000000EE");
        var projectId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        ucFactory.Setup(f => f.CreateUseCase(projectId, "UC1", Guid.Empty, Guid.Empty))
                 .ReturnsAsync((elementId, projectId));

        var result = tools.CreateUseCase("Alpha", "UC1");

        Assert.Equal(elementId, result.useCaseUuid);
        Assert.Equal(projectId, result.projectUuid);
        Assert.Equal("UC1", result.useCaseName);
        ucFactory.Verify(f => f.CreateUseCase(projectId, "UC1", Guid.Empty, Guid.Empty), Times.Once);
    }

    [Fact]
    public void CreateRequirement_ResolvesProjectAndCallsFactory()
    {
        var (_, reqFactory, _, tools) = Make();
        var expected = Guid.Parse("00000000-0000-0000-0000-0000000000AB");
        reqFactory.Setup(f => f.CreateRequirement(
                Guid.Parse("00000000-0000-0000-0000-000000000001"), "R1", "text", "REQ-1", Guid.Empty))
               .ReturnsAsync(expected);

        var result = tools.CreateRequirement("Alpha", "R1", "text", "REQ-1");

        Assert.Equal(expected, result);
        reqFactory.Verify(f => f.CreateRequirement(
            Guid.Parse("00000000-0000-0000-0000-000000000001"), "R1", "text", "REQ-1", Guid.Empty), Times.Once);
    }

    [Fact]
    public void CreateRequirementDefinition_PassesIsAbstractAndParent()
    {
        var (_, reqFactory, _, tools) = Make();
        var parent = Guid.Parse("00000000-0000-0000-0000-0000000000AA");
        var expected = Guid.Parse("00000000-0000-0000-0000-0000000000BB");
        reqFactory.Setup(f => f.CreateRequirementDefinition(
                It.IsAny<Guid>(), "D1", "text", null, true, parent))
               .ReturnsAsync(expected);

        var result = tools.CreateRequirementDefinition("Alpha", "D1", "text", null, true, parent);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void AddSubjectToRequirement_CallsFactory()
    {
        var (_, reqFactory, _, tools) = Make();
        var reqId = Guid.Parse("00000000-0000-0000-0000-0000000000CC");
        var expected = Guid.Parse("00000000-0000-0000-0000-0000000000DD");
        reqFactory.Setup(f => f.AddSubjectToRequirement(It.IsAny<Guid>(), reqId, "Subject"))
               .ReturnsAsync(expected);

        var result = tools.AddSubjectToRequirement("Alpha", reqId, "Subject");

        Assert.Equal(expected, result);
        reqFactory.Verify(f => f.AddSubjectToRequirement(
            Guid.Parse("00000000-0000-0000-0000-000000000001"), reqId, "Subject"), Times.Once);
    }

    [Fact]
    public void SetRequirementDefinition_CallsFactory()
    {
        var (_, reqFactory, _, tools) = Make();
        var usageId = Guid.Parse("00000000-0000-0000-0000-0000000000C0");
        var defId = Guid.Parse("00000000-0000-0000-0000-0000000000C1");

        tools.SetRequirementDefinition("Alpha", usageId, defId);

        reqFactory.Verify(f => f.SetRequirementDefinition(
            Guid.Parse("00000000-0000-0000-0000-000000000001"), usageId, defId), Times.Once);
    }

    [Theory]
    [InlineData("CreateUseCase")]
    [InlineData("CreateRequirement")]
    [InlineData("CreateRequirementDefinition")]
    [InlineData("AddSubjectToRequirement")]
    [InlineData("SetRequirementDefinition")]
    public void RequirementTools_MethodsHaveMcpServerToolAttribute(string methodName)
    {
        var method = typeof(RequirementTools).GetMethod(methodName);
        Assert.NotNull(method);
        Assert.Contains(method!.GetCustomAttributes(false), a => a.GetType().Name == "McpServerToolAttribute");
        var desc = method.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>().SingleOrDefault();
        Assert.NotNull(desc);
        Assert.False(string.IsNullOrWhiteSpace(desc!.Description));
    }

    [Fact]
    public void RequirementTools_IsInstanceClass_WithFourServiceConstructor()
    {
        var ctor = typeof(RequirementTools).GetConstructor(new[]
        {
            typeof(ISysMLApiService),
            typeof(SysMLRequirementFactory),
            typeof(SysMLUseCaseFactory),
            typeof(ProjectContextResolver)
        });
        Assert.NotNull(ctor);
        Assert.False(typeof(RequirementTools).IsAbstract && typeof(RequirementTools).IsSealed);
    }
}