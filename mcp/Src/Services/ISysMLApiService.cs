using Org.OpenAPITools.Model;

namespace Src.Services;

public interface ISysMLApiService
{
    Task<Project> CreateNewProjectAsync(string projectName, string projectDescription);

    Task<Project> GetProjectAsync(Guid projectId);

    Task<List<Project>> GetProjects();

    Task<Branch> CreateNewBranchAsync(Guid projectId, string branchName);

    Task<Branch> GetBranchAsync(Guid projectId, Guid branchId);

    Task<Element> CreateElementAsync(Guid projectId, Guid branchId, Element element);

    Task<List<Commit>> GetCommits(Guid projectId, Guid branchId);

    Task<Guid> CommitToBranchAsync(Guid projectId, Guid branchId, Commit commit);

    Task<List<Element>> GetElementsAsync(Guid projectId, Guid commitId);
}