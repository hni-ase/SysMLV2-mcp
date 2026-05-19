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

    /// <summary>
    /// Updates an existing RequirementUsage so its requirementDefinition points to the given
    /// RequirementDefinition element. Reads the current element state, overrides the field,
    /// and submits a new commit.
    /// </summary>
    public async Task SetRequirementDefinition(
        Guid projectId,
        Guid requirementId,
        Guid definitionId)
    {
        var project = await _sysMLApiService.GetProjectAsync(projectId);
        var projectGuid = project.Id ?? throw new Exception("Project has no ID");
        var defaultBranchGuid = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch");

        // Resolve head commit to read current element state
        var branch = await _sysMLApiService.GetBranchAsync(projectGuid, defaultBranchGuid);
        var headCommitId = branch.Head?.Id ?? throw new Exception("Branch has no head commit");

        var element = await _sysMLApiService.GetElementByIdAsync(projectGuid, headCommitId, requirementId);

        // Reconstruct full payload from the element, then override requirementDefinition
        var payload = new Dictionary<string, JsonElement>
        {
            ["@id"]   = JsonSerializer.SerializeToElement(element.Id!.Value),
            ["@type"] = JsonSerializer.SerializeToElement(element.Type!)
        };

        if (element.AdditionalProperties != null)
            foreach (var kvp in element.AdditionalProperties)
                payload[kvp.Key] = kvp.Value;

        payload["requirementDefinition"] = JsonSerializer.SerializeToElement(new SysMLRef(definitionId));

        var commitRequest = new CommitRequest
        {
            Change =
            [
                new DataVersionRequest
                {
                    Identity = new SysMLRef(requirementId),
                    Payload  = JsonSerializer.SerializeToElement(payload)
                }
            ]
        };

        await _sysMLApiService.CommitToBranchAsync(projectGuid, defaultBranchGuid, commitRequest);
    }

    /// <summary>
    /// Adds a subject (SubjectMembership + ReferenceUsage) to an existing RequirementUsage or
    /// RequirementDefinition in a single commit.
    /// </summary>
    /// <param name="projectId">Target project ID.</param>
    /// <param name="requirementId">The requirement element to add the subject to.</param>
    /// <param name="subjectName">The name of the subject (e.g. "vehicle").</param>
    /// <returns>The element ID of the created ReferenceUsage subject parameter.</returns>
    public async Task<Guid> AddSubjectToRequirement(
        Guid projectId,
        Guid requirementId,
        string subjectName)
    {
        var project = await _sysMLApiService.GetProjectAsync(projectId);
        var projectGuid = project.Id ?? throw new Exception("Project has no ID");
        var defaultBranchGuid = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch");

        // Pre-generate IDs so both elements can cross-reference each other in one commit.
        var subjectGuid = Guid.NewGuid();
        var membershipGuid = Guid.NewGuid();

        // ReferenceUsage — the actual subject parameter element
        var subjectPayload = new Dictionary<string, JsonElement>
        {
            { "@type",                JsonSerializer.SerializeToElement("ReferenceUsage") },
            { "@id",                  JsonSerializer.SerializeToElement(subjectGuid.ToString()) },
            { "name",                 JsonSerializer.SerializeToElement(subjectName) },
            { "declaredName",         JsonSerializer.SerializeToElement(subjectName) },
            { "direction",            JsonSerializer.SerializeToElement("in") },
            { "isComposite",          JsonSerializer.SerializeToElement(false) },
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
            { "owner",                JsonSerializer.SerializeToElement(new SysMLRef(requirementId)) },
            { "owningNamespace",      JsonSerializer.SerializeToElement(new SysMLRef(requirementId)) },
            { "owningMembership",     JsonSerializer.SerializeToElement(new SysMLRef(membershipGuid)) },
            { "owningRelationship",   JsonSerializer.SerializeToElement(new SysMLRef(membershipGuid)) },
        };

        // SubjectMembership — the relationship connecting the requirement to the subject
        var membershipPayload = new Dictionary<string, JsonElement>
        {
            { "@type",                JsonSerializer.SerializeToElement("SubjectMembership") },
            { "@id",                  JsonSerializer.SerializeToElement(membershipGuid.ToString()) },
            { "ownedMemberElement",   JsonSerializer.SerializeToElement(new SysMLRef(subjectGuid)) },
            { "owningRelatedElement", JsonSerializer.SerializeToElement(new SysMLRef(requirementId)) },
            { "aliasIds",             JsonSerializer.SerializeToElement(Array.Empty<string>()) },
            { "documentation",        JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedAnnotation",      JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedElement",         JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedRelatedElement",  JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedRelationship",    JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "elementId",            JsonSerializer.SerializeToElement<string?>(null) },
            { "qualifiedName",        JsonSerializer.SerializeToElement<string?>(null) },
            { "isImplied",            JsonSerializer.SerializeToElement<bool?>(null) },
            { "isImpliedIncluded",    JsonSerializer.SerializeToElement<bool?>(null) },
            { "isLibraryElement",     JsonSerializer.SerializeToElement<bool?>(null) },
            { "owner",                JsonSerializer.SerializeToElement(new SysMLRef(requirementId)) },
            { "owningRelationship",   JsonSerializer.SerializeToElement<object?>(null) },
        };

        var commitRequest = new CommitRequest
        {
            Change =
            [
                new DataVersionRequest
                {
                    Identity = new SysMLRef(membershipGuid),
                    Payload  = JsonSerializer.SerializeToElement(membershipPayload)
                },
                new DataVersionRequest
                {
                    Identity = new SysMLRef(subjectGuid),
                    Payload  = JsonSerializer.SerializeToElement(subjectPayload)
                }
            ]
        };

        await _sysMLApiService.CommitToBranchAsync(projectGuid, defaultBranchGuid, commitRequest);
        return subjectGuid;
    }
}
