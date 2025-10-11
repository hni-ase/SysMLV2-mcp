using System.Text.Json;
using mcp.Src.Services;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;
using Src.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.
    AddOpenTelemetry().
    AddConsole(consoleLogOptions =>
    {
        // Configure all logs to go to stderr
        consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
    });
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services.AddHttpClient(
    "SysMLV2-Database-Client",
    client =>
    {
        // Set the base address of the named client.
        client.BaseAddress = new Uri("http://localhost:9000");

        // Add a user-agent default request header.
        client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
    });
// builder.Services.AddHttpClient<ProjectApi>((client) =>
// {
//     client.BaseAddress = new Uri("http://localhost:9000");
// });

// builder.Services.AddScoped<JsonSerializerOptionsProvider>();
// builder.Services.AddScoped<JsonSerializerOptions>();

builder.Services.AddSingleton<ISysMLApiService, SysMLApiService>();


await builder.Build().RunAsync();