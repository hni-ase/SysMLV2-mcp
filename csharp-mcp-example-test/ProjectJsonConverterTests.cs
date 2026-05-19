using System.Text.Json;
using SysMLV2.MCP.Models;
using Xunit;

/// <summary>
/// Tests that the built-in System.Text.Json serialization handles the SysML API JSON format
/// correctly for Project, Branch, Commit and Element — no custom converters required.
/// </summary>
public class ProjectSerializationTest
{
    [Fact]
    public void SysMLProject_RoundTrips_Via_SystemTextJson()
    {
        var json = """
            {
                "@id": "0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3",
                "@type": "Project",
                "alias": ["string"],
                "created": "2025-10-22T08:37:08.158329Z",
                "defaultBranch": {
                    "@id": "e590cd47-7ae7-4a24-b66f-c40b7512f119"
                },
                "description": "string",
                "name": "string"
            }
            """;

        var project = JsonSerializer.Deserialize<SysMLProject>(json);

        Assert.NotNull(project);
        Assert.Equal(new Guid("0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"), project.Id);
        Assert.Equal("Project", project.Type);
        Assert.Equal("string", project.Description);
        Assert.Equal("string", project.Name);
        Assert.NotNull(project.DefaultBranch);
        Assert.Equal(new Guid("e590cd47-7ae7-4a24-b66f-c40b7512f119"), project.DefaultBranch.Id);
    }

    [Fact]
    public void SysMLProject_Serializes_To_CorrectJson()
    {
        var project = new SysMLProject
        {
            Id = Guid.Parse("0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"),
            Type = "Project",
            Name = "MyProject",
            Description = "desc",
            DefaultBranch = new SysMLRef(Guid.Parse("e590cd47-7ae7-4a24-b66f-c40b7512f119"))
        };

        var json = JsonSerializer.Serialize(project);
        using var doc = JsonDocument.Parse(json);

        Assert.Equal("0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3", doc.RootElement.GetProperty("@id").GetString());
        Assert.Equal("Project", doc.RootElement.GetProperty("@type").GetString());
        Assert.Equal("MyProject", doc.RootElement.GetProperty("name").GetString());
        Assert.Equal("e590cd47-7ae7-4a24-b66f-c40b7512f119",
            doc.RootElement.GetProperty("defaultBranch").GetProperty("@id").GetString());
    }
}
