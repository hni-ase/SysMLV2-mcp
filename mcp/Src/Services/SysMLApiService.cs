using Microsoft.AspNetCore.Authorization.Infrastructure;
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
        private readonly IBranchApi _branchApi;
        private readonly IElementApi _elementApi;
        private readonly IQueryApi _queryApi;
        private readonly IRelationshipApi _relationshipApi;
        private readonly ICommitApi _commitApi;
        private readonly ITagApi _tagApi;

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
                    Converters =
                    {
                        new ProjectJsonConverter(),
                        new ProjectDefaultBranchJsonConverter(),
                        new BranchJsonConverter(),
                        new BranchHeadJsonConverter(),
                        new BranchOwningProjectJsonConverter(),
                        new CommitJsonConverter(),
                        new DataVersionJsonConverter()
                    }
                }
            );

            _projectApi = new ProjectApi(loggerFactory.CreateLogger<ProjectApi>(),
                loggerFactory,
                _httpClient,
                jsonSerializerOptionsProvider,
                new ProjectApiEvents());

            _branchApi = new BranchApi(loggerFactory.CreateLogger<BranchApi>(),
                loggerFactory,
                _httpClient,
                jsonSerializerOptionsProvider,
                new BranchApiEvents());

            _elementApi = new ElementApi(loggerFactory.CreateLogger<ElementApi>(),
                loggerFactory,
                _httpClient,
                jsonSerializerOptionsProvider,
                new ElementApiEvents());

            _queryApi = new QueryApi(loggerFactory.CreateLogger<QueryApi>(),
                loggerFactory,
                _httpClient,
                jsonSerializerOptionsProvider,
                new QueryApiEvents());

            _relationshipApi = new RelationshipApi(loggerFactory.CreateLogger<RelationshipApi>(),
                loggerFactory,
                _httpClient,
                jsonSerializerOptionsProvider,
                new RelationshipApiEvents());

            _commitApi = new CommitApi(loggerFactory.CreateLogger<CommitApi>(),
                loggerFactory,
                _httpClient,
                jsonSerializerOptionsProvider,
                new CommitApiEvents());
            _tagApi = new TagApi(loggerFactory.CreateLogger<TagApi>(),
                loggerFactory,
                _httpClient,
                jsonSerializerOptionsProvider,
                new TagApiEvents());
        }

        public async Task<Project> CreateNewProjectAsync(string projectName, string projectDescription)
        {
            var response = await _projectApi.PostProjectOrDefaultAsync(
                new Project(
                    default,
                    Project.TypeEnum.Project,
                    default,
                    default,
                    default,
                    projectDescription,
                    projectName));

            if (response != null && response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.ToString());
                var project = response.Created();
                var newProjectId = project.Id ?? throw new Exception("project not created");
                return project;
            }
            else
            {
                throw new Exception("Failed to create project");
            }
        }

        public async Task<Project> GetProjectAsync(Guid projectId)
        {
            var response = await _projectApi.GetProjectByIdOrDefaultAsync(projectId);
            if (response != null && response.TryOk(out var project))
            {
                return project;
            }
            throw new Exception($"Project with id {projectId} not found");
        }

        public async Task<Branch> CreateNewBranchAsync(Guid projectId, string branchName)
        {
            Branch? retval = null;
            // we need to first get the project to find its default branch
            var projectResponse = await this._projectApi.GetProjectByIdOrDefaultAsync(projectId) ?? throw new Exception($"Project with id {projectId} not found");
            if (projectResponse.TryOk(out var retreievedProject))
            {
                // Now we need to find the default branch head
                var defaultBranchGuid = retreievedProject.DefaultBranch?.Id ?? throw new Exception("Project has no default branch");
                var projectGuid = retreievedProject.Id ?? throw new Exception("Project has no ID");
                // now we need to get the default branch
                var defaultBranchResponse = await this._branchApi.GetBranchesByProjectAndIdOrDefaultAsync(projectGuid, defaultBranchGuid) ?? throw new Exception($"Default branch with id {defaultBranchGuid} not found");
                defaultBranchResponse.TryOk(out var defaultBranch);
                var branchHeadId = defaultBranch?.Head?.Id ?? throw new Exception("Default branch has no head");
                // Now we can create the new branch
                var response = await _branchApi.PostBranchByProjectOrDefaultAsync(
                    projectGuid,
                    new Branch(
                        default,
                        Branch.TypeEnum.Branch,
                        default,
                        default,
                        default,
                        default,
                        new BranchHead(branchHeadId), branchName,
                        default,
                        default,
                        default));
                if (response != null && response.TryCreated(out var newBranch))
                {
                    retval = newBranch;
                }
            }
            return retval ?? throw new Exception("Failed to create branch");
        }

        public async Task<List<Commit>> GetCommits(Guid projectId, Guid branchId)
        {
            var apiInstance = _host.Services.GetRequiredService<ICommitApi>();
            var response = await apiInstance.GetCommitsByProjectAsync(projectId);
            // Map to CommitInformation if needed
            // return response.Select(c => new CommitInformation()).ToList();
            return new List<Commit>();
        }

        public async Task<Guid> CommitToBranchAsync(Guid projectId, Guid branchId, Org.OpenAPITools.Model.Commit commit)
        {
            var response = await _commitApi.PostCommitByProjectOrDefaultAsync(projectId, commit, branchId);
            if (response != null && response.TryCreated(out var createdCommit))
            {
                return createdCommit.Id ?? throw new Exception("Created commit has no id!");
            }
            else
            {
                throw new Exception($"Commit creation with body {commit} failed!");
            }
        }


        public async Task<Element> CreateElementAsync(Guid projectId, Guid branchId, Element element)
        {
            throw new NotImplementedException();
        }

        public async Task<Branch> GetBranchAsync(Guid projectId, Guid branchId)
        {
            var response = await _branchApi.GetBranchesByProjectAndIdOrDefaultAsync(projectId, branchId);
            if (response != null && response.TryOk(out var branch))
            {
                return branch;
            }
            throw new Exception($"Branch with id {branchId} in project with id {projectId} not found");
        }

        public async Task<List<Element>> GetElementsAsync(Guid projectId, Guid commitId)
        {
            var response = await _elementApi.GetElementsByProjectCommitOrDefaultAsync(projectId, commitId);
            if (response != null && response.TryOk(out var elements))
            {
                return elements;
            } else
            {
                throw new Exception($"Failed to fetch elements for commit {commitId} in project {projectId}");
            }
        }

        public async Task<List<Project>> GetProjects()
        {
            var response = await _projectApi.GetProjectsAsync();
            if (response != null && response.TryOk(out var elements))
            {
                return elements;
            } else
            {
                throw new Exception($"Failed to fetch projects!");
            }
        }
    }

}