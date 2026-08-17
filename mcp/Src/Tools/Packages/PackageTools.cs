using System.ComponentModel;
using MCP.Src.Services.FactoryServices;
using ModelContextProtocol.Server;
using Src.Services;

namespace Tools.Packages;

[McpServerToolType]
public class PackageTools
{
    private readonly ISysMLApiService _api;
    private readonly SysMLPackageFactory _packageFactory;
    private readonly ProjectContextResolver _projectContext;

    public PackageTools(
        ISysMLApiService api,
        SysMLPackageFactory packageFactory,
        ProjectContextResolver projectContext)
    {
        _api = api;
        _packageFactory = packageFactory;
        _projectContext = projectContext;
    }

    [McpServerTool, Description("Creates a new SysML V2 package inside an optional parent package.")]
    public Guid CreatePackage(string projectName, string packageName, Guid parentPackageGuid = default)
    {
        var project = _projectContext.FindProjectByName(projectName);
        return _packageFactory.CreatePackage(project.Id!.Value, packageName, packageName, parentPackageGuid)
            .GetAwaiter()
            .GetResult();
    }

    [McpServerTool, Description("Creates a top-level SysML V2 package in the specified project.")]
    public Guid CreateTopLevelPackage(string projectName, string packageName)
        => CreatePackage(projectName, packageName, Guid.Empty);
}