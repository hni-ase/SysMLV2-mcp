using SysMLV2.MCP.Models;

namespace Src.Services;

public class ProjectContextResolver
{
    private readonly ISysMLApiService _api;

    public ProjectContextResolver(ISysMLApiService api)
    {
        _api = api;
    }

    public SysMLProject FindProjectByName(string projectName)
    {
        var projects = _api.GetProjects().GetAwaiter().GetResult();
        return projects.FirstOrDefault(p => string.Equals(p.Name, projectName, StringComparison.OrdinalIgnoreCase))
               ?? throw new Exception($"Project with name '{projectName}' was not found.");
    }

    public (Guid ProjectId, Guid HeadCommitId) GetProjectAndHeadCommitOrDefault(SysMLProject project)
    {
        var projectId = project.Id ?? throw new Exception("Project has no ID.");
        var defaultBranchId = project.DefaultBranch?.Id ?? throw new Exception($"Project '{project.Name}' has no default branch.");
        var defaultBranch = _api.GetBranchAsync(projectId, defaultBranchId).GetAwaiter().GetResult();
        var headCommitId = defaultBranch.Head?.Id ?? Guid.Empty;
        return (projectId, headCommitId);
    }
}