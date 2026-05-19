using System.Text.Json;
using mcp.Src.Services;
using Src.Services;
using SysMLV2.MCP.Models;

namespace MCP.Src.Services.FactoryServices;

public class SysMLUseCaseFactory
{
    private readonly ISysMLApiService _sysMLApiService;

    public SysMLUseCaseFactory(ISysMLApiService sysMLApiService)
    {
        _sysMLApiService = sysMLApiService;
    }

    /// <summary>
    /// Creates a UseCaseUsage element and commits it to the project's default branch.
    /// </summary>
    /// <param name="projectId">Target project ID.</param>
    /// <param name="useCaseName">Display name of the use case.</param>
    /// <param name="objectiveRequirementId">Optional ID of a RequirementUsage to link as the objective.</param>
    /// <param name="ownerPackageGuid">Optional parent package to nest this use case under.</param>
    /// <returns>The element ID of the created UseCaseUsage, and the project ID.</returns>
    public async Task<(Guid ElementId, Guid ProjectId)> CreateUseCase(
        Guid projectId,
        string useCaseName,
        Guid objectiveRequirementId = default,
        Guid ownerPackageGuid = default)
    {
        var project = await _sysMLApiService.GetProjectAsync(projectId);
        var projectGuid = project.Id ?? throw new Exception("Project has no ID");
        var defaultBranchGuid = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch");

        var elementArgs = new Dictionary<string, JsonElement>
        {
            { "@type",                JsonSerializer.SerializeToElement("UseCaseUsage") },
            { "name",                 JsonSerializer.SerializeToElement(useCaseName) },
            { "declaredName",         JsonSerializer.SerializeToElement(useCaseName) },
            { "shortName",            JsonSerializer.SerializeToElement<string?>(null) },
            { "declaredShortName",    JsonSerializer.SerializeToElement<string?>(null) },
            { "useCaseDefinition",    JsonSerializer.SerializeToElement<object?>(null) },
            { "objectiveRequirement", JsonSerializer.SerializeToElement<object?>(null) },
            { "includedUseCase",      JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "actorParameter",       JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "aliasIds",             JsonSerializer.SerializeToElement(Array.Empty<string>()) },
            { "documentation",        JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedAnnotation",      JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedElement",         JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedMembership",      JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedRelationship",    JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "textualRepresentation",JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "elementId",            JsonSerializer.SerializeToElement<string?>(null) },
            { "qualifiedName",        JsonSerializer.SerializeToElement<string?>(null) },
            { "isImpliedIncluded",    JsonSerializer.SerializeToElement<bool?>(null) },
            { "isLibraryElement",     JsonSerializer.SerializeToElement<bool?>(null) },
            { "owner",                JsonSerializer.SerializeToElement<object?>(null) },
            { "owningMembership",     JsonSerializer.SerializeToElement<object?>(null) },
            { "owningNamespace",      JsonSerializer.SerializeToElement<object?>(null) },
            { "owningRelationship",   JsonSerializer.SerializeToElement<object?>(null) },
        };

        if (objectiveRequirementId != default)
            elementArgs["objectiveRequirement"] = JsonSerializer.SerializeToElement(new SysMLRef(objectiveRequirementId));

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
                          string.Equals(e.GetName(), useCaseName, StringComparison.OrdinalIgnoreCase)
                          && string.Equals(e.Type, "UseCaseUsage", StringComparison.OrdinalIgnoreCase))
                      ?? elements.FirstOrDefault(e =>
                          string.Equals(e.Type, "UseCaseUsage", StringComparison.OrdinalIgnoreCase));

        return (created?.Id ?? commitGuid, projectGuid);
    }
}
