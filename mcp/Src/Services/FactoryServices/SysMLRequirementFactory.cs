using System.Text.Json;
using mcp.Src.Services;
using Src.Services;
using SysMLV2.MCP.Models;

namespace MCP.Src.Services.FactoryServices;

public class SysMLRequirementFactory
{
    private readonly ISysMLApiService _sysMLApiService;

    public SysMLRequirementFactory(ISysMLApiService sysMLApiService)
    {
        _sysMLApiService = sysMLApiService;
    }

    /// <summary>
    /// Creates a RequirementUsage element and commits it to the project's default branch.
    /// </summary>
    /// <param name="projectId">Target project ID.</param>
    /// <param name="requirementName">Display name of the requirement.</param>
    /// <param name="requirementText">Requirement text / description.</param>
    /// <param name="reqId">Optional short requirement identifier (e.g. "REQ-001").</param>
    /// <param name="ownerPackageGuid">Optional parent package to nest this requirement under.</param>
    /// <returns>The element ID of the created RequirementUsage.</returns>
    public async Task<Guid> CreateRequirement(
        Guid projectId,
        string requirementName,
        string requirementText,
        string? reqId = null,
        Guid ownerPackageGuid = default)
    {
        var project = await _sysMLApiService.GetProjectAsync(projectId);
        var projectGuid = project.Id ?? throw new Exception("Project has no ID");
        var defaultBranchGuid = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch");

        var elementArgs = new Dictionary<string, JsonElement>
        {
            { "@type",                JsonSerializer.SerializeToElement("RequirementUsage") },
            { "name",                 JsonSerializer.SerializeToElement(requirementName) },
            { "declaredName",         JsonSerializer.SerializeToElement(requirementName) },
            { "shortName",            JsonSerializer.SerializeToElement<string?>(null) },
            { "declaredShortName",    JsonSerializer.SerializeToElement<string?>(null) },
            { "reqId",                JsonSerializer.SerializeToElement(reqId) },
            { "text",                 JsonSerializer.SerializeToElement(new[] { requirementText }) },
            { "aliasIds",             JsonSerializer.SerializeToElement(Array.Empty<string>()) },
            { "documentation",        JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedAnnotation",      JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedElement",         JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedMembership",      JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedRelationship",    JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "textualRepresentation",JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "assumedConstraint",    JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "requiredConstraint",   JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "actorParameter",       JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "stakeholderParameter", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "elementId",            JsonSerializer.SerializeToElement<string?>(null) },
            { "qualifiedName",        JsonSerializer.SerializeToElement<string?>(null) },
            { "isImpliedIncluded",    JsonSerializer.SerializeToElement<bool?>(null) },
            { "isLibraryElement",     JsonSerializer.SerializeToElement<bool?>(null) },
            { "owner",                JsonSerializer.SerializeToElement<object?>(null) },
            { "owningMembership",     JsonSerializer.SerializeToElement<object?>(null) },
            { "owningNamespace",      JsonSerializer.SerializeToElement<object?>(null) },
            { "owningRelationship",   JsonSerializer.SerializeToElement<object?>(null) },
        };

        if (ownerPackageGuid != default)
        {
            elementArgs["owner"] = JsonSerializer.SerializeToElement(new SysMLRef(ownerPackageGuid));
            elementArgs["owningNamespace"] = JsonSerializer.SerializeToElement(new SysMLRef(ownerPackageGuid));
        }

        var payload = JsonSerializer.SerializeToElement(elementArgs);
        var commitRequest = new CommitRequest
        {
            Change = [new DataVersionRequest { Payload = payload }]
        };

        var commitGuid = await _sysMLApiService.CommitToBranchAsync(projectGuid, defaultBranchGuid, commitRequest);

        var elements = await _sysMLApiService.GetElementsAsync(projectGuid, commitGuid);

        var created = elements.FirstOrDefault(e =>
                          string.Equals(e.GetName(), requirementName, StringComparison.OrdinalIgnoreCase)
                          && string.Equals(e.Type, "RequirementUsage", StringComparison.OrdinalIgnoreCase))
                      ?? elements.FirstOrDefault(e =>
                          string.Equals(e.Type, "RequirementUsage", StringComparison.OrdinalIgnoreCase));

        return created?.Id ?? commitGuid;
    }

    /// <summary>
    /// Creates a RequirementDefinition element and commits it to the project's default branch.
    /// </summary>
    /// <param name="projectId">Target project ID.</param>
    /// <param name="definitionName">Display name of the requirement definition.</param>
    /// <param name="definitionText">Requirement text / description.</param>
    /// <param name="reqId">Optional short identifier (e.g. "REQDEF-001").</param>
    /// <param name="isAbstract">Whether the definition is abstract.</param>
    /// <param name="ownerPackageGuid">Optional parent package to nest this definition under.</param>
    /// <returns>The element ID of the created RequirementDefinition.</returns>
    public async Task<Guid> CreateRequirementDefinition(
        Guid projectId,
        string definitionName,
        string definitionText,
        string? reqId = null,
        bool isAbstract = false,
        Guid ownerPackageGuid = default)
    {
        var project = await _sysMLApiService.GetProjectAsync(projectId);
        var projectGuid = project.Id ?? throw new Exception("Project has no ID");
        var defaultBranchGuid = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch");

        var elementArgs = new Dictionary<string, JsonElement>
        {
            { "@type",                JsonSerializer.SerializeToElement("RequirementDefinition") },
            { "name",                 JsonSerializer.SerializeToElement(definitionName) },
            { "declaredName",         JsonSerializer.SerializeToElement(definitionName) },
            { "shortName",            JsonSerializer.SerializeToElement<string?>(null) },
            { "declaredShortName",    JsonSerializer.SerializeToElement<string?>(null) },
            { "reqId",                JsonSerializer.SerializeToElement(reqId) },
            { "text",                 JsonSerializer.SerializeToElement(new[] { definitionText }) },
            { "isAbstract",           JsonSerializer.SerializeToElement(isAbstract) },
            { "isSufficient",         JsonSerializer.SerializeToElement(false) },
            { "aliasIds",             JsonSerializer.SerializeToElement(Array.Empty<string>()) },
            { "documentation",        JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedAnnotation",      JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedElement",         JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedMembership",      JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedRelationship",    JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "textualRepresentation",JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "assumedConstraint",    JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "requiredConstraint",   JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "actorParameter",       JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "stakeholderParameter", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "elementId",            JsonSerializer.SerializeToElement<string?>(null) },
            { "qualifiedName",        JsonSerializer.SerializeToElement<string?>(null) },
            { "isImpliedIncluded",    JsonSerializer.SerializeToElement<bool?>(null) },
            { "isLibraryElement",     JsonSerializer.SerializeToElement<bool?>(null) },
            { "owner",                JsonSerializer.SerializeToElement<object?>(null) },
            { "owningMembership",     JsonSerializer.SerializeToElement<object?>(null) },
            { "owningNamespace",      JsonSerializer.SerializeToElement<object?>(null) },
            { "owningRelationship",   JsonSerializer.SerializeToElement<object?>(null) },
        };

        if (ownerPackageGuid != default)
        {
            elementArgs["owner"] = JsonSerializer.SerializeToElement(new SysMLRef(ownerPackageGuid));
            elementArgs["owningNamespace"] = JsonSerializer.SerializeToElement(new SysMLRef(ownerPackageGuid));
        }

        var payload = JsonSerializer.SerializeToElement(elementArgs);
        var commitRequest = new CommitRequest
        {
            Change = [new DataVersionRequest { Payload = payload }]
        };

        var commitGuid = await _sysMLApiService.CommitToBranchAsync(projectGuid, defaultBranchGuid, commitRequest);

        var elements = await _sysMLApiService.GetElementsAsync(projectGuid, commitGuid);

        var created = elements.FirstOrDefault(e =>
                          string.Equals(e.GetName(), definitionName, StringComparison.OrdinalIgnoreCase)
                          && string.Equals(e.Type, "RequirementDefinition", StringComparison.OrdinalIgnoreCase))
                      ?? elements.FirstOrDefault(e =>
                          string.Equals(e.Type, "RequirementDefinition", StringComparison.OrdinalIgnoreCase));

        return created?.Id ?? commitGuid;
    }
}
