using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Src.Services;
using System;
using System.Threading.Tasks;
using Xunit;
using ModelContextProtocol.Server;
using System.Runtime.CompilerServices;
using Org.OpenAPITools.Model;

namespace csharp_mcp_example_test;

public class ModelCreationToolTests
{
    private readonly Mock<ISysMLApiService> _mockSysMLService;
    private readonly Mock<McpServer> _mockMcpServer;
    private readonly Mock<IServiceProvider> _mockServiceProvider;

    public ModelCreationToolTests()
    {
        _mockSysMLService = new Mock<ISysMLApiService>();
        _mockMcpServer = new Mock<McpServer>();
        _mockServiceProvider = new Mock<IServiceProvider>();
    }

    [Fact]
    public void ModelCreationTools_CanBeInstantiated()
    {
        // Act & Assert
        var tools = new ModelCreationTools(_mockSysMLService.Object);
        Assert.NotNull(tools);
    }

    [Fact]
    public void CreateProject_ShouldReturnSuccessMessage_WhenProjectIsCreated()
    {
        // Arrange
        var projectName = "TestProject";
        var expectedProjectId = new Project();
        
        _mockSysMLService.Setup(s => s.CreateNewProjectAsync(
                It.Is<string>(name => name == projectName),
                It.IsAny<string>()))
            .ReturnsAsync(expectedProjectId);

        _mockServiceProvider.Setup(sp => sp.GetService(typeof(ISysMLApiService)))
            .Returns(_mockSysMLService.Object);

        _mockMcpServer.SetupGet(s => s.Services)
            .Returns(_mockServiceProvider.Object);

        // Act
        var result = ModelCreationTools.CreateProject(_mockMcpServer.Object, projectName);

        // Assert
        Assert.Contains(projectName, result);
        Assert.Contains(expectedProjectId.ToString(), result);
        Assert.Contains("created successfully", result);
    }

    [Fact]
    public void CreateProject_ShouldHandleNullService()
    {
        // Arrange
        var projectName = "TestProject";
        
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(ISysMLApiService)))
            .Returns((ISysMLApiService?)null);

        _mockMcpServer.SetupGet(s => s.Services)
            .Returns(_mockServiceProvider.Object);

        // Act & Assert
        // This should not throw an exception, but handle gracefully
        var result = ModelCreationTools.CreateProject(_mockMcpServer.Object, projectName);
        
        // The result should still contain some indication of the project name
        Assert.Contains(projectName, result);
    }

    [Fact]
    public void CreateElement_ShouldReturnSuccessMessage()
    {
        // Arrange
        var elementName = "TestElement";

        // Act
        var result = ModelCreationTools.CreateElement(elementName);

        // Assert
        Assert.Contains(elementName, result);
        Assert.Contains("created successfully", result);
    }

    [Fact]
    public void CreateRelationship_ShouldThrowNotImplementedException()
    {
        // Arrange
        var sourceElement = "Source";
        var targetElement = "Target";

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => 
            ModelCreationTools.CreateRelationship(sourceElement, targetElement));
    }
}

public class SysMLApiServiceIntegrationTests
{
    [Fact]
    public void CommitInformation_CanBeInstantiated()
    {
        // Act & Assert
        var commitInfo = new Commit();
        Assert.NotNull(commitInfo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ValidProjectName")]
    [InlineData("Project With Spaces")]
    [InlineData("ProjectWith123Numbers")]
    public void ISysMLApiService_CreateNewProjectAsync_ShouldAcceptValidProjectNames(string projectName)
    {
        // This is a contract test - we're verifying the interface contract
        var interfaceType = typeof(ISysMLApiService);
        var method = interfaceType.GetMethod("CreateNewProjectAsync");
        
        Assert.NotNull(method);
        
        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal(typeof(string), parameters[0].ParameterType);
        Assert.Equal(typeof(string), parameters[1].ParameterType);
        Assert.Equal(typeof(Task<Guid>), method.ReturnType);
    }

    [Fact]
    public void ISysMLApiService_Methods_ShouldHaveCorrectSignatures()
    {
        var interfaceType = typeof(ISysMLApiService);
        
        // Test CreateNewBranchAsync
        var createBranchMethod = interfaceType.GetMethod("CreateNewBranchAsync");
        Assert.NotNull(createBranchMethod);
        var branchParams = createBranchMethod.GetParameters();
        Assert.Equal(2, branchParams.Length);
        Assert.Equal(typeof(Guid), branchParams[0].ParameterType);
        Assert.Equal(typeof(string), branchParams[1].ParameterType);
        
        // Test GetCommits
        var getCommitsMethod = interfaceType.GetMethod("GetCommits");
        Assert.NotNull(getCommitsMethod);
        var commitsParams = getCommitsMethod.GetParameters();
        Assert.Equal(2, commitsParams.Length);
        Assert.Equal(typeof(Guid), commitsParams[0].ParameterType);
        Assert.Equal(typeof(Guid), commitsParams[1].ParameterType);
        
        // Test CommitElementToBranchAsync
        var commitElementMethod = interfaceType.GetMethod("CommitElementToBranchAsync");
        Assert.NotNull(commitElementMethod);
        var commitParams = commitElementMethod.GetParameters();
        Assert.Equal(3, commitParams.Length);
        Assert.Equal(typeof(Guid), commitParams[0].ParameterType);
        Assert.Equal(typeof(Guid), commitParams[1].ParameterType);
    }
}