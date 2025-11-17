
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Tools;
using Src.Services;
using mcp.Src.Services;
using System.Runtime.CompilerServices;
using ModelContextProtocol.Protocol;
using MCP.Src.Services.FactoryServices;

[McpServerToolType]
public class ModelCreationTools 
{

    public ModelCreationTools()
    {
        Debug.WriteLine("Model Creation Tool Initialized");
    }


    [McpServerTool, Description("Creates a new SysML V2 model in the specified project.")]
    public static string CreateProject(McpServer server, string projectName)
    {
        Debug.WriteLine("Model Creation Tool is handling operation...");
        // Implementation of model creation logic goes here
        // Now we need to inject the SysMLApiService and call its method to create a project
        var sysMLApiService = server.Services?.GetService<ISysMLApiService>();
        var result = sysMLApiService?.CreateNewProjectAsync(projectName, "Created via MCP Tool").GetAwaiter().GetResult();
        return string.Format("Project '{0}' created successfully with ID: {1}.", projectName, result);
    }

    [McpServerTool, Description("Creates a new SysML V2 model in the specified project.")]
    public static string CreateElement(string elementName)
    {
        Debug.WriteLine("Element Creation Tool is handling operation...");
        // Implementation of model creation logic goes here
        return string.Format("Element '{0}' created successfully.", elementName);
    }



    [McpServerTool, Description("Creates a new relationship between two elements in the specified project.")]
    public static string CreateRelationship(string sourceElement, string targetElement)
    {
        throw new NotImplementedException();
        // return string.Format("Element '{0}' created successfully.", elementName);
    }

    [McpServerTool, Description("Creates a new relationship between two elements in the specified project.")]
    public static Guid CreatePackage(McpServer server, string projectName, string packageName, Guid parentPackageGuid)
    {
        Debug.WriteLine("Package creation tool is handling operation...");
        var apiService = server.Services?.GetService<ISysMLApiService>();
        var metamodelFactory = server.Services?.GetService<SysMLMetaModelFactory>();
        var packageFactory = new SysMLPackageFactory(apiService, metamodelFactory);
        // 1. Check if the project exists, if not, return an Error
        var projects = apiService.GetProjects().GetAwaiter().GetResult();
        // if the package doesn't exist we need to get it. 
        var foundProject = projects.FirstOrDefault(p => p.Name == projectName) ?? throw new Exception($"project with name {projectName} not found!");
        var packageGuid = packageFactory.CreatePackage((Guid)foundProject.Id, packageName, packageName, parentPackageGuid);

        

        // if t

        return Guid.Empty;

    }

    [McpServerTool, Description("Creates a new use case in the specified project.")]
    public static UseCaseLLMInformation CreateUseCase(string name, string documentation, string precondition)
    {
        throw new NotImplementedException();
        // return string.Format("Element '{0}' created successfully.", elementName);
    }

    [McpServerTool, Description("Creates a new actor in the specified project.")]
    public static string CreateActor(string name, string documentation)
    {
        throw new NotImplementedException();
        // return string.Format("Element '{0}' created successfully.", elementName);
    }

}
