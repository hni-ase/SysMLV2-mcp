using System;
using System.Text.Json;
using Xunit;
using Org.OpenAPITools.Model;

namespace csharp_mcp_example_test
{
    public class UnknownPropertiesDeserializationTest
    {
        [Fact]
        public void Project_Should_Deserialize_With_Unknown_Properties()
        {
            // Arrange
            var jsonWithUnknownProperties = @"{
                ""@id"": ""0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"",
                ""@type"": ""Project"",
                ""alias"": [""string""],
                ""created"": ""2025-10-22T08:37:08.158329Z"",
                ""defaultBranch"": {
                    ""@id"": ""e590cd47-7ae7-4a24-b66f-c40b7512f119""
                },
                ""description"": ""string"",
                ""name"": ""string"",
                ""unknownProperty1"": ""this should be ignored"",
                ""unknownProperty2"": {
                    ""nested"": ""object"",
                    ""should"": ""be ignored""
                },
                ""unknownArray"": [1, 2, 3]
            }";

            var options = new JsonSerializerOptions();
            options.Converters.Add(new ProjectJsonConverter());
            options.Converters.Add(new ProjectDefaultBranchJsonConverter());

            // Act & Assert - Should not throw an exception
            var project = JsonSerializer.Deserialize<Project>(jsonWithUnknownProperties, options);

            // Verify known properties are correctly deserialized
            Assert.NotNull(project);
            Assert.Equal(new Guid("0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"), project.Id);
            Assert.Equal(Project.TypeEnum.Project, project.Type);
            Assert.Equal("string", project.Description);
            Assert.Equal("string", project.Name);
            Assert.NotNull(project.Alias);
            Assert.Single(project.Alias);
            Assert.Equal("string", project.Alias[0]);
        }

        [Fact]
        public void Commit_Should_Deserialize_With_Unknown_Properties()
        {
            // Arrange
            var jsonWithUnknownProperties = @"{
                ""@id"": ""1a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"",
                ""@type"": ""Commit"",
                ""alias"": [""commit-alias""],
                ""created"": ""2025-10-22T08:37:08.158329Z"",
                ""description"": ""Test commit"",
                ""unknownProperty1"": ""this should be ignored"",
                ""unknownProperty2"": {
                    ""nested"": ""object"",
                    ""should"": ""be ignored""
                },
                ""unknownArray"": [1, 2, 3]
            }";

            var options = new JsonSerializerOptions();
            options.Converters.Add(new CommitJsonConverter());
            options.Converters.Add(new BranchHeadJsonConverter());
            options.Converters.Add(new BranchOwningProjectJsonConverter());
            options.Converters.Add(new DataVersionJsonConverter());

            // Act & Assert - Should not throw an exception
            var commit = JsonSerializer.Deserialize<Commit>(jsonWithUnknownProperties, options);

            // Verify known properties are correctly deserialized
            Assert.NotNull(commit);
            Assert.Equal(new Guid("1a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"), commit.Id);
            Assert.Equal(Commit.TypeEnum.Commit, commit.Type);
            Assert.Equal("Test commit", commit.Description);
            Assert.NotNull(commit.Alias);
            Assert.Single(commit.Alias);
            Assert.Equal("commit-alias", commit.Alias[0]);
        }

        [Fact]
        public void Branch_Should_Deserialize_With_Unknown_Properties()
        {
            // Arrange
            var jsonWithUnknownProperties = @"{
                ""@id"": ""2a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"",
                ""@type"": ""Branch"",
                ""alias"": [""branch-alias""],
                ""created"": ""2025-10-22T08:37:08.158329Z"",
                ""description"": ""Test branch"",
                ""name"": ""main"",
                ""unknownProperty1"": ""this should be ignored"",
                ""unknownProperty2"": {
                    ""nested"": ""object"",
                    ""should"": ""be ignored""
                },
                ""unknownArray"": [1, 2, 3]
            }";

            var options = new JsonSerializerOptions();
            options.Converters.Add(new BranchJsonConverter());
            options.Converters.Add(new BranchHeadJsonConverter());
            options.Converters.Add(new BranchOwningProjectJsonConverter());

            // Act & Assert - Should not throw an exception
            var branch = JsonSerializer.Deserialize<Branch>(jsonWithUnknownProperties, options);

            // Verify known properties are correctly deserialized
            Assert.NotNull(branch);
            Assert.Equal(new Guid("2a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"), branch.Id);
            Assert.Equal(Branch.TypeEnum.Branch, branch.Type);
            Assert.Equal("Test branch", branch.Description);
            Assert.Equal("main", branch.Name);
            Assert.NotNull(branch.Alias);
            Assert.Single(branch.Alias);
            Assert.Equal("branch-alias", branch.Alias[0]);
        }
    }
}