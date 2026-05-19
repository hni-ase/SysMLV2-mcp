using MCP.Src.Services.FactoryServices;
using mcp.Src.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace mcp.Src.Services.FactoryServices.Tests
{
    public class SysMLPackageFactoryTest
    {
        private static (SysMLApiService apiService, SysMLPackageFactory factory) BuildServices()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddLogging(b => b.AddConsole());

                    services.AddHttpClient("SysMLV2-Database-Client", client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:9000");
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-test");
                    });

                    var schemasPath = Path.Combine(Directory.GetCurrentDirectory(),
                        "..", "..", "..", "..", "sysmlv2-api-spec", "metamodels");
                    services.AddSingleton(new SysMLMetaModelFactory(schemasPath));
                })
                .Build();

            var apiService = new SysMLApiService(host.Services.GetRequiredService<IHttpClientFactory>());
            var factory = new SysMLPackageFactory(apiService,
                host.Services.GetRequiredService<SysMLMetaModelFactory>());

            return (apiService, factory);
        }

        [Fact]
        public async Task CreatePackage_CreatesPackageInProject()
        {
            var (apiService, factory) = BuildServices();

            var project = await apiService.CreateNewProjectAsync(
                $"PackageFactoryTest-{Guid.NewGuid():N}", "Test project description");
            var projectGuid = project?.Id ?? throw new Exception("project not created");

            var packageGuid = await factory.CreatePackage(projectGuid, "NewTestPackage");

            Assert.NotEqual(Guid.Empty, packageGuid);
        }

        [Fact]
        public async Task CreatePackage_PackageAppearsInProjectElements()
        {
            var (apiService, factory) = BuildServices();

            var projectName = $"PackageElemTest-{Guid.NewGuid():N}";
            var project = await apiService.CreateNewProjectAsync(projectName, "");
            var projectGuid = project?.Id ?? throw new Exception("project not created");

            var packageName = "VerifyPackage";
            await factory.CreatePackage(projectGuid, packageName);

            // Get the default branch head commit
            var branches = await apiService.GetBranchesAsync(projectGuid);
            var defaultBranch = branches.FirstOrDefault()
                ?? throw new Exception("No branch found");
            var headCommitId = defaultBranch.Head?.Id
                ?? throw new Exception("No head commit");

            var elements = await apiService.GetElementsAsync(projectGuid, headCommitId);
            Assert.NotNull(elements);
            Assert.Contains(elements, e =>
                string.Equals(e.GetName(), packageName, StringComparison.OrdinalIgnoreCase)
                || string.Equals(e.Type, "Package", StringComparison.OrdinalIgnoreCase));
        }
    }
}
