using System.Diagnostics;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace SysMLv2Mcp.Tests;

[Trait("Category", "Integration")]
public class McpServerEndToEndTests
{
    private static string ServerProjectPath =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "mcp", "SysMLv2Mcp.Tools.csproj"));

    private static string SchemasPath =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "sysmlv2-api-spec", "metamodels"));

    [Fact]
    public async Task Client_CanInitialize_And_ListTools_FromRealServer()
    {
        await using var client = await LaunchClientAsync();

        var tools = await client.ListToolsAsync();

        Assert.NotEmpty(tools);
        var toolNames = tools.Select(t => t.Name).ToHashSet();
        Assert.Contains("get_projects", toolNames);
        Assert.Contains("describe_type_schema", toolNames);
        Assert.Contains("create_requirement", toolNames);
        Assert.Contains("create_block_definition", toolNames);
        Assert.Contains("create_package", toolNames);
    }

    [Fact]
    public async Task Client_CanCallTool_DescribeTypeSchema_FromRealServer()
    {
        await using var client = await LaunchClientAsync();

        var result = await client.CallToolAsync("describe_type_schema",
            new Dictionary<string, object?> { ["elementType"] = "ItemDefinition" });

        Assert.NotEmpty(result.Content);
        var text = result.Content!
            .Select(c => (c as TextContentBlock)?.Text)
            .FirstOrDefault(t => !string.IsNullOrEmpty(t));
        Assert.NotNull(text);
        Assert.Contains("ItemDefinition", text);
        Assert.Contains("requiredAttributes", text);
    }

    [Fact]
    public async Task Client_CallTool_UnknownTool_ThrowsMcpException_FromRealServer()
    {
        await using var client = await LaunchClientAsync();

        await Assert.ThrowsAsync<McpException>(async () =>
            await client.CallToolAsync("nonexistent_tool", new Dictionary<string, object?>()));
    }

    private static async Task<McpClient> LaunchClientAsync()
    {
        Assert.True(File.Exists(ServerProjectPath),
            $"Server project not found at {ServerProjectPath}. BaseDirectory={AppContext.BaseDirectory}");
        Assert.True(Directory.Exists(SchemasPath),
            $"Schemas not found at {SchemasPath}. BaseDirectory={AppContext.BaseDirectory}");

        var transportOptions = new StdioClientTransportOptions
        {
            Command = "dotnet",
            Name = "sysmlv2-mcp-e2e",
            Arguments = new List<string> { "run", "--project", ServerProjectPath, "--no-build" },
            WorkingDirectory = Path.GetDirectoryName(ServerProjectPath)!
        };
        var transport = new StdioClientTransport(transportOptions);

        var clientOptions = new McpClientOptions
        {
            ClientInfo = new Implementation { Name = "e2e-test-client", Version = "1.0.0" },
            ProtocolVersion = "2024-11-05"
        };

        return await McpClient.CreateAsync(transport, clientOptions);
    }
}