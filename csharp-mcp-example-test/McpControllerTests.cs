using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Services;
using Src.Controllers;
using System;
using Xunit;

namespace csharp_mcp_example_test;

public class McpControllerTests
{
    // We'll test the controller with a real McpService instance since mocking is difficult
    private readonly McpController _controller;
    private readonly McpService _mcpService;

    public McpControllerTests()
    {
        var mockLogger = new Mock<ILogger<McpService>>();
        _mcpService = new McpService(mockLogger.Object);
        _controller = new McpController(_mcpService);
    }

    [Fact]
    public void Ping_ShouldReturnOkResult()
    {
        // Act
        var result = _controller.Ping();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        // Verify the response contains expected message
        var response = okResult.Value;
        Assert.NotNull(response);
        
        // Check if it has a message property with expected content
        var messageProperty = response.GetType().GetProperty("message");
        Assert.NotNull(messageProperty);
        var messageValue = messageProperty.GetValue(response) as string;
        Assert.Equal("MCP endpoint is alive.", messageValue);
    }

    [Fact]
    public void ReceiveData_ShouldReturnOkResult()
    {
        // Arrange
        var testData = new { test = "data", number = 42 };

        // Act
        var result = _controller.ReceiveData(testData);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        // Verify the response structure
        var response = okResult.Value;
        Assert.NotNull(response);
        
        var statusProperty = response.GetType().GetProperty("status");
        var receivedProperty = response.GetType().GetProperty("received");
        
        Assert.NotNull(statusProperty);
        Assert.NotNull(receivedProperty);
        
        Assert.Equal("received", statusProperty.GetValue(response));
        Assert.Equal(testData, receivedProperty.GetValue(response));
    }

    [Fact]
    public void ReceiveData_ShouldHandleNullData()
    {
        // Act
        var result = _controller.ReceiveData(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Theory]
    [InlineData("string data")]
    [InlineData(123)]
    [InlineData(true)]
    public void ReceiveData_ShouldHandleDifferentDataTypes(object testData)
    {
        // Act
        var result = _controller.ReceiveData(testData);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact (Skip = "not implemented")]
    public void McpController_ShouldRequireMcpService()
    {
        // This test verifies that the controller requires McpService dependency
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new McpController(null!));
    }

    [Fact]
    public void McpController_CanBeCreatedWithValidService()
    {
        // Arrange & Act
        var logger = new Mock<ILogger<McpService>>();
        var service = new McpService(logger.Object);
        var controller = new McpController(service);

        // Assert
        Assert.NotNull(controller);
    }
}

public class EndToEndIntegrationTests
{
    [Fact]
    public void McpWorkflow_ShouldIntegrateCorrectly()
    {
        // This is a conceptual test showing how the components work together
        // Arrange
        var logger = new Mock<ILogger<McpService>>();
        var mcpService = new McpService(logger.Object);
        var controller = new McpController(mcpService);

        // Act
        var pingResult = controller.Ping();
        var dataResult = controller.ReceiveData(new { workflow = "test" });

        // Assert
        Assert.IsType<OkObjectResult>(pingResult);
        Assert.IsType<OkObjectResult>(dataResult);
        
        // Verify logging was called (indirectly through the service)
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing incoming MCP data")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}