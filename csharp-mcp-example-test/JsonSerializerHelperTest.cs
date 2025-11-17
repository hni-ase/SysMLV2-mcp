using System;
using System.Text.Json;
using Xunit;
using Org.OpenAPITools.Model;
using Org.OpenAPITools;

namespace csharp_mcp_example_test
{
    public class JsonSerializerHelperTest
    {
        [Fact]
        public void JsonSerializerHelper_Should_Configure_Options_For_Unknown_Properties()
        {
            // Arrange
            var jsonWithUnknownProperties = @"{
                ""@id"": ""0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"",
                ""@type"": ""Project"",
                ""name"": ""Test Project"",
                ""description"": ""Test Description"",
                ""unknownProperty1"": ""this should be ignored"",
                ""unknownProperty2"": {
                    ""nested"": ""object"",
                    ""should"": ""be ignored""
                },
                ""unknownArray"": [1, 2, 3]
            }";

            // Act
            var options = JsonSerializerHelper.CreateOptionsWithUnknownPropertyHandling();
            var project = JsonSerializer.Deserialize<Project>(jsonWithUnknownProperties, options);

            // Assert
            Assert.NotNull(project);
            Assert.Equal(new Guid("0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"), project.Id);
            Assert.Equal(Project.TypeEnum.Project, project.Type);
            Assert.Equal("Test Project", project.Name);
            Assert.Equal("Test Description", project.Description);
        }
    }
}