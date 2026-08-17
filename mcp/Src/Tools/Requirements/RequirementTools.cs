using System.ComponentModel;
using MCP.Src.Services.FactoryServices;
using ModelContextProtocol.Server;
using Src.Services;
using SysMLV2.MCP.Models;

namespace Tools.Requirements;

[McpServerToolType]
public class RequirementTools
{
    private readonly ISysMLApiService _api;
    private readonly SysMLRequirementFactory _factory;
    private readonly SysMLUseCaseFactory _useCaseFactory;
    private readonly ProjectContextResolver _projectContext;

    public RequirementTools(
        ISysMLApiService api,
        SysMLRequirementFactory factory,
        SysMLUseCaseFactory useCaseFactory,
        ProjectContextResolver projectContext)
    {
        _api = api;
        _factory = factory;
        _useCaseFactory = useCaseFactory;
        _projectContext = projectContext;
    }

    [McpServerTool, Description("Creates a UseCaseUsage element in the specified project. Optionally links an objective RequirementUsage and nests under a parent package.")]
    public UseCaseLLMInformation CreateUseCase(
        string projectName,
        string useCaseName,
        Guid objectiveRequirementId = default,
        Guid parentPackageGuid = default)
    {
        var project = _projectContext.FindProjectByName(projectName);
        var (elementId, projectId) = _useCaseFactory.CreateUseCase(
            project.Id!.Value,
            useCaseName,
            objectiveRequirementId,
            parentPackageGuid)
            .GetAwaiter().GetResult();
        return new UseCaseLLMInformation(elementId, projectId, useCaseName);
    }

    [McpServerTool, Description("Creates a RequirementUsage element in the specified project. Optionally nested under a parent package.")]
    public Guid CreateRequirement(
        string projectName,
        string requirementName,
        string requirementText,
        string? reqId = null,
        Guid parentPackageGuid = default)
    {
        var project = _projectContext.FindProjectByName(projectName);
        return _factory.CreateRequirement(
            project.Id!.Value,
            requirementName,
            requirementText,
            reqId,
            parentPackageGuid)
            .GetAwaiter().GetResult();
    }

    [McpServerTool, Description("Creates a RequirementDefinition element in the specified project. Optionally nested under a parent package.")]
    public Guid CreateRequirementDefinition(
        string projectName,
        string definitionName,
        string definitionText,
        string? reqId = null,
        bool isAbstract = false,
        Guid parentPackageGuid = default)
    {
        var project = _projectContext.FindProjectByName(projectName);
        return _factory.CreateRequirementDefinition(
            project.Id!.Value,
            definitionName,
            definitionText,
            reqId,
            isAbstract,
            parentPackageGuid)
            .GetAwaiter().GetResult();
    }

    [McpServerTool, Description("Adds a subject parameter (SubjectMembership + ReferenceUsage) to an existing RequirementUsage or RequirementDefinition. Returns the element ID of the created subject ReferenceUsage.")]
    public Guid AddSubjectToRequirement(string projectName, Guid requirementId, string subjectName)
    {
        var project = _projectContext.FindProjectByName(projectName);
        return _factory.AddSubjectToRequirement(project.Id!.Value, requirementId, subjectName)
            .GetAwaiter().GetResult();
    }

    [McpServerTool, Description("Types a RequirementUsage against a RequirementDefinition by setting the requirementDefinition field. Fetches the current element state and re-commits with the updated field.")]
    public void SetRequirementDefinition(string projectName, Guid requirementUsageId, Guid requirementDefinitionId)
    {
        var project = _projectContext.FindProjectByName(projectName);
        _factory.SetRequirementDefinition(project.Id!.Value, requirementUsageId, requirementDefinitionId)
            .GetAwaiter().GetResult();
    }
}