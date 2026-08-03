using System.Text.Json;
using mcp.Src.Services;
using SysMLV2.MCP.Models;

namespace Src.Services;

public class ElementMutationService
{
    private readonly ISysMLApiService _api;
    private readonly SysMLMetaModelFactory _metamodelFactory;
    private readonly ProjectContextResolver _projectContext;

    public ElementMutationService(
        ISysMLApiService api,
        SysMLMetaModelFactory metamodelFactory,
        ProjectContextResolver projectContext)
    {
        _api = api;
        _metamodelFactory = metamodelFactory;
        _projectContext = projectContext;
    }

    public virtual ElementUpdateResult UpdateElementAttributes(string projectName, Guid elementId, string attributesJson)
    {
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

        var project = _projectContext.FindProjectByName(projectName);
        var projectId = project.Id ?? throw new Exception("Project has no ID.");
        var defaultBranchId = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch.");

        var branch = _api.GetBranchAsync(projectId, defaultBranchId).GetAwaiter().GetResult();
        var headCommitId = branch.Head?.Id ?? throw new Exception("Branch has no head commit.");

        var element = _api.GetElementByIdAsync(projectId, headCommitId, elementId).GetAwaiter().GetResult();
        var type = element.Type ?? throw new Exception($"Element '{elementId}' has no @type.");

        var validAttributeNames = _metamodelFactory.GetSchemaProperties(type).Keys.ToHashSet(StringComparer.Ordinal);

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

        _api.CommitToBranchAsync(projectId, defaultBranchId, commitRequest).GetAwaiter().GetResult();

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

    public virtual ElementCreationResult CreateElementOfType(string projectName, string elementType, string attributesJson, string? parentElementId = null)
    {
        if (!_metamodelFactory.GetAvailableSchemas().Contains(elementType))
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

        var validAttributeNames = _metamodelFactory.GetSchemaProperties(elementType).Keys.ToHashSet(StringComparer.Ordinal);
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

        var project = _projectContext.FindProjectByName(projectName);
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

        _api.CommitToBranchAsync(projectId, defaultBranchId, createCommitRequest).GetAwaiter().GetResult();

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

    public virtual ElementCreationResult CreateNamedElementOfType(string projectName, string elementType, string elementName, string? parentElementId = null)
    {
        var attributesJson = JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["name"] = elementName,
            ["declaredName"] = elementName
        });
        return CreateElementOfType(projectName, elementType, attributesJson, parentElementId);
    }
}