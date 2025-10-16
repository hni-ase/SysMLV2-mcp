using Microsoft.Extensions.Logging;
using Moq;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;
using Org.OpenAPITools.Test.Api;
using Src.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace csharp_mcp_example_test;

public class SysMLApiServiceImplementationTests
{
    [Fact]
    public void CommitInformation_ShouldBeInstantiable()
    {
        // Act
        var commitInfo = new Commit();
        
        // Assert
        Assert.NotNull(commitInfo);
    }

    [Fact]
    public void ISysMLApiService_Interface_ShouldHaveAllRequiredMethods()
    {
        // This test ensures our interface contract is complete
        var interfaceType = typeof(ISysMLApiService);
        var methods = interfaceType.GetMethods();

        var expectedMethods = new[]
        {
            "CreateNewProjectAsync",
            "CreateNewBranchAsync", 
            "GetCommits",
            "CommitElementToBranchAsync"
        };

        foreach (var expectedMethod in expectedMethods)
        {
            Assert.True(methods.Any(m => m.Name == expectedMethod), 
                $"Method {expectedMethod} should exist in ISysMLApiService");
        }
    }

    [Theory]
    [InlineData("ValidProject")]
    [InlineData("Project123")]
    [InlineData("Test Project With Spaces")]
    public void ISysMLApiService_CreateNewProjectAsync_ShouldAcceptValidNames(string projectName)
    {
        // This verifies the interface method signature accepts string parameters
        var method = typeof(ISysMLApiService).GetMethod("CreateNewProjectAsync");
        
        Assert.NotNull(method);
        var parameters = method.GetParameters();
        
        // Should have 2 string parameters: projectName and projectDescription
        Assert.Equal(2, parameters.Length);
        Assert.All(parameters, p => Assert.Equal(typeof(string), p.ParameterType));
        
        // Should return Task<Guid>
        Assert.Equal(typeof(Task<Guid>), method.ReturnType);
    }

    [Fact]
    public void ISysMLApiService_CreateNewBranchAsync_ShouldHaveCorrectSignature()
    {
        var method = typeof(ISysMLApiService).GetMethod("CreateNewBranchAsync");
        
        Assert.NotNull(method);
        var parameters = method.GetParameters();
        
        Assert.Equal(2, parameters.Length);
        Assert.Equal(typeof(Guid), parameters[0].ParameterType); // projectId
        Assert.Equal(typeof(string), parameters[1].ParameterType); // branchName
        Assert.Equal(typeof(Task<Guid>), method.ReturnType);
    }

    [Fact]
    public void ISysMLApiService_GetCommits_ShouldHaveCorrectSignature()
    {
        var method = typeof(ISysMLApiService).GetMethod("GetCommits");
        
        Assert.NotNull(method);
        var parameters = method.GetParameters();
        
        Assert.Equal(2, parameters.Length);
        Assert.Equal(typeof(Guid), parameters[0].ParameterType); // projectId
        Assert.Equal(typeof(Guid), parameters[1].ParameterType); // branchId
        Assert.Equal(typeof(Task<List<Commit>>), method.ReturnType);
    }

    [Fact]
    public void ISysMLApiService_CommitElementToBranchAsync_ShouldHaveCorrectSignature()
    {
        var method = typeof(ISysMLApiService).GetMethod("CommitElementToBranchAsync");
        
        Assert.NotNull(method);
        var parameters = method.GetParameters();
        
        Assert.Equal(3, parameters.Length);
        Assert.Equal(typeof(Guid), parameters[0].ParameterType); // projectId
        Assert.Equal(typeof(Guid), parameters[1].ParameterType); // branchId
        // Third parameter should be Commit type from the OpenAPI generated code
        Assert.Equal("Commit", parameters[2].ParameterType.Name);
        Assert.Equal(typeof(Task<Guid>), method.ReturnType);
    }
}

public class MockSysMLApiServiceTests
{
    private class MockSysMLApiService : ISysMLApiService
    {
        public Task<string> CreateNewProjectAsync(string projectName, string projectDescription)
        {
            if (string.IsNullOrEmpty(projectName))
                throw new ArgumentException("Project name cannot be null or empty", nameof(projectName));
            
            return Task.FromResult(Guid.NewGuid().ToString());
        }

        public Task<Guid> CreateNewBranchAsync(Guid projectId, string branchName)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("Project ID cannot be empty", nameof(projectId));
            if (string.IsNullOrEmpty(branchName))
                throw new ArgumentException("Branch name cannot be null or empty", nameof(branchName));
            
            return Task.FromResult(Guid.NewGuid());
        }

        public Task<List<Commit>> GetCommits(Guid projectId, Guid branchId)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("Project ID cannot be empty", nameof(projectId));
            if (branchId == Guid.Empty)
                throw new ArgumentException("Branch ID cannot be empty", nameof(branchId));
            
            return Task.FromResult(new List<Commit>
            {
                new(),
                new()
            });
        }

        public Task<Guid> CommitElementToBranchAsync(Guid projectId, Guid branchId, Commit commit)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("Project ID cannot be empty", nameof(projectId));
            if (branchId == Guid.Empty)
                throw new ArgumentException("Branch ID cannot be empty", nameof(branchId));
            if (commit == null)
                throw new ArgumentNullException(nameof(commit));
            
            return Task.FromResult(Guid.NewGuid());
        }

        Task<Project> ISysMLApiService.CreateNewProjectAsync(string projectName, string projectDescription)
        {
            throw new NotImplementedException();
        }

        Task<Branch> ISysMLApiService.CreateNewBranchAsync(Guid projectId, string branchName)
        {
            throw new NotImplementedException();
        }

        public Task<Element> CreateElementAsync(Guid projectId, Guid branchId, Element element)
        {
            throw new NotImplementedException();
        }
    }

    private readonly MockSysMLApiService _service;

    public MockSysMLApiServiceTests()
    {
        _service = new MockSysMLApiService();
    }

    [Fact]
    public async Task TestTest(){
        ProjectApiTests tests = new ProjectApiTests();

        await tests.PostProjectAsyncTest();
    }

    [Fact]
    public async Task CreateNewProjectAsync_ShouldReturnGuid_WhenValidParametersProvided()
    {
        // Act
        var result = await _service.CreateNewProjectAsync("TestProject", "Test Description");

        // Assert
        Assert.NotEqual(Guid.Empty.ToString(), result);
    }

    [Fact]
    public async Task CreateNewProjectAsync_ShouldThrowException_WhenProjectNameIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateNewProjectAsync(null, "Description"));
    }

    [Fact]
    public async Task CreateNewProjectAsync_ShouldThrowException_WhenProjectNameIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateNewProjectAsync("", "Description"));
    }

    [Fact]
    public async Task CreateNewBranchAsync_ShouldReturnGuid_WhenValidParametersProvided()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var branchName = "feature-branch";

        // Act
        var result = await _service.CreateNewBranchAsync(projectId, branchName);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task CreateNewBranchAsync_ShouldThrowException_WhenProjectIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateNewBranchAsync(Guid.Empty, "BranchName"));
    }

    [Fact]
    public async Task GetCommits_ShouldReturnCommitList_WhenValidParametersProvided()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        // Act
        var result = await _service.GetCommits(projectId, branchId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetCommits_ShouldThrowException_WhenProjectIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetCommits(Guid.Empty, Guid.NewGuid()));
    }
}