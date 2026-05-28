
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

    [McpServerTool, Description("Creates a UseCaseUsage element in the specified project. Optionally links an objective RequirementUsage and nests under a parent package.")]
    public static UseCaseLLMInformation CreateUseCase(
        McpServer server,
        string projectName,
        string useCaseName,
        Guid objectiveRequirementId = default,
        Guid parentPackageGuid = default)
    {
        var apiService = RequireApiService(server);
        var project = FindProjectByName(apiService, projectName);
        var factory = new SysMLUseCaseFactory(apiService);
        var (elementId, projectId) = factory.CreateUseCase(
            project.Id!.Value,
            useCaseName,
            objectiveRequirementId,
            parentPackageGuid)
            .GetAwaiter().GetResult();
        return new UseCaseLLMInformation(elementId, projectId, useCaseName);
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

    [McpServerTool, Description("Creates a signal definition element in the specified project. SysML v2 metamodel mapping: SignalDefinition -> ItemDefinition.")]
    public static ElementCreationResult CreateSignalDefinition(
        McpServer server,
        string projectName,
        string signalName,
        string? parentElementId = null)
    {
        return CreateNamedElementOfType(server, projectName, "ItemDefinition", signalName, parentElementId);
    }

    [McpServerTool, Description("Creates a signal usage element in the specified project. SysML v2 metamodel mapping: SignalUsage -> ItemUsage.")]
    public static ElementCreationResult CreateSignal(
        McpServer server,
        string projectName,
        string signalName,
        string? parentElementId = null)
    {
        return CreateNamedElementOfType(server, projectName, "ItemUsage", signalName, parentElementId);
    }

    [McpServerTool, Description("Creates a block definition element in the specified project. SysML v2 metamodel mapping: BlockDefinition -> PartDefinition.")]
    public static ElementCreationResult CreateBlockDefinition(
        McpServer server,
        string projectName,
        string blockDefinitionName,
        string? parentElementId = null)
    {
        return CreateNamedElementOfType(server, projectName, "PartDefinition", blockDefinitionName, parentElementId);
    }

    [McpServerTool, Description("Creates a part/block usage element in the specified project. SysML v2 metamodel mapping: BlockUsage -> PartUsage.")]
    public static ElementCreationResult CreatePart(
        McpServer server,
        string projectName,
        string partName,
        string? parentElementId = null)
    {
        return CreateNamedElementOfType(server, projectName, "PartUsage", partName, parentElementId);
    }

    [McpServerTool, Description("Creates an interface definition element in the specified project.")]
    public static ElementCreationResult CreateInterfaceDefinition(
        McpServer server,
        string projectName,
        string interfaceDefinitionName,
        string? parentElementId = null)
    {
        return CreateNamedElementOfType(server, projectName, "InterfaceDefinition", interfaceDefinitionName, parentElementId);
    }

    [McpServerTool, Description("Creates an interface usage element in the specified project.")]
    public static ElementCreationResult CreateInterface(
        McpServer server,
        string projectName,
        string interfaceName,
        string? parentElementId = null)
    {
        return CreateNamedElementOfType(server, projectName, "InterfaceUsage", interfaceName, parentElementId);
    }

    [McpServerTool, Description("Updates attributes of a signal definition element (mapped to ItemDefinition) in the specified project.")]
    public static ElementUpdateResult UpdateSignalDefinition(
        McpServer server,
        string projectName,
        Guid signalDefinitionId,
        string attributesJson)
    {
        return UpdateElementAttributes(server, projectName, signalDefinitionId, attributesJson);
    }

    [McpServerTool, Description("Updates attributes of a signal usage element (mapped to ItemUsage) in the specified project.")]
    public static ElementUpdateResult UpdateSignal(
        McpServer server,
        string projectName,
        Guid signalId,
        string attributesJson)
    {
        return UpdateElementAttributes(server, projectName, signalId, attributesJson);
    }

    [McpServerTool, Description("Updates attributes of a block definition element (mapped to PartDefinition) in the specified project.")]
    public static ElementUpdateResult UpdateBlockDefinition(
        McpServer server,
        string projectName,
        Guid blockDefinitionId,
        string attributesJson)
    {
        return UpdateElementAttributes(server, projectName, blockDefinitionId, attributesJson);
    }

    [McpServerTool, Description("Updates attributes of a part/block usage element (mapped to PartUsage) in the specified project.")]
    public static ElementUpdateResult UpdatePart(
        McpServer server,
        string projectName,
        Guid partId,
        string attributesJson)
    {
        return UpdateElementAttributes(server, projectName, partId, attributesJson);
    }

    [McpServerTool, Description("Updates attributes of an interface definition element in the specified project.")]
    public static ElementUpdateResult UpdateInterfaceDefinition(
        McpServer server,
        string projectName,
        Guid interfaceDefinitionId,
        string attributesJson)
    {
        return UpdateElementAttributes(server, projectName, interfaceDefinitionId, attributesJson);
    }

    [McpServerTool, Description("Updates attributes of an interface usage element in the specified project.")]
    public static ElementUpdateResult UpdateInterface(
        McpServer server,
        string projectName,
        Guid interfaceId,
        string attributesJson)
    {
        return UpdateElementAttributes(server, projectName, interfaceId, attributesJson);
    }

    [McpServerTool, Description("Adds a subject parameter (SubjectMembership + ReferenceUsage) to an existing RequirementUsage or RequirementDefinition. Returns the element ID of the created subject ReferenceUsage.")]
    public static Guid AddSubjectToRequirement(
        McpServer server,
        string projectName,
        Guid requirementId,
        string subjectName)
    {
        var apiService = RequireApiService(server);
        var project = FindProjectByName(apiService, projectName);
        var factory = new SysMLRequirementFactory(apiService);
        return factory.AddSubjectToRequirement(
            project.Id!.Value,
            requirementId,
            subjectName)
            .GetAwaiter().GetResult();
    }

    [McpServerTool, Description("Types a RequirementUsage against a RequirementDefinition by setting the requirementDefinition field. Fetches the current element state and re-commits with the updated field.")]
    public static void SetRequirementDefinition(
        McpServer server,
        string projectName,
        Guid requirementUsageId,
        Guid requirementDefinitionId)
    {
        var apiService = RequireApiService(server);
        var project = FindProjectByName(apiService, projectName);
        var factory = new SysMLRequirementFactory(apiService);
        factory.SetRequirementDefinition(
            project.Id!.Value,
            requirementUsageId,
            requirementDefinitionId)
            .GetAwaiter().GetResult();
    }

    [McpServerTool, Description("Fetches an element by ID and returns its SysML V2 type together with all schema-defined attributes split into required and optional, each mapped to its JSON Schema type description. Use this before UpdateElementAttributes to discover valid attribute names.")]
    public static ElementSchemaInfo DescribeElementSchema(
        McpServer server,
        string projectName,
        Guid elementId)
    {
        var apiService = RequireApiService(server);
        var metamodelFactory = RequireMetaModelFactory(server);
        var project = FindProjectByName(apiService, projectName);
        var (projectId, headCommitId) = GetProjectAndHeadCommitOrDefault(apiService, project);
        if (headCommitId == Guid.Empty)
            throw new Exception($"Project '{projectName}' has no commits yet.");

        var element = apiService.GetElementByIdAsync(projectId, headCommitId, elementId).GetAwaiter().GetResult();
        var type = element.Type ?? throw new Exception($"Element '{elementId}' has no @type.");

        var (required, allProperties) = metamodelFactory.GetTypeAttributeInfo(type);

        return new ElementSchemaInfo
        {
            ElementId = elementId.ToString(),
            Type = type,
            RequiredAttributes = allProperties
                .Where(kv => required.Contains(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value),
            OptionalAttributes = allProperties
                .Where(kv => !required.Contains(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value),
        };
    }

    [McpServerTool, Description("Returns all schema-defined attributes for a given SysML V2 element type, split into required and optional, each mapped to its JSON Schema type description. Use this when you know the type name but do not have an element ID. If the type name is not found an error listing all available types is thrown.")]
    public static ElementSchemaInfo DescribeTypeSchema(
        McpServer server,
        string elementType)
    {
        var metamodelFactory = RequireMetaModelFactory(server);

        var availableTypes = metamodelFactory.GetAvailableSchemas().OrderBy(x => x).ToList();
        if (!availableTypes.Contains(elementType))
            throw new ArgumentException(
                $"Unknown element type '{elementType}'. Available types: {string.Join(", ", availableTypes)}");

        var (required, allProperties) = metamodelFactory.GetTypeAttributeInfo(elementType);

        return new ElementSchemaInfo
        {
            ElementId = "",
            Type = elementType,
            RequiredAttributes = allProperties
                .Where(kv => required.Contains(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value),
            OptionalAttributes = allProperties
                .Where(kv => !required.Contains(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value),
        };
    }

    [McpServerTool, Description("Updates specific attributes on an existing SysML V2 element. Pass attributesJson as a JSON object string, e.g. {\"name\":\"NewName\",\"reqId\":\"REQ-002\"}. The element @type and all current attribute values are preserved; only the supplied attributes are overwritten. Attributes that do not exist in the element's schema are reported as invalid and skipped — no error is thrown.")]
    public static ElementUpdateResult UpdateElementAttributes(
        McpServer server,
        string projectName,
        Guid elementId,
        string attributesJson)
    {
        var apiService = RequireApiService(server);
        var metamodelFactory = RequireMetaModelFactory(server);

        Dictionary<string, JsonElement> updates;
        try
        {
            updates = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(attributesJson)
                      ?? throw new ArgumentException("attributesJson parsed to null.");
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"attributesJson is not valid JSON: {ex.Message}");
        }

        var project = FindProjectByName(apiService, projectName);
        var projectId = project.Id ?? throw new Exception("Project has no ID.");
        var defaultBranchId = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch.");

        var branch = apiService.GetBranchAsync(projectId, defaultBranchId).GetAwaiter().GetResult();
        var headCommitId = branch.Head?.Id ?? throw new Exception("Branch has no head commit.");

        var element = apiService.GetElementByIdAsync(projectId, headCommitId, elementId).GetAwaiter().GetResult();
        var type = element.Type ?? throw new Exception($"Element '{elementId}' has no @type.");

        var validAttributeNames = metamodelFactory.GetSchemaProperties(type).Keys.ToHashSet(StringComparer.Ordinal);

        var updated = new List<string>();
        var invalid = new List<string>();

        foreach (var key in updates.Keys)
        {
            if (key == "@id")
                invalid.Add($"{key} (read-only: element identity cannot be changed)");
            else if (validAttributeNames.Contains(key) || key.StartsWith("@"))
                updated.Add(key);
            else
                invalid.Add(key);
        }

        if (updated.Count == 0)
        {
            return new ElementUpdateResult
            {
                ElementId = elementId.ToString(),
                Type = type,
                UpdatedAttributes = updated,
                InvalidAttributes = invalid,
                Success = false,
                Message = "No valid attributes to update. Use DescribeElementSchema to discover valid attribute names."
            };
        }

        var payload = new Dictionary<string, JsonElement>
        {
            ["@id"]   = JsonSerializer.SerializeToElement(element.Id!.Value),
            ["@type"] = JsonSerializer.SerializeToElement(element.Type!)
        };
        if (element.AdditionalProperties != null)
            foreach (var kvp in element.AdditionalProperties)
                payload[kvp.Key] = kvp.Value;

        foreach (var key in updated)
            payload[key] = updates[key];

        var commitRequest = new CommitRequest
        {
            Change =
            [
                new DataVersionRequest
                {
                    Identity = new SysMLRef(elementId),
                    Payload  = JsonSerializer.SerializeToElement(payload)
                }
            ]
        };

        apiService.CommitToBranchAsync(projectId, defaultBranchId, commitRequest).GetAwaiter().GetResult();

        return new ElementUpdateResult
        {
            ElementId = elementId.ToString(),
            Type = type,
            UpdatedAttributes = updated,
            InvalidAttributes = invalid,
            Success = true,
            Message = invalid.Count > 0
                ? $"Updated {updated.Count} attribute(s). {invalid.Count} attribute(s) were invalid and skipped."
                : $"Successfully updated {updated.Count} attribute(s)."
        };
    }

    [McpServerTool, Description("Creates a new SysML V2 element of the given type and commits it under the specified parent. Use DescribeTypeSchema first to discover valid attribute names. Pass attributesJson as a JSON object string, e.g. {\"name\":\"MyElement\",\"isAbstract\":false}. Pass parentElementId to nest the element inside a package or another element; omit or pass null to create at the project root. @id and @type are managed automatically and must not appear in attributesJson. Attributes not in the schema are reported as invalid and excluded without failing.")]
    public static ElementCreationResult CreateElementOfType(
        McpServer server,
        string projectName,
        string elementType,
        string attributesJson,
        string? parentElementId = null)
    {
        var apiService = RequireApiService(server);
        var metamodelFactory = RequireMetaModelFactory(server);

        if (!metamodelFactory.GetAvailableSchemas().Contains(elementType))
            throw new ArgumentException(
                $"Unknown element type '{elementType}'. Call DescribeTypeSchema for valid types.");

        Dictionary<string, JsonElement> attributes;
        try
        {
            attributes = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(attributesJson)
                         ?? throw new ArgumentException("attributesJson parsed to null.");
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"attributesJson is not valid JSON: {ex.Message}");
        }

        var validAttributeNames = metamodelFactory.GetSchemaProperties(elementType).Keys.ToHashSet(StringComparer.Ordinal);
        var applied = new List<string>();
        var invalid = new List<string>();

        foreach (var key in attributes.Keys)
        {
            if (key == "@id" || key == "@type")
                invalid.Add($"{key} (reserved: managed automatically)");
            else if (validAttributeNames.Contains(key))
                applied.Add(key);
            else
                invalid.Add(key);
        }

        Guid? parentId = null;
        if (!string.IsNullOrWhiteSpace(parentElementId) && Guid.TryParse(parentElementId, out var parsedParent))
            parentId = parsedParent;

        var project = FindProjectByName(apiService, projectName);
        var projectId = project.Id ?? throw new Exception("Project has no ID.");
        var defaultBranchId = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch.");

        var newElementId = Guid.NewGuid();
        var payload = new Dictionary<string, JsonElement>
        {
            ["@id"]   = JsonSerializer.SerializeToElement(newElementId),
            ["@type"] = JsonSerializer.SerializeToElement(elementType)
        };

        if (parentId.HasValue)
        {
            var ownerRef = JsonSerializer.SerializeToElement(new SysMLRef(parentId.Value));
            payload["owner"] = ownerRef;
            payload["owningNamespace"] = ownerRef;
        }

        foreach (var key in applied)
            payload[key] = attributes[key];

        var createCommitRequest = new CommitRequest
        {
            Change =
            [
                new DataVersionRequest
                {
                    Identity = new SysMLRef(newElementId),
                    Payload  = JsonSerializer.SerializeToElement(payload)
                }
            ]
        };

        apiService.CommitToBranchAsync(projectId, defaultBranchId, createCommitRequest).GetAwaiter().GetResult();

        return new ElementCreationResult
        {
            ElementId = newElementId.ToString(),
            Type = elementType,
            ParentId = parentId?.ToString(),
            AppliedAttributes = applied,
            InvalidAttributes = invalid,
            Success = true,
            Message = invalid.Count > 0
                ? $"Element of type '{elementType}' created with {applied.Count} attribute(s). {invalid.Count} attribute(s) were invalid and excluded."
                : $"Element of type '{elementType}' created successfully with {applied.Count} attribute(s)."
        };
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

    private static ElementCreationResult CreateNamedElementOfType(
        McpServer server,
        string projectName,
        string elementType,
        string elementName,
        string? parentElementId = null)
    {
        var attributesJson = JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["name"] = elementName,
            ["declaredName"] = elementName
        });

        return CreateElementOfType(server, projectName, elementType, attributesJson, parentElementId);
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
