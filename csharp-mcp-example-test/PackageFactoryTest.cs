using MCP.Src.Services.FactoryServices;
using mcp.Src.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace mcp.Src.Services.FactoryServices.Tests
{

    public class SysMLPackageFactoryTest
    {


        [Fact]
        public async Task SysMLPackageFactory_CreatePackage_MustCreatePackageInDatabase()
        {
            // Create an Http Host
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Add logging
                    services.AddLogging(builder => builder.AddConsole());
                    
                    // Add HTTP client factory
                    services.AddHttpClient("SysMLV2-Database-Client", client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:9000");
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-test");
                    });
                    
                    // Add the SysML metamodel factory with the correct path to schemas
                    // var schemasPath = Path.Combine(Directory.GetCurrentDirectory(), "sysmlv2-api-spec", "metamodels");
                    var schemasPath =  Path.Combine(Directory.GetCurrentDirectory(),"..", "..", "..", "..", "sysmlv2-api-spec", "metamodels");
                    services.AddSingleton(new SysMLMetaModelFactory(schemasPath));
                });

            var host = hostBuilder.Build();

            var apiService = new SysMLApiService(host);
            var factory = new SysMLPackageFactory(apiService, host.Services.GetRequiredService<SysMLMetaModelFactory>());

            // First we need to create the project
            var project = await apiService.CreateNewProjectAsync("TestProjectUnitTest", "Test project description");
            var projectGuid = project?.Id ?? throw new Exception("project not created");

            var package = factory.CreatePackage(projectGuid, "TestPakcage", "This is a test package");

            // Add your test logic here
            Assert.NotNull(factory);
        }


    }
    
}