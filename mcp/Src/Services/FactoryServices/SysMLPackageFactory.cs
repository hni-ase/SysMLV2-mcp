using System.Diagnostics;
using System.Text.Json;
using mcp.Src.Services;
using mcp.Src.Services.FactoryServices.Utils;
using Org.OpenAPITools.Model;
using Src.Services;

namespace MCP.Src.Services.FactoryServices;

public class SysMLPackageFactory
{

    ISysMLApiService _sysMLApiService;

    SysMLMetaModelFactory _sysMLMetaModelFactory;


    public SysMLPackageFactory(ISysMLApiService sysMLApiService, SysMLMetaModelFactory sysMLMetaModelFactory)
    {
        Debug.WriteLine("Package Creation Tool Initialized");
        _sysMLApiService = sysMLApiService;
        _sysMLMetaModelFactory = sysMLMetaModelFactory;
    }

    public async Task<Element> GetPackageById(Guid projectId, Guid packageId)
    {
        return null;
    }

    public async Task<Guid> CreatePackage(Guid projectId, string packageName, string shortName = "", Guid ownerPackageGuid = default)
    {
        // first we need to get the project with the ID
        var project = _sysMLApiService.GetProjectAsync(projectId).GetAwaiter().GetResult();
        var projectGuid = project.Id ?? throw new Exception("Project has no ID");
        // now we can create the package element on the default branch
        var defaultBranchGuid = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch");
        // get the branch so we can get the head commit
        var branch = _sysMLApiService.GetBranchAsync(projectGuid, defaultBranchGuid).GetAwaiter().GetResult();
        var elementType = _sysMLMetaModelFactory.GetSchemaProperties("Package").FirstOrDefault(e => e.Key == "@type").Value.Replace("const:", "");

        // first we need to define the payload params
        var elementArgs = new Dictionary<string, JsonElement>
        {
            { "name", JsonSerializer.SerializeToElement(packageName) },
            { "@type", JsonSerializer.SerializeToElement(elementType) },
            {"shortName", JsonSerializer.SerializeToElement(shortName)}
        };
        if (ownerPackageGuid != default)
        {
            // create a JsonElement with the id 
            var identifiedJsonElement = JsonSerializer.SerializeToElement(new { @id = ownerPackageGuid.ToString() });
            elementArgs.Add("owner", identifiedJsonElement);
        }
        var commitGuid = await _sysMLApiService.CommitToBranchAsync(projectGuid, defaultBranchGuid,
            new Commit
            {
                Type = Commit.TypeEnum.Commit,
                Change =
                [
                    new DataVersion
                    {
                        Type = DataVersion.TypeEnum.DataVersion,
                        Payload = new Data
                        {
                            AdditionalProperties = elementArgs
                        }
                    }
                ]
            });

        // Now we'll get the actual package and try to deserialize it with the json schema
        // we need to query the elements we just created and get the one from the new commit 
        var existingElements = await _sysMLApiService.GetElementsAsync(projectGuid, commitGuid);
        var createdElement = existingElements.FirstOrDefault(element => element.GetName() == packageName);
        return createdElement.Id ?? throw new Exception($"Created package with name {packageName} could not be found");
    }

}