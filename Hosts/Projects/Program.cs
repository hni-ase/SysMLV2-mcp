using mcp.Src.Services;
using Microsoft.Extensions.DependencyInjection;
using Src.Services;
using System.IO;
using Tools.Projects;

const string SYSML_DATABASE_CLIENT_NAME = "SysMLV2-Database-Client";
const string SYSML_DATABASE_SERVER_URL = "http://localhost:9000";

var builder = WebApplication.CreateBuilder(args);
builder.Logging
    .AddConsole(consoleLogOptions =>
    {
        consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Error;
    });

builder.Services.AddHttpClient(
    SYSML_DATABASE_CLIENT_NAME,
    client =>
    {
        client.BaseAddress = new Uri(SYSML_DATABASE_SERVER_URL);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
    });
builder.Services.AddSingleton<ISysMLApiService, SysMLApiService>();

builder.Services
    .AddMcpServer()
    .WithTools(new[] { typeof(ProjectTools) })
    .WithStdioServerTransport();

await builder.Build().RunAsync();