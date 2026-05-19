using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Src.Services;
using SysMLV2.MCP.Models;

namespace mcp.Src.Services;

public class SysMLApiService : ISysMLApiService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public SysMLApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("SysMLV2-Database-Client");
    }

    // ── Projects ────────────────────────────────────────────────────────────

    public async Task<List<SysMLProject>> GetProjects()
    {
        return await _httpClient.GetFromJsonAsync<List<SysMLProject>>("/projects", _jsonOptions)
               ?? new List<SysMLProject>();
    }

    public async Task<SysMLProject> CreateNewProjectAsync(string projectName, string projectDescription)
    {
        var body = new JsonObject
        {
            ["@type"] = "Project",
            ["name"] = projectName,
            ["description"] = projectDescription
        };
        var response = await _httpClient.PostAsync("/projects", JsonContent.Create(body));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SysMLProject>(_jsonOptions)
               ?? throw new Exception("Failed to deserialize created project");
    }

    public async Task<SysMLProject> GetProjectAsync(Guid projectId)
    {
        return await _httpClient.GetFromJsonAsync<SysMLProject>($"/projects/{projectId}", _jsonOptions)
               ?? throw new Exception($"Project {projectId} not found");
    }

    // ── Branches ─────────────────────────────────────────────────────────────

    public async Task<List<SysMLBranch>> GetBranchesAsync(Guid projectId)
    {
        return await _httpClient.GetFromJsonAsync<List<SysMLBranch>>($"/projects/{projectId}/branches", _jsonOptions)
               ?? new List<SysMLBranch>();
    }

    public async Task<SysMLBranch> GetBranchAsync(Guid projectId, Guid branchId)
    {
        return await _httpClient.GetFromJsonAsync<SysMLBranch>($"/projects/{projectId}/branches/{branchId}", _jsonOptions)
               ?? throw new Exception($"Branch {branchId} not found in project {projectId}");
    }

    public async Task<SysMLBranch> CreateNewBranchAsync(Guid projectId, string branchName)
    {
        var project = await GetProjectAsync(projectId);
        var defaultBranchId = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch");
        var defaultBranch = await GetBranchAsync(projectId, defaultBranchId);
        var headCommitId = defaultBranch.Head?.Id ?? throw new Exception("Default branch has no head commit");

        var body = new JsonObject
        {
            ["@type"] = "Branch",
            ["name"] = branchName,
            ["head"] = new JsonObject { ["@id"] = headCommitId.ToString() }
        };
        var response = await _httpClient.PostAsync($"/projects/{projectId}/branches", JsonContent.Create(body));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SysMLBranch>(_jsonOptions)
               ?? throw new Exception("Failed to deserialize created branch");
    }

    // ── Commits ──────────────────────────────────────────────────────────────

    public async Task<List<SysMLCommit>> GetCommits(Guid projectId, Guid branchId)
    {
        return await _httpClient.GetFromJsonAsync<List<SysMLCommit>>($"/projects/{projectId}/commits", _jsonOptions)
               ?? new List<SysMLCommit>();
    }

    public async Task<SysMLCommit> GetCommitAsync(Guid projectId, Guid commitId)
    {
        return await _httpClient.GetFromJsonAsync<SysMLCommit>($"/projects/{projectId}/commits/{commitId}", _jsonOptions)
               ?? throw new Exception($"Commit {commitId} not found in project {projectId}");
    }

    public async Task<Guid> CommitToBranchAsync(Guid projectId, Guid branchId, CommitRequest commit)
    {
        var url = branchId != Guid.Empty
            ? $"/projects/{projectId}/commits?branchId={branchId}"
            : $"/projects/{projectId}/commits";

        var response = await _httpClient.PostAsJsonAsync(url, commit, _jsonOptions);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<SysMLCommit>(_jsonOptions)
                      ?? throw new Exception("Failed to deserialize created commit");
        return created.Id ?? throw new Exception("Created commit has no @id");
    }

    // ── Elements ─────────────────────────────────────────────────────────────

    public async Task<List<SysMLElement>> GetElementsAsync(Guid projectId, Guid commitId)
    {
        return await _httpClient.GetFromJsonAsync<List<SysMLElement>>(
                   $"/projects/{projectId}/commits/{commitId}/elements", _jsonOptions)
               ?? new List<SysMLElement>();
    }

    public async Task<SysMLElement> GetElementByIdAsync(Guid projectId, Guid commitId, Guid elementId)
    {
        return await _httpClient.GetFromJsonAsync<SysMLElement>(
                   $"/projects/{projectId}/commits/{commitId}/elements/{elementId}", _jsonOptions)
               ?? throw new Exception($"Element {elementId} not found in commit {commitId} (project {projectId})");
    }

    public async Task<SysMLElement> CreateElementAsync(Guid projectId, Guid branchId, SysMLElement element)
    {
        var payload = JsonSerializer.SerializeToElement(element, _jsonOptions);
        var commit = new CommitRequest
        {
            Change = [new DataVersionRequest { Payload = payload }]
        };
        var commitId = await CommitToBranchAsync(projectId, branchId, commit);
        var elements = await GetElementsAsync(projectId, commitId);

        if (element.Id.HasValue)
        {
            var found = elements.FirstOrDefault(e => e.Id == element.Id);
            if (found != null) return found;
        }
        return elements.FirstOrDefault()
               ?? throw new Exception("No elements found in commit after create");
    }
}