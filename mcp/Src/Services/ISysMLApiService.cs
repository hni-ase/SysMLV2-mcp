using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;
using Org.OpenAPITools.Extensions;

namespace Src.Services
{
    public interface ISysMLApiService
    {
        Task<Guid> CreateNewProjectAsync(string projectName, string projectDescription);
        Task<Guid> CreateNewBranchAsync(Guid projectId, string branchName);

        Task<List<CommitInformation>> GetCommits(Guid projectId, Guid branchId);
        Task<Guid> CommitElementToBranchAsync(Guid projectId, Guid branchId, Commit commit);
    }

    public class CommitInformation
    {
    }

    // public class SysMLApiService : ISysMLApiService
    // {
    //     private const string BaseUrl = "http://localhost:9000"; // Example base URL
    //     private readonly IHost _host;

    //     public SysMLApiService(IHost httpHost)
    //     {
    //         _host = httpHost;
    //     }

    //     public async Task<object> CreateModelAsync(object modelData)
    //     {
    //         var apiInstance = _host.Services.GetRequiredService<IProjectApi>();
    //         Guid projectGuid = new();
    //         Guid branchGuid = new();
    //         var response = await apiInstance.PostProjectOrDefaultAsync(
    //             new Project(projectGuid,
    //             Project.TypeEnum.Project, new ProjectDefaultBranch(branchGuid), "This is a model description", "modelName"));

    //         // Now we need to add the model data to the project we just creatd
    //         return response;
    //     }

    //     public Task<object> GetModelAsync(string modelId)
    //     {
    //         throw new NotImplementedException();
    //     }

    //     public Task<object> UpdateModelAsync(string modelId, object modelData)
    //     {
    //         throw new NotImplementedException();
    //     }

    //     public Task<bool> DeleteModelAsync(string modelId)
    //     {
    //         throw new NotImplementedException();
    //     }


    // }
}