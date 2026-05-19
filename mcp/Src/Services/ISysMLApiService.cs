using SysMLV2.MCP.Models;

namespace Src.Services;

public interface ISysMLApiService
{
    Task<SysMLProject> CreateNewProjectAsync(string projectName, string projectDescription);

    Task<SysMLProject> GetProjectAsync(Guid projectId);

    Task<List<SysMLProject>> GetProjects();

    Task<List<SysMLBranch>> GetBranchesAsync(Guid projectId);

    Task<SysMLBranch> CreateNewBranchAsync(Guid projectId, string branchName);

    Task<SysMLBranch> GetBranchAsync(Guid projectId, Guid branchId);

    Task<SysMLCommit> GetCommitAsync(Guid projectId, Guid commitId);

    Task<SysMLElement> CreateElementAsync(Guid projectId, Guid branchId, SysMLElement element);

    Task<SysMLElement> GetElementByIdAsync(Guid projectId, Guid commitId, Guid elementId);

    Task<List<SysMLCommit>> GetCommits(Guid projectId, Guid branchId);

    Task<Guid> CommitToBranchAsync(Guid projectId, Guid branchId, CommitRequest commit);

    Task<List<SysMLElement>> GetElementsAsync(Guid projectId, Guid commitId);
}