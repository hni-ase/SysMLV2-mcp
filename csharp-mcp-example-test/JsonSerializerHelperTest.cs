using System.Text.Json;
using System.Text.Json.Nodes;
using SysMLV2.MCP.Models;
using Xunit;

namespace csharp_mcp_example_test;

/// <summary>
/// Tests that <see cref="SysMLProject"/>, <see cref="SysMLElement"/> and
/// <see cref="CommitRequest"/> serialize / deserialize correctly with
/// <see cref="System.Text.Json"/> (no custom converter helpers needed).
/// </summary>
public class JsonSerializerTests
{
    // ── SysMLProject ──────────────────────────────────────────────────────────

    [Fact]
    public void SysMLProject_Deserializes_All_Known_Fields()
    {
        var json = """
            {
                "@id": "0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3",
                "@type": "Project",
                "alias": ["alias1", "alias2"],
                "created": "2025-10-22T08:37:08.158329Z",
                "defaultBranch": { "@id": "e590cd47-7ae7-4a24-b66f-c40b7512f119" },
                "description": "A test project",
                "name": "TestProject"
            }
            """;

        var project = JsonSerializer.Deserialize<SysMLProject>(json);

        Assert.NotNull(project);
        Assert.Equal(new Guid("0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"), project.Id);
        Assert.Equal("Project", project.Type);
        Assert.Equal("A test project", project.Description);
        Assert.Equal("TestProject", project.Name);
        Assert.Collection(project.Alias!, a => Assert.Equal("alias1", a), a => Assert.Equal("alias2", a));
        Assert.Equal(new Guid("e590cd47-7ae7-4a24-b66f-c40b7512f119"), project.DefaultBranch!.Id);
    }

    [Fact]
    public void SysMLProject_Deserializes_Null_Optional_Fields()
    {
        var json = """{ "@type": "Project" }""";
        var project = JsonSerializer.Deserialize<SysMLProject>(json);

        Assert.NotNull(project);
        Assert.Null(project.Id);
        Assert.Null(project.Name);
        Assert.Null(project.Description);
        Assert.Null(project.DefaultBranch);
    }

    // ── SysMLElement ──────────────────────────────────────────────────────────

    [Fact]
    public void SysMLElement_Deserializes_And_GetName_Works()
    {
        var json = """
            {
                "@id": "aaaaaaaa-0000-0000-0000-000000000001",
                "@type": "Package",
                "name": "RootPackage",
                "elementId": "aaaaaaaa-0000-0000-0000-000000000001",
                "someOtherField": true
            }
            """;

        var element = JsonSerializer.Deserialize<SysMLElement>(json);

        Assert.NotNull(element);
        Assert.Equal(new Guid("aaaaaaaa-0000-0000-0000-000000000001"), element.Id);
        Assert.Equal("Package", element.Type);
        Assert.Equal("RootPackage", element.GetName());
        Assert.NotNull(element.AdditionalProperties);
        Assert.True(element.AdditionalProperties.ContainsKey("elementId"));
    }

    [Fact]
    public void SysMLElement_GetName_ReturnsEmpty_WhenNoNameField()
    {
        var element = new SysMLElement { Type = "Package" };
        Assert.Equal(string.Empty, element.GetName());
    }

    // ── SysMLRef ──────────────────────────────────────────────────────────────

    [Fact]
    public void SysMLRef_Deserializes_Id_Field()
    {
        var json = """{ "@id": "bbbbbbbb-0000-0000-0000-000000000002" }""";
        var sysmlRef = JsonSerializer.Deserialize<SysMLRef>(json);
        Assert.NotNull(sysmlRef);
        Assert.Equal(new Guid("bbbbbbbb-0000-0000-0000-000000000002"), sysmlRef.Id);
    }

    [Fact]
    public void SysMLRef_Serializes_With_AtId_Key()
    {
        var id = Guid.NewGuid();
        var sysmlRef = new SysMLRef(id);
        var json = JsonSerializer.Serialize(sysmlRef);
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(id.ToString(), doc.RootElement.GetProperty("@id").GetString());
    }

    // ── CommitRequest ─────────────────────────────────────────────────────────

    [Fact]
    public void CommitRequest_Serializes_With_Correct_Structure()
    {
        var payload = JsonSerializer.SerializeToElement(new Dictionary<string, object?>
        {
            ["@type"] = "Package",
            ["name"] = "MyPackage"
        });

        var commit = new CommitRequest
        {
            Change = [new DataVersionRequest { Payload = payload }]
        };

        var json = JsonSerializer.Serialize(commit);
        using var doc = JsonDocument.Parse(json);

        Assert.Equal("Commit", doc.RootElement.GetProperty("@type").GetString());
        var change = doc.RootElement.GetProperty("change");
        Assert.Equal(JsonValueKind.Array, change.ValueKind);
        Assert.Equal(1, change.GetArrayLength());
        Assert.Equal("DataVersion", change[0].GetProperty("@type").GetString());
        Assert.Equal("Package", change[0].GetProperty("payload").GetProperty("@type").GetString());
    }

    [Fact]
    public void CommitRequest_EmptyChange_SerializesAsEmptyArray()
    {
        var commit = new CommitRequest { Change = [] };
        var json = JsonSerializer.Serialize(commit);
        using var doc = JsonDocument.Parse(json);

        var change = doc.RootElement.GetProperty("change");
        Assert.Equal(JsonValueKind.Array, change.ValueKind);
        Assert.Equal(0, change.GetArrayLength());
    }

    // ── JsonObject for API requests ───────────────────────────────────────────

    [Fact]
    public void JsonObject_With_AtType_Key_Serializes_Correctly()
    {
        var node = new JsonObject
        {
            ["@type"] = "Project",
            ["name"] = "TestProject",
            ["description"] = "Test description"
        };

        var json = node.ToJsonString();
        using var doc = JsonDocument.Parse(json);

        Assert.Equal("Project", doc.RootElement.GetProperty("@type").GetString());
        Assert.Equal("TestProject", doc.RootElement.GetProperty("name").GetString());
        Assert.Equal("Test description", doc.RootElement.GetProperty("description").GetString());
    }
}
