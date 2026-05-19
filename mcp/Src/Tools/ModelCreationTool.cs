
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ModelContextProtocol.Server;
using Src.Services;
using mcp.Src.Services;
using MCP.Src.Services.FactoryServices;
using SysMLV2.MCP.Models;

[McpServerToolType]
public class ModelCreationTools
{

    public ModelCreationTools()
    {
        Debug.WriteLine("Model Creation Tool Initialized");
    }


    [McpServerTool, Description("Creates a new SysML V2 model in the specified project.")]
    public static string CreateProject(McpServer server, string projectName)
    {
        Debug.WriteLine("Model Creation Tool is handling operation...");
        var sysMLApiService = RequireApiService(server);
        var result = sysMLApiService.CreateNewProjectAsync(projectName, "Created via MCP Tool").GetAwaiter().GetResult();
        return string.Format("Project '{0}' created successfully with ID: {1}.", projectName, result.Id);
    }

    [McpServerTool, Description("Gets all SysML V2 projects from localhost:9000.")]
    public static List<ProjectLookupResult> GetProjects(McpServer server)
    {
        var apiService = RequireApiService(server);
        return apiService
            .GetProjects()
            .GetAwaiter()
            .GetResult()
            .Select(project => new ProjectLookupResult
            {
                Id = project.Id ?? Guid.Empty,
                Name = project.Name ?? string.Empty,
                DefaultBranchId = project.DefaultBranch?.Id ?? Guid.Empty,
                Description = project.Description ?? string.Empty
            })
            .ToList();
    }

    [McpServerTool, Description("Gets a SysML V2 project by name from localhost:9000.")]
    public static ProjectLookupResult GetProjectByName(McpServer server, string projectName)
    {
        var apiService = RequireApiService(server);
        var project = FindProjectByName(apiService, projectName);
        return new ProjectLookupResult
        {
            Id = project.Id ?? Guid.Empty,
            Name = project.Name ?? string.Empty,
            DefaultBranchId = project.DefaultBranch?.Id ?? Guid.Empty,
            Description = project.Description ?? string.Empty
        };
    }

    [McpServerTool, Description("Creates a new SysML V2 element in the specified project.")]
    public static string CreateElement(string elementName)
    {
        Debug.WriteLine("Element Creation Tool is handling operation...");
        return string.Format("Element '{0}' created successfully.", elementName);
    }

    [McpServerTool, Description("Creates a new relationship between two elements in the specified project.")]
    public static string CreateRelationship(string sourceElement, string targetElement)
    {
        throw new NotImplementedException();
    }

    [McpServerTool, Description("Creates a new SysML V2 package inside an optional parent package.")]
    public static Guid CreatePackage(McpServer server, string projectName, string packageName, Guid parentPackageGuid = default)
    {
        Debug.WriteLine("Package creation tool is handling operation...");
        var apiService = RequireApiService(server);
        var metamodelFactory = RequireMetaModelFactory(server);
        var packageFactory = new SysMLPackageFactory(apiService, metamodelFactory);
        var projects = apiService.GetProjects().GetAwaiter().GetResult();
        var foundProject = projects.FirstOrDefault(p => p.Name == projectName)
                           ?? throw new Exception($"project with name {projectName} not found!");

        return packageFactory.CreatePackage((Guid)foundProject.Id!, packageName, packageName, parentPackageGuid)
            .GetAwaiter()
            .GetResult();
    }

    [McpServerTool, Description("Creates a top-level SysML V2 package in the specified project.")]
    public static Guid CreateTopLevelPackage(McpServer server, string projectName, string packageName)
    {
        return CreatePackage(server, projectName, packageName, Guid.Empty);
    }

    [McpServerTool, Description("Gets elements from the default branch head commit of a project in localhost:9000. Optional filters by elementType and nameContains.")]
    public static List<ElementLookupResult> GetElementsFromProjectHead(McpServer server, string projectName, string? elementType = null, string? nameContains = null)
    {
        var apiService = RequireApiService(server);
        var project = FindProjectByName(apiService, projectName);
        var (projectId, headCommitId) = GetProjectAndHeadCommitOrDefault(apiService, project);

        if (headCommitId == Guid.Empty)
        {
            return new List<ElementLookupResult>();
        }

        var elements = apiService.GetElementsAsync(projectId, headCommitId).GetAwaiter().GetResult();
        return elements
            .Where(element =>
            {
                var type = element.Type ?? string.Empty;
                var name = element.GetName();

                var typeOk = string.IsNullOrWhiteSpace(elementType) || string.Equals(type, elementType, StringComparison.OrdinalIgnoreCase);
                var nameOk = string.IsNullOrWhiteSpace(nameContains) || name.Contains(nameContains, StringComparison.OrdinalIgnoreCase);
                return typeOk && nameOk;
            })
            .Select(element => new ElementLookupResult
            {
                Id = element.Id ?? Guid.Empty,
                Name = element.GetName(),
                Type = element.Type ?? string.Empty
            })
            .ToList();
    }

    [McpServerTool, Description("Gets all elements from the default branch head commit of a project in localhost:9000.")]
    public static List<ElementLookupResult> GetAllElementsFromProjectHead(McpServer server, string projectName)
    {
        return GetElementsFromProjectHead(server, projectName);
    }

    [McpServerTool, Description("Gets elements of a specific type from the default branch head commit of a project in localhost:9000.")]
    public static List<ElementLookupResult> GetElementsByTypeFromProjectHead(McpServer server, string projectName, string elementType)
    {
        return GetElementsFromProjectHead(server, projectName, elementType, null);
    }

    [McpServerTool, Description("Gets packages from the default branch head commit of a project in localhost:9000. Optional filter by packageNameContains.")]
    public static List<ElementLookupResult> GetPackagesFromProjectHead(McpServer server, string projectName, string? packageNameContains = null)
    {
        var packages = GetElementsFromProjectHead(server, projectName)
            .Where(element => string.Equals(element.Type, "Package", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(element.Type, "LibraryPackage", StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(packageNameContains))
        {
            packages = packages.Where(element => element.Name.Contains(packageNameContains, StringComparison.OrdinalIgnoreCase));
        }

        return packages.ToList();
    }

    [McpServerTool, Description("Gets an element by ID from the default branch head commit of a project in localhost:9000.")]
    public static ElementLookupResult GetElementByIdFromProjectHead(McpServer server, string projectName, Guid elementId)
    {
        var apiService = RequireApiService(server);
        var project = FindProjectByName(apiService, projectName);
        var (projectId, headCommitId) = GetProjectAndHeadCommitOrDefault(apiService, project);

        if (headCommitId == Guid.Empty)
        {
            throw new Exception($"Project '{project.Name}' has no head commit yet, so no elements are available.");
        }

        var element = apiService.GetElementByIdAsync(projectId, headCommitId, elementId).GetAwaiter().GetResult();

        static string? ExtractRef(SysMLElement el, string key)
        {
            if (el.AdditionalProperties == null) return null;
            if (!el.AdditionalProperties.TryGetValue(key, out var val)) return null;
            if (val.ValueKind == System.Text.Json.JsonValueKind.Null) return null;
            if (val.ValueKind == System.Text.Json.JsonValueKind.Object
                && val.TryGetProperty("@id", out var idProp))
                return idProp.GetString();
            return null;
        }

        return new ElementLookupResult
        {
            Id = element.Id ?? Guid.Empty,
            Name = element.GetName(),
            Type = element.Type ?? string.Empty,
            OwnerId = ExtractRef(element, "owner"),
            OwningNamespaceId = ExtractRef(element, "owningNamespace"),
            OwningMembershipId = ExtractRef(element, "owningMembership")
        };
    }

    [McpServerTool, Description("Creates a new use case in the specified project.")]
    public static UseCaseLLMInformation CreateUseCase(string name, string documentation, string precondition)
    {
        throw new NotImplementedException();
    }

    [McpServerTool, Description("Creates a new actor in the specified project.")]
    public static string CreateActor(string name, string documentation)
    {
        throw new NotImplementedException();
    }

    [McpServerTool, Description("Creates a RequirementUsage element in the specified project. Optionally nested under a parent package.")]
    public static Guid CreateRequirement(
        McpServer server,
        string projectName,
        string requirementName,
        string requirementText,
        string? reqId = null,
        Guid parentPackageGuid = default)
    {
        var apiService = RequireApiService(server);
        var project = FindProjectByName(apiService, projectName);
        var factory = new SysMLRequirementFactory(apiService);
        return factory.CreateRequirement(
            project.Id!.Value,
            requirementName,
            requirementText,
            reqId,
            parentPackageGuid)
            .GetAwaiter().GetResult();
    }

    [McpServerTool, Description("Creates a RequirementDefinition element in the specified project. Optionally nested under a parent package.")]
    public static Guid CreateRequirementDefinition(
        McpServer server,
        string projectName,
        string definitionName,
        string definitionText,
        string? reqId = null,
        bool isAbstract = false,
        Guid parentPackageGuid = default)
    {
        var apiService = RequireApiService(server);
        var project = FindProjectByName(apiService, projectName);
        var factory = new SysMLRequirementFactory(apiService);
        return factory.CreateRequirementDefinition(
            project.Id!.Value,
            definitionName,
            definitionText,
            reqId,
            isAbstract,
            parentPackageGuid)
            .GetAwaiter().GetResult();
    }

    private static ISysMLApiService RequireApiService(McpServer server)
    {
        return server.Services?.GetService<ISysMLApiService>() ?? throw new Exception("ISysMLApiService is not registered.");
    }

    private static SysMLMetaModelFactory RequireMetaModelFactory(McpServer server)
    {
        return server.Services?.GetService<SysMLMetaModelFactory>() ?? throw new Exception("SysMLMetaModelFactory is not registered.");
    }

    private static SysMLProject FindProjectByName(ISysMLApiService apiService, string projectName)
    {
        var projects = apiService.GetProjects().GetAwaiter().GetResult();
        return projects.FirstOrDefault(project => string.Equals(project.Name, projectName, StringComparison.OrdinalIgnoreCase))
               ?? throw new Exception($"Project with name '{projectName}' was not found.");
    }

    private static (Guid ProjectId, Guid HeadCommitId) GetProjectAndHeadCommitOrDefault(ISysMLApiService apiService, SysMLProject project)
    {
        var projectId = project.Id ?? throw new Exception("Project has no ID.");
        var defaultBranchId = project.DefaultBranch?.Id ?? throw new Exception($"Project '{project.Name}' has no default branch.");
        var defaultBranch = apiService.GetBranchAsync(projectId, defaultBranchId).GetAwaiter().GetResult();
        var headCommitId = defaultBranch.Head?.Id ?? Guid.Empty;
        return (projectId, headCommitId);
    }

    public class ProjectLookupResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid DefaultBranchId { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class ElementLookupResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? OwnerId { get; set; }
        public string? OwningNamespaceId { get; set; }
        public string? OwningMembershipId { get; set; }
    }
}

