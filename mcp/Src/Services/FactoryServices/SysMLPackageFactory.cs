using System.Diagnostics;
using System.Text.Json;
using mcp.Src.Services;
using mcp.Src.Services.FactoryServices.Utils;
using Src.Services;
using SysMLV2.MCP.Models;

namespace MCP.Src.Services.FactoryServices;

public class SysMLPackageFactory
{
    private readonly ISysMLApiService _sysMLApiService;
    private readonly SysMLMetaModelFactory _sysMLMetaModelFactory;

    public SysMLPackageFactory(ISysMLApiService sysMLApiService, SysMLMetaModelFactory sysMLMetaModelFactory)
    {
        Debug.WriteLine("Package Creation Tool Initialized");
        _sysMLApiService = sysMLApiService;
        _sysMLMetaModelFactory = sysMLMetaModelFactory;
    }

    public async Task<SysMLElement?> GetPackageById(Guid projectId, Guid packageId)
    {
        return null;
    }

    public async Task<Guid> CreatePackage(Guid projectId, string packageName, string shortName = "", Guid ownerPackageGuid = default)
    {
        var project = await _sysMLApiService.GetProjectAsync(projectId);
        var projectGuid = project.Id ?? throw new Exception("Project has no ID");
        var defaultBranchGuid = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch");

        var elementType = _sysMLMetaModelFactory.GetSchemaProperties("Package")
            .FirstOrDefault(e => e.Key == "@type").Value?.Replace("const:", "") ?? "Package";

        var elementArgs = new Dictionary<string, JsonElement>
        {
            { "name", JsonSerializer.SerializeToElement(packageName) },
            { "declaredName", JsonSerializer.SerializeToElement(packageName) },
            { "@type", JsonSerializer.SerializeToElement(elementType) },
            { "shortName", JsonSerializer.SerializeToElement(shortName) },
            { "declaredShortName", JsonSerializer.SerializeToElement(shortName) },
            { "aliasIds", JsonSerializer.SerializeToElement(Array.Empty<string>()) },
            { "documentation", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "filterCondition", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "importedMembership", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "member", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "membership", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedAnnotation", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedElement", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedImport", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedMember", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedMembership", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "ownedRelationship", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "textualRepresentation", JsonSerializer.SerializeToElement(Array.Empty<object>()) },
            { "elementId", JsonSerializer.SerializeToElement<string?>(null) },
            { "qualifiedName", JsonSerializer.SerializeToElement<string?>(null) },
            { "isImpliedIncluded", JsonSerializer.SerializeToElement<bool?>(null) },
            { "isLibraryElement", JsonSerializer.SerializeToElement<bool?>(null) },
            { "owner", JsonSerializer.SerializeToElement<object?>(null) },
            { "owningMembership", JsonSerializer.SerializeToElement<object?>(null) },
            { "owningNamespace", JsonSerializer.SerializeToElement<object?>(null) },
            { "owningRelationship", JsonSerializer.SerializeToElement<object?>(null) }
        };

        if (ownerPackageGuid != default)
        {
            elementArgs["owner"] = JsonSerializer.SerializeToElement(new SysMLRef(ownerPackageGuid));
            elementArgs["owningNamespace"] = JsonSerializer.SerializeToElement(new SysMLRef(ownerPackageGuid));
        }

        var payload = JsonSerializer.SerializeToElement(elementArgs);
        var commitRequest = new CommitRequest
        {
            Change =
            [
                new DataVersionRequest { Payload = payload }
            ]
        };

        var commitGuid = await _sysMLApiService.CommitToBranchAsync(projectGuid, defaultBranchGuid, commitRequest);

        var existingElements = await _sysMLApiService.GetElementsAsync(projectGuid, commitGuid);

        var createdElement = existingElements.FirstOrDefault(element =>
                                string.Equals(element.GetName(), packageName, StringComparison.OrdinalIgnoreCase))
                            ?? existingElements.FirstOrDefault(element =>
                                string.Equals(element.Type, "Package", StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(GetJsonString(element, "declaredName") ?? GetJsonString(element, "name"), packageName, StringComparison.OrdinalIgnoreCase))
                            ?? existingElements.FirstOrDefault(element =>
                                string.Equals(element.Type, "Package", StringComparison.OrdinalIgnoreCase));

        return createdElement?.Id ?? commitGuid;
    }

    private static string? GetJsonString(SysMLElement element, string propertyName)
    {
        if (element.AdditionalProperties != null &&
            element.AdditionalProperties.TryGetValue(propertyName, out var jsonElement) &&
            jsonElement.ValueKind == JsonValueKind.String)
        {
            return jsonElement.GetString();
        }
        return null;
    }
}
