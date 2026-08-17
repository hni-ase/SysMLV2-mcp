using System.ComponentModel;
using ModelContextProtocol.Server;
using Src.Services;
using SysMLV2.MCP.Models;

namespace Tools.ElementQuery;

[McpServerToolType]
public class ElementQueryTools
{
    private readonly ISysMLApiService _api;
    private readonly ProjectContextResolver _projectContext;

    public ElementQueryTools(ISysMLApiService api, ProjectContextResolver projectContext)
    {
        _api = api;
        _projectContext = projectContext;
    }

    [McpServerTool, Description("Gets elements from the default branch head commit of a project in localhost:9000. Optional filters by elementType and nameContains.")]
    public List<ElementLookupResult> GetElementsFromProjectHead(string projectName, string? elementType = null, string? nameContains = null)
    {
        var project = _projectContext.FindProjectByName(projectName);
        var (projectId, headCommitId) = _projectContext.GetProjectAndHeadCommitOrDefault(project);

        if (headCommitId == Guid.Empty)
        {
            return new List<ElementLookupResult>();
        }

        var elements = _api.GetElementsAsync(projectId, headCommitId).GetAwaiter().GetResult();
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
    public List<ElementLookupResult> GetAllElementsFromProjectHead(string projectName)
        => GetElementsFromProjectHead(projectName);

    [McpServerTool, Description("Gets elements of a specific type from the default branch head commit of a project in localhost:9000.")]
    public List<ElementLookupResult> GetElementsByTypeFromProjectHead(string projectName, string elementType)
        => GetElementsFromProjectHead(projectName, elementType, null);

    [McpServerTool, Description("Gets packages from the default branch head commit of a project in localhost:9000. Optional filter by packageNameContains.")]
    public List<ElementLookupResult> GetPackagesFromProjectHead(string projectName, string? packageNameContains = null)
    {
        var packages = GetElementsFromProjectHead(projectName)
            .Where(element => string.Equals(element.Type, "Package", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(element.Type, "LibraryPackage", StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(packageNameContains))
        {
            packages = packages.Where(element => element.Name.Contains(packageNameContains, StringComparison.OrdinalIgnoreCase));
        }

        return packages.ToList();
    }

    [McpServerTool, Description("Gets an element by ID from the default branch head commit of a project in localhost:9000.")]
    public ElementLookupResult GetElementByIdFromProjectHead(string projectName, Guid elementId)
    {
        var project = _projectContext.FindProjectByName(projectName);
        var (projectId, headCommitId) = _projectContext.GetProjectAndHeadCommitOrDefault(project);

        if (headCommitId == Guid.Empty)
        {
            throw new Exception($"Project '{project.Name}' has no head commit yet, so no elements are available.");
        }

        var element = _api.GetElementByIdAsync(projectId, headCommitId, elementId).GetAwaiter().GetResult();

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
}