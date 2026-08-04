using System.IO;
using System.Text.Json;
using MCP.Src.Services.FactoryServices;
using mcp.Src.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Src.Services;

const string SYSML_DATABASE_CLIENT_NAME = "SysMLV2-Database-Client";
const string SYSML_DATABSE_SERVER_URL = "http://localhost:9000";

var builder = WebApplication.CreateBuilder(args);
builder.Logging.
    AddConsole(consoleLogOptions =>
    {
        consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Error;
    });

var mode = ParseMode(args);

var mcpBuilder = builder.Services.AddMcpServer().WithToolsFromAssembly();
if (mode == "stdio")
{
    mcpBuilder.WithStdioServerTransport();
}
// In http mode the McpServerOptions (with tools) is registered by AddMcpServer
// but no transport is wired here; the /mcp endpoint below drives it per request.

builder.Services.AddHttpClient(
    SYSML_DATABASE_CLIENT_NAME,
    client =>
    {
        client.BaseAddress = new Uri(SYSML_DATABSE_SERVER_URL);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
    });
builder.Services.AddSingleton<ISysMLApiService, SysMLApiService>();
builder.Services.AddSingleton(new SysMLMetaModelFactory(ResolveSchemasPath(builder.Environment.ContentRootPath)));

builder.Services.AddSingleton<ProjectContextResolver>();
builder.Services.AddSingleton<SysMLPackageFactory>();
builder.Services.AddSingleton<SysMLRequirementFactory>();
builder.Services.AddSingleton<SysMLUseCaseFactory>();
builder.Services.AddSingleton<ElementMutationService>();

var app = builder.Build();

if (mode == "http")
{
    const string McpEndpoint = "/mcp";

    app.MapPost(McpEndpoint, async (HttpContext context, IServiceProvider sp) =>
    {
        var options = sp.GetRequiredService<IOptions<McpServerOptions>>().Value;
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        JsonRpcMessage? request;
        try
        {
            request = await JsonSerializer.DeserializeAsync<JsonRpcMessage>(context.Request.Body);
        }
        catch (JsonException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        if (request is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var transport = new StreamableHttpServerTransport
        {
            Stateless = true
        };

        var server = McpServer.Create(transport, options, loggerFactory, sp);
        var runTask = Task.Run(() => server.RunAsync(context.RequestAborted));

        context.Response.ContentType = "application/json";
        context.Response.Headers["Mcp-Session-Id"] = transport.SessionId;

        try
        {
            await transport.HandlePostRequest(request, context.Response.Body, context.RequestAborted);
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await server.DisposeAsync();
            return;
        }

        await transport.DisposeAsync();
        await server.DisposeAsync();
        if (runTask.IsCompleted)
        {
            try { await runTask; } catch { }
        }
    });

    app.MapGet(McpEndpoint, () => Results.StatusCode(StatusCodes.Status405MethodNotAllowed));
    app.MapDelete(McpEndpoint, () => Results.StatusCode(StatusCodes.Status202Accepted));

    Console.WriteLine($"SysMLv2Mcp HTTP server listening. POST to http://localhost:5000{McpEndpoint}");
}

await app.RunAsync();

static string ResolveSchemasPath(string contentRootPath)
{
    var candidates = new[]
    {
        Path.Combine(contentRootPath, "..", "sysmlv2-api-spec", "metamodels"),
        Path.Combine(contentRootPath, "sysmlv2-api-spec", "metamodels"),
        Path.Combine(Directory.GetCurrentDirectory(), "sysmlv2-api-spec", "metamodels"),
        Path.Combine(Directory.GetCurrentDirectory(), "..", "sysmlv2-api-spec", "metamodels")
    };

    var found = candidates
        .Select(Path.GetFullPath)
        .FirstOrDefault(Directory.Exists);

    return found ?? Path.GetFullPath(Path.Combine(contentRootPath, "..", "sysmlv2-api-spec", "metamodels"));
}

static string ParseMode(string[] args)
{
    for (var i = 0; i < args.Length - 1; i++)
    {
        if (args[i] == "--mode")
            return args[i + 1].ToLowerInvariant();
    }
    foreach (var a in args)
    {
        if (a.StartsWith("--mode=", StringComparison.Ordinal))
            return a["--mode=".Length..].ToLowerInvariant();
    }
    return "stdio";
}