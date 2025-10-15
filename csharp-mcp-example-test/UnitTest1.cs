using Microsoft.Extensions.Logging;
using Moq;
using Services;
using Tools;
using Src.Services;
using System;
using Xunit;
using System.Threading.Tasks;

namespace csharp_mcp_example_test;

public class McpServiceTests
{
    private readonly Mock<ILogger<McpService>> _mockLogger;
    private readonly McpService _mcpService;

    public McpServiceTests()
    {
        _mockLogger = new Mock<ILogger<McpService>>();
        _mcpService = new McpService(_mockLogger.Object);
    }

    [Fact]
    public void ExecuteTool_ShouldLogAndExecuteTool()
    {
        // Arrange
        var mockTool = new Mock<AbstractTool>("TestTool", "Test Description");
        mockTool.Setup(t => t.HandleOperation(It.IsAny<object>()));

        // Act
        _mcpService.ExecuteTool(mockTool.Object);

        // Assert
        mockTool.Verify(t => t.HandleOperation(null), Times.Once);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Executing tool: TestTool")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void ProcessIncomingData_ShouldLogIncomingData()
    {
        // Arrange
        var testData = new { message = "test data" };

        // Act
        _mcpService.ProcessIncomingData(testData);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing incoming MCP data")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void McpService_CanBeCreatedWithoutLogger()
    {
        // Act & Assert
        var service = new McpService();
        Assert.NotNull(service);
    }
}

public class AbstractToolTests
{
    private class TestTool : AbstractTool
    {
        public bool HandleOperationCalled { get; private set; }
        public object? LastReceivedValue { get; private set; }

        public TestTool() : base("TestTool", "Test tool for unit tests") { }

        public override void HandleOperation(object? value)
        {
            HandleOperationCalled = true;
            LastReceivedValue = value;
        }
    }

    [Fact]
    public void AbstractTool_ShouldInitializeWithNameAndDescription()
    {
        // Act
        var tool = new TestTool();

        // Assert
        Assert.Equal("TestTool", tool.Name);
        Assert.Equal("Test tool for unit tests", tool.Description);
    }

    [Fact]
    public void HandleOperation_ShouldBeCallable()
    {
        // Arrange
        var tool = new TestTool();
        var testData = new { test = "data" };

        // Act
        tool.HandleOperation(testData);

        // Assert
        Assert.True(tool.HandleOperationCalled);
        Assert.Equal(testData, tool.LastReceivedValue);
    }

    [Fact]
    public void HandleOperation_ShouldAcceptNullValue()
    {
        // Arrange
        var tool = new TestTool();

        // Act
        tool.HandleOperation(null);

        // Assert
        Assert.True(tool.HandleOperationCalled);
        Assert.Null(tool.LastReceivedValue);
    }
}

public class SysMLApiServiceTests
{
    [Fact]
    public void ISysMLApiService_ShouldHaveRequiredMethods()
    {
        // This test verifies that the interface has the expected methods
        var interfaceType = typeof(ISysMLApiService);
        
        var createProjectMethod = interfaceType.GetMethod("CreateNewProjectAsync");
        var createBranchMethod = interfaceType.GetMethod("CreateNewBranchAsync");
        var getCommitsMethod = interfaceType.GetMethod("GetCommits");
        var commitElementMethod = interfaceType.GetMethod("CommitElementToBranchAsync");

        Assert.NotNull(createProjectMethod);
        Assert.NotNull(createBranchMethod);
        Assert.NotNull(getCommitsMethod);
        Assert.NotNull(commitElementMethod);

        // Verify return types
        Assert.Equal(typeof(Task<Guid>), createProjectMethod.ReturnType);
        Assert.Equal(typeof(Task<Guid>), createBranchMethod.ReturnType);
        Assert.Equal(typeof(Task<List<CommitInformation>>), getCommitsMethod.ReturnType);
        Assert.Equal(typeof(Task<Guid>), commitElementMethod.ReturnType);
    }
}
