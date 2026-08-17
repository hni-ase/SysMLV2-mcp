using System.ComponentModel;
using ModelContextProtocol.Server;
using Src.Services;
using SysMLV2.MCP.Models;

namespace Tools.Projects;

[McpServerToolType]
public class ProjectTools
{
    private readonly ISysMLApiService _api;

    public ProjectTools(ISysMLApiService api)
    {
        _api = api;
    }

    [McpServerTool, Description("Creates a new SysML V2 project.")]
    public string CreateProject(string projectName)
    {
        var result = _api.CreateNewProjectAsync(projectName, "Created via MCP Tool")
            .GetAwaiter().GetResult();
        return string.Format("Project '{0}' created successfully with ID: {1}.", projectName, result.Id);
    }

    [McpServerTool, Description("Gets all SysML V2 projects from localhost:9000.")]
    public List<ProjectLookupResult> GetProjects()
    {
        return _api.GetProjects()
            .GetAwaiter()
            .GetResult()
            .Select(ProjectLookupResult.From)
            .ToList();
    }

    [McpServerTool, Description("Gets a SysML V2 project by name from localhost:9000.")]
    public ProjectLookupResult GetProjectByName(string projectName)
    {
        var project = FindProjectByName(projectName);
        return ProjectLookupResult.From(project);
    }

    private SysMLProject FindProjectByName(string projectName)
    {
        var projects = _api.GetProjects().GetAwaiter().GetResult();
        return projects.FirstOrDefault(p => string.Equals(p.Name, projectName, StringComparison.OrdinalIgnoreCase))
               ?? throw new Exception($"Project with name '{projectName}' was not found.");
    }
}