
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Tools;
using Src.Services;

[McpServerToolType]
public class ModelCreationTools
{

    // private static readonly _sysmlApiService = new SysMLApiService();
    private SysMLApiService _sysmlApiService = new SysMLApiService();
    private string _apiKey = "";

    [McpServerTool, Description("Creates a new SysML V2 model in the specified project.")]
    public static string CreateProject(string projectName)
    {
        Debug.WriteLine("Model Creation Tool is handling operation...");
        // Implementation of model creation logic goes here
        return "Model created successfully.";
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

internal class _sysmlApiService
{
}