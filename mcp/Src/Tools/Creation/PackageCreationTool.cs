using mcp.Src.Services;
using Src.Services;

namespace Tools.Creation
{

    public class PackageCreationHandler : AbstractToolSet
    {
        public PackageCreationHandler(ISysMLApiService sysMLApiService, SysMLMetaModelFactory metamodelFactory, string toolSetName) : base(sysMLApiService, metamodelFactory, toolSetName)
        {
        }

        public override Guid PerformOperation(object input)
        {
            // In this case we need the operation to be a 
            return Guid.Empty;
        }
    }
}