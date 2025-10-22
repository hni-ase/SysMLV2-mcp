using System.Diagnostics;
using mcp.Src.Models;
using mcp.Src.Services;
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

    public Guid CreatePackage(Guid projectId, string packageName, string documentation)
    {
        // first we need to get the project with the ID
        var project = _sysMLApiService.GetProjectAsync(projectId).GetAwaiter().GetResult();
        var projectGuid = project.Id ?? throw new Exception("Project has no ID");
        // now we can create the package element on the default branch
        var defaultBranchGuid = project.DefaultBranch?.Id ?? throw new Exception("Project has no default branch");
        // get the branch so we can get the head commit
        var branch = _sysMLApiService.GetBranchAsync(projectGuid, defaultBranchGuid).GetAwaiter().GetResult();
        var schemaProperties = _sysMLMetaModelFactory.GetSchemaProperties("Package").FirstOrDefault(e => e.Key == "@type").Value;

        var retval = _sysMLApiService.CommitToBranchAsync(projectGuid , defaultBranchGuid,
            new Commit
            {
                Type = Commit.TypeEnum.Commit,
                Change =
                [
                    new DataVersion
                    {
                        Type = DataVersion.TypeEnum.DataVersion,
                        Payload = new Package
                        {
                            Name = packageName,
                            Documentation = documentation
                        }
                        
                    }
                ]
            }).GetAwaiter().GetResult();

        return retval;
    }
}