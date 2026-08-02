namespace SysMLV2.MCP.Models;

public class ProjectLookupResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid DefaultBranchId { get; set; }
    public string Description { get; set; } = string.Empty;

    public static ProjectLookupResult From(SysMLProject project) => new()
    {
        Id = project.Id ?? Guid.Empty,
        Name = project.Name ?? string.Empty,
        DefaultBranchId = project.DefaultBranch?.Id ?? Guid.Empty,
        Description = project.Description ?? string.Empty
    };
}