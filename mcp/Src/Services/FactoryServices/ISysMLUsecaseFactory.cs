namespace MCP.Src.Services.FactoryServices;
public interface ISysMLUsecaseFactory
{
    Guid CreateUsecase(string projectId, string usecaseName, string documenation, string actorId = null);
    Guid CreateActor(string projectId, string actorName, string documenation);
    Guid CreateAssociation(string projectId, string sourceElementId, string targetElementId, string associationName);
}