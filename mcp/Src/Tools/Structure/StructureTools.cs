using System.ComponentModel;
using ModelContextProtocol.Server;
using Src.Services;
using SysMLV2.MCP.Models;

namespace Tools.Structure;

[McpServerToolType]
public class StructureTools
{
    private readonly ElementMutationService _mutation;

    public StructureTools(ElementMutationService mutation)
    {
        _mutation = mutation;
    }

    [McpServerTool, Description("Creates a signal definition element in the specified project. SysML v2 metamodel mapping: SignalDefinition -> ItemDefinition.")]
    public ElementCreationResult CreateSignalDefinition(string projectName, string signalName, string? parentElementId = null)
        => _mutation.CreateNamedElementOfType(projectName, "ItemDefinition", signalName, parentElementId);

    [McpServerTool, Description("Creates a signal usage element in the specified project. SysML v2 metamodel mapping: SignalUsage -> ItemUsage.")]
    public ElementCreationResult CreateSignal(string projectName, string signalName, string? parentElementId = null)
        => _mutation.CreateNamedElementOfType(projectName, "ItemUsage", signalName, parentElementId);

    [McpServerTool, Description("Creates a block definition element in the specified project. SysML v2 metamodel mapping: BlockDefinition -> PartDefinition.")]
    public ElementCreationResult CreateBlockDefinition(string projectName, string blockDefinitionName, string? parentElementId = null)
        => _mutation.CreateNamedElementOfType(projectName, "PartDefinition", blockDefinitionName, parentElementId);

    [McpServerTool, Description("Creates a part/block usage element in the specified project. SysML v2 metamodel mapping: BlockUsage -> PartUsage.")]
    public ElementCreationResult CreatePart(string projectName, string partName, string? parentElementId = null)
        => _mutation.CreateNamedElementOfType(projectName, "PartUsage", partName, parentElementId);

    [McpServerTool, Description("Creates an interface definition element in the specified project.")]
    public ElementCreationResult CreateInterfaceDefinition(string projectName, string interfaceDefinitionName, string? parentElementId = null)
        => _mutation.CreateNamedElementOfType(projectName, "InterfaceDefinition", interfaceDefinitionName, parentElementId);

    [McpServerTool, Description("Creates an interface usage element in the specified project.")]
    public ElementCreationResult CreateInterface(string projectName, string interfaceName, string? parentElementId = null)
        => _mutation.CreateNamedElementOfType(projectName, "InterfaceUsage", interfaceName, parentElementId);

    [McpServerTool, Description("Updates attributes of a signal definition element (mapped to ItemDefinition) in the specified project.")]
    public ElementUpdateResult UpdateSignalDefinition(string projectName, Guid signalDefinitionId, string attributesJson)
        => _mutation.UpdateElementAttributes(projectName, signalDefinitionId, attributesJson);

    [McpServerTool, Description("Updates attributes of a signal usage element (mapped to ItemUsage) in the specified project.")]
    public ElementUpdateResult UpdateSignal(string projectName, Guid signalId, string attributesJson)
        => _mutation.UpdateElementAttributes(projectName, signalId, attributesJson);

    [McpServerTool, Description("Updates attributes of a block definition element (mapped to PartDefinition) in the specified project.")]
    public ElementUpdateResult UpdateBlockDefinition(string projectName, Guid blockDefinitionId, string attributesJson)
        => _mutation.UpdateElementAttributes(projectName, blockDefinitionId, attributesJson);

    [McpServerTool, Description("Updates attributes of a part/block usage element (mapped to PartUsage) in the specified project.")]
    public ElementUpdateResult UpdatePart(string projectName, Guid partId, string attributesJson)
        => _mutation.UpdateElementAttributes(projectName, partId, attributesJson);

    [McpServerTool, Description("Updates attributes of an interface definition element in the specified project.")]
    public ElementUpdateResult UpdateInterfaceDefinition(string projectName, Guid interfaceDefinitionId, string attributesJson)
        => _mutation.UpdateElementAttributes(projectName, interfaceDefinitionId, attributesJson);

    [McpServerTool, Description("Updates attributes of an interface usage element in the specified project.")]
    public ElementUpdateResult UpdateInterface(string projectName, Guid interfaceId, string attributesJson)
        => _mutation.UpdateElementAttributes(projectName, interfaceId, attributesJson);
}