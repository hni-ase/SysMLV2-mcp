using mcp.Src.Services;
using Microsoft.Extensions.DependencyInjection;
using Src.Services;
using System.IO;



const string SYSML_DATABASE_CLIENT_NAME = "SysMLV2-Database-Client";
const string SYSML_DATABSE_SERVER_URL = "http://localhost:9000";

var builder = WebApplication.CreateBuilder(args);
builder.Logging.
    AddConsole(consoleLogOptions =>
    {
        consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Error;
    });


builder.Services
    .AddMcpServer()
    .WithToolsFromAssembly().WithStdioServerTransport();

builder.Services.AddHttpClient(
    SYSML_DATABASE_CLIENT_NAME,
    client =>
    {
        // Set the base address of the named client.
        client.BaseAddress = new Uri(SYSML_DATABSE_SERVER_URL);
        // Add a user-agent default request header.
        client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
    });
builder.Services.AddSingleton<ISysMLApiService, SysMLApiService>();
// Now we need to bind the SysML meta model service
builder.Services.AddSingleton(new SysMLMetaModelFactory(ResolveSchemasPath(builder.Environment.ContentRootPath)));


await builder.Build().RunAsync();

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