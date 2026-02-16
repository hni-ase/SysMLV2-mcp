using mcp.Src.Services;
using MCP.Src.Services.FactoryServices;
using Src.Services;

namespace Tools
{
    public abstract class AbstractToolSet(ISysMLApiService sysMLApiService, SysMLMetaModelFactory metamodelFactory, string toolSetName)
    {
        protected string _name { get; } = toolSetName;



        protected ISysMLApiService _sysMLApiService { get; } = sysMLApiService;

        protected SysMLMetaModelFactory _metamodelFactory { get; } = metamodelFactory;

        public abstract Guid PerformOperation(object input);

    }
}