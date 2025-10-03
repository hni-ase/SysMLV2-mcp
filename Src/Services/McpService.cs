using Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
namespace Services
{
    public class McpService
    {
        public void ExecuteTool(AbstractTool tool)
        {
            System.Console.WriteLine($"Executing tool: {tool.Name}");
            tool.HandleOperation(null);
        }
    }
}