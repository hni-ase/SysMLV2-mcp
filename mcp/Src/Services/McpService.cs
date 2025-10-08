using Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace Services
{
    public class McpService
    {
        private readonly ILogger<McpService>? _logger;

        public McpService(ILogger<McpService>? logger = null)
        {
            _logger = logger;
        }

        public void ExecuteTool(AbstractTool tool)
        {
            _logger?.LogInformation("Executing tool: {ToolName}", tool.Name);
            Console.WriteLine($"Executing tool: {tool.Name}");
            tool.HandleOperation(null);
        }

        // New: Process incoming MCP data (called from controller)
        public void ProcessIncomingData(object data)
        {
            _logger?.LogInformation("Processing incoming MCP data: {Data}", data);
            Console.WriteLine($"Processing incoming MCP data: {data}");

            // Try to use ModelContextProtocol.Server API if available (reflection to avoid hard dependency issues)
            try
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "ModelContextProtocol" || a.GetName().Name == "ModelContextProtocol.Core");

                if (assembly != null)
                {
                    // Look for a type that could represent a server or processor
                    var serverType = assembly.GetType("ModelContextProtocol.Server.ModelContextServer")
                                  ?? assembly.GetType("ModelContextProtocol.Server.ModelContext")
                                  ?? assembly.GetTypes().FirstOrDefault(t => t.Name.Contains("ModelContext") || t.Name.Contains("Mcp"));

                    if (serverType != null)
                    {
                        _logger?.LogInformation("Found MCP type: {Type}", serverType.FullName);

                        // If there's a static or parameterless constructor available, try to create an instance
                        object? instance = null;
                        try
                        {
                            instance = Activator.CreateInstance(serverType);
                        }
                        catch { /* ignore */ }

                        // Try to find a method to handle incoming data
                        var handleMethod = serverType.GetMethod("HandleIncomingMessage")
                                        ?? serverType.GetMethod("Receive")
                                        ?? serverType.GetMethod("Process")
                                        ?? serverType.GetMethods().FirstOrDefault(m => m.GetParameters().Length == 1);

                        if (handleMethod != null)
                        {
                            try
                            {
                                handleMethod.Invoke(instance, new[] { data });
                                _logger?.LogInformation("Forwarded data to MCP type {Type}", serverType.FullName);
                            }
                            catch (Exception ex)
                            {
                                _logger?.LogWarning(ex, "Failed to invoke handler on MCP type {Type}", serverType.FullName);
                            }
                        }
                        else
                        {
                            _logger?.LogInformation("No suitable handler method found on MCP type {Type}", serverType.FullName);
                        }
                    }
                    else
                    {
                        _logger?.LogInformation("No ModelContextProtocol server type found in loaded assemblies.");
                    }
                }
                else
                {
                    _logger?.LogInformation("ModelContextProtocol assembly not loaded; skipping protocol forwarding.");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while attempting to forward data to ModelContextProtocol types");
            }
        }
    }
}