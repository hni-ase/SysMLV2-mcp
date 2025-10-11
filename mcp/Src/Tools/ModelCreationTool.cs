
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Tools;
using Src.Services;
using mcp.Src.Services;
using System.Runtime.CompilerServices;
using ModelContextProtocol.Protocol;

[McpServerToolType]
public class ModelCreationTools
{

    ISysMLApiService _sysMLApiService;

    public ModelCreationTools(ISysMLApiService sysMLApiService)
    {
        Debug.WriteLine("Model Creation Tool Initialized");
        _sysMLApiService = sysMLApiService;
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

}
