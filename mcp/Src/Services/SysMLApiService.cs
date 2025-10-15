using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;
using Src.Services;

namespace mcp.Src.Services
{

    public class SysMLApiService : ISysMLApiService
    {
        private const string BaseUrl = "http://localhost:9000"; // Example base URL
        private readonly IHost _host;
        private readonly HttpClient _httpClient;

        private readonly IProjectApi _projectApi;

        public SysMLApiService(IHost httpHost)
        {
            _host = httpHost;
            _httpClient = _host.Services.GetRequiredService<IHttpClientFactory>().CreateClient("SysMLV2-Database-Client");
            // Create the services we need MANUALLY and not via DI
            ILoggerFactory loggerFactory = _host.Services.GetRequiredService<ILoggerFactory>();
            JsonSerializerOptionsProvider jsonSerializerOptionsProvider = new(
                new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
                }
            );

            _projectApi = new ProjectApi(loggerFactory.CreateLogger<ProjectApi>(),
                loggerFactory,
                _httpClient,
                jsonSerializerOptionsProvider,
                new ProjectApiEvents());
        }

        public async Task<string> CreateNewProjectAsync(string projectName, string projectDescription)
        {

            var response = await _projectApi.PostProjectOrDefaultAsync(
                new Project(null,
                    Project.TypeEnum.Project,
                    null, projectDescription,
                    projectName));

            if (response != null && response.TryCreated(out var project))
            {
                Console.WriteLine(response.ToString());
                var newProjectId = project.Id;
                return newProjectId.ToString();
            }
            else
            {
                return response.ToString();
            }
        }

        public async Task<Guid> CreateNewBranchAsync(Guid projectId, string branchName)
        {
            var apiInstance = _host.Services.GetRequiredService<IBranchApi>();
            Guid branchGuid = new();
            var response = await apiInstance.PostBranchByProjectOrDefaultAsync(projectId,
                new Org.OpenAPITools.Model.Branch(branchGuid, null, null, null));

            return new Guid();
        }

        public async Task<List<CommitInformation>> GetCommits(Guid projectId, Guid branchId)
        {
            var apiInstance = _host.Services.GetRequiredService<ICommitApi>();
            var response = await apiInstance.GetCommitsByProjectAsync(projectId);
            // Map to CommitInformation if needed
            // return response.Select(c => new CommitInformation()).ToList();
            return new List<CommitInformation>();
        }

        public async Task<Guid> CommitElementToBranchAsync(Guid projectId, Guid branchId, Org.OpenAPITools.Model.Commit commit)
        {
            var apiInstance = _host.Services.GetRequiredService<ICommitApi>();
            // var response = await apiInstance.PostCommitOrDefaultAsync(projectId, branchId, commit);
            // return response.Id;
            return new Guid();
        }
    }

}