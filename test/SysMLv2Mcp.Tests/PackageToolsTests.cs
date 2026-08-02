using System.ComponentModel;
using MCP.Src.Services.FactoryServices;
using mcp.Src.Services;
using Moq;
using Src.Services;
using SysMLV2.MCP.Models;
using Tools.Packages;

namespace SysMLv2Mcp.Tests;

public class PackageToolsTests
{
    [Fact]
    public void CreatePackage_ResolvesProjectAndCallsFactory()
    {
        var project = new SysMLProject { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Alpha" };
        var api = new Mock<ISysMLApiService>();
        api.Setup(a => a.GetProjects()).ReturnsAsync(new List<SysMLProject> { project });
        var factory = new Mock<SysMLPackageFactory>(MockBehavior.Strict, api.Object, new SysMLMetaModelFactory(GetSchemasPath()));
        var ctx = new ProjectContextResolver(api.Object);
        var tools = new PackageTools(api.Object, factory.Object, ctx);

        factory.Setup(f => f.CreatePackage(project.Id!.Value, "Pkg", "Pkg", Guid.Empty))
               .ReturnsAsync(Guid.Parse("00000000-0000-0000-0000-0000000000AB"));

        var result = tools.CreatePackage("Alpha", "Pkg");

        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-0000000000AB"), result);
        factory.Verify(f => f.CreatePackage(project.Id!.Value, "Pkg", "Pkg", Guid.Empty), Times.Once);
    }

    [Fact]
    public void CreateTopLevelPackage_PassesEmptyParentGuid()
    {
        var project = new SysMLProject { Id = Guid.NewGuid(), Name = "Alpha" };
        var api = new Mock<ISysMLApiService>();
        api.Setup(a => a.GetProjects()).ReturnsAsync(new List<SysMLProject> { project });
        var factory = new Mock<SysMLPackageFactory>(MockBehavior.Loose, api.Object, new SysMLMetaModelFactory(GetSchemasPath()));
        factory.Setup(f => f.CreatePackage(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
               .ReturnsAsync(Guid.NewGuid());
        var ctx = new ProjectContextResolver(api.Object);
        var tools = new PackageTools(api.Object, factory.Object, ctx);

        tools.CreateTopLevelPackage("Alpha", "Pkg");

        factory.Verify(f => f.CreatePackage(project.Id!.Value, "Pkg", "Pkg", Guid.Empty), Times.Once);
    }

    [Theory]
    [InlineData("CreatePackage")]
    [InlineData("CreateTopLevelPackage")]
    public void PackageTools_MethodsHaveMcpServerToolAttribute(string methodName)
    {
        var method = typeof(PackageTools).GetMethod(methodName);
        Assert.NotNull(method);
        Assert.Contains(method!.GetCustomAttributes(false), a => a.GetType().Name == "McpServerToolAttribute");
        var desc = method.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>().SingleOrDefault();
        Assert.NotNull(desc);
        Assert.False(string.IsNullOrWhiteSpace(desc!.Description));
    }

    [Fact]
    public void PackageTools_IsInstanceClass_WithThreeServiceConstructor()
    {
        var ctor = typeof(PackageTools).GetConstructor(new[]
        {
            typeof(ISysMLApiService), typeof(SysMLPackageFactory), typeof(ProjectContextResolver)
        });
        Assert.NotNull(ctor);
        Assert.False(typeof(PackageTools).IsAbstract && typeof(PackageTools).IsSealed);
    }

    private static string GetSchemasPath() =>
        Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "sysmlv2-api-spec", "metamodels");
}