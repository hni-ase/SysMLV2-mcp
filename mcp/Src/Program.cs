using mcp.Src.Services;
using Src.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.
    AddConsole(consoleLogOptions =>
    {
        consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Error;
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
builder.Services.AddSingleton<ISysMLApiService, SysMLApiService>();


await builder.Build().RunAsync();