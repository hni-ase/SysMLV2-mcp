using System.ComponentModel;
using mcp.Src.Services;
using ModelContextProtocol.Server;
using Src.Services;
using SysMLV2.MCP.Models;

namespace Tools.Schema;

[McpServerToolType]
public class SchemaTools
{
    private readonly ISysMLApiService _api;
    private readonly SysMLMetaModelFactory _metamodelFactory;
    private readonly ProjectContextResolver _projectContext;

    public SchemaTools(
        ISysMLApiService api,
        SysMLMetaModelFactory metamodelFactory,
        ProjectContextResolver projectContext)
    {
        _api = api;
        _metamodelFactory = metamodelFactory;
        _projectContext = projectContext;
    }

    [McpServerTool, Description("Returns all schema-defined attributes for a given SysML V2 element type, split into required and optional, each mapped to its JSON Schema type description. Use this when you know the type name but do not have an element ID. If the type name is not found an error listing all available types is thrown.")]
    public ElementSchemaInfo DescribeTypeSchema(string elementType)
    {
        var availableTypes = _metamodelFactory.GetAvailableSchemas().OrderBy(x => x).ToList();
        if (!availableTypes.Contains(elementType))
            throw new ArgumentException(
                $"Unknown element type '{elementType}'. Available types: {string.Join(", ", availableTypes)}");

        var (required, allProperties) = _metamodelFactory.GetTypeAttributeInfo(elementType);

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

    [McpServerTool, Description("Fetches an element by ID and returns its SysML V2 type together with all schema-defined attributes split into required and optional, each mapped to its JSON Schema type description. Use this before UpdateElementAttributes to discover valid attribute names.")]
    public ElementSchemaInfo DescribeElementSchema(string projectName, Guid elementId)
    {
        var project = _projectContext.FindProjectByName(projectName);
        var (projectId, headCommitId) = _projectContext.GetProjectAndHeadCommitOrDefault(project);
        if (headCommitId == Guid.Empty)
            throw new Exception($"Project '{projectName}' has no commits yet.");

        var element = _api.GetElementByIdAsync(projectId, headCommitId, elementId).GetAwaiter().GetResult();
        var type = element.Type ?? throw new Exception($"Element '{elementId}' has no @type.");

        var (required, allProperties) = _metamodelFactory.GetTypeAttributeInfo(type);

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
}