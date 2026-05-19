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

    // ── SysMLBranch ───────────────────────────────────────────────────────────

    [Fact]
    public void SysMLBranch_Deserializes_All_Known_Fields()
    {
        var json = """
            {
                "@id": "cccccccc-0000-0000-0000-000000000003",
                "@type": "Branch",
                "alias": ["main"],
                "created": "2025-10-22T09:00:00Z",
                "description": "Default branch",
                "name": "main",
                "head": { "@id": "dddddddd-0000-0000-0000-000000000004" },
                "owningProject": { "@id": "eeeeeeee-0000-0000-0000-000000000005" }
            }
            """;

        var branch = JsonSerializer.Deserialize<SysMLBranch>(json);

        Assert.NotNull(branch);
        Assert.Equal(new Guid("cccccccc-0000-0000-0000-000000000003"), branch.Id);
        Assert.Equal("Branch", branch.Type);
        Assert.Equal("main", branch.Name);
        Assert.Equal("Default branch", branch.Description);
        Assert.Equal(new Guid("dddddddd-0000-0000-0000-000000000004"), branch.Head!.Id);
        Assert.Equal(new Guid("eeeeeeee-0000-0000-0000-000000000005"), branch.OwningProject!.Id);
    }

    [Fact]
    public void SysMLBranch_Deserializes_Null_Optional_Fields()
    {
        var json = """{ "@type": "Branch", "name": "feature" }""";
        var branch = JsonSerializer.Deserialize<SysMLBranch>(json);

        Assert.NotNull(branch);
        Assert.Equal("feature", branch.Name);
        Assert.Null(branch.Id);
        Assert.Null(branch.Head);
        Assert.Null(branch.OwningProject);
    }

    // ── SysMLCommit ───────────────────────────────────────────────────────────

    [Fact]
    public void SysMLCommit_Deserializes_All_Known_Fields()
    {
        var json = """
            {
                "@id": "ffffffff-0000-0000-0000-000000000006",
                "@type": "Commit",
                "created": "2025-10-22T10:00:00Z",
                "description": "initial commit",
                "name": null,
                "owningProject": { "@id": "11111111-0000-0000-0000-000000000007" }
            }
            """;

        var commit = JsonSerializer.Deserialize<SysMLCommit>(json);

        Assert.NotNull(commit);
        Assert.Equal(new Guid("ffffffff-0000-0000-0000-000000000006"), commit.Id);
        Assert.Equal("Commit", commit.Type);
        Assert.Equal("initial commit", commit.Description);
        Assert.Equal(new Guid("11111111-0000-0000-0000-000000000007"), commit.OwningProject!.Id);
    }

    // ── ElementCreationResult ─────────────────────────────────────────────────

    [Fact]
    public void ElementCreationResult_Serializes_Success()
    {
        var result = new ElementCreationResult
        {
            ElementId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            Type = "Package",
            ParentId = "11111111-2222-3333-4444-555555555555",
            AppliedAttributes = ["name", "isAbstract"],
            InvalidAttributes = ["bogusField"],
            Success = true,
            Message = "Created successfully."
        };

        var json = JsonSerializer.Serialize(result);
        using var doc = JsonDocument.Parse(json);

        Assert.Equal("Package", doc.RootElement.GetProperty("type").GetString());
        Assert.True(doc.RootElement.GetProperty("success").GetBoolean());
        var applied = doc.RootElement.GetProperty("appliedAttributes");
        Assert.Equal(2, applied.GetArrayLength());
        var invalid = doc.RootElement.GetProperty("invalidAttributes");
        Assert.Equal(1, invalid.GetArrayLength());
        Assert.Equal("bogusField", invalid[0].GetString());
    }

    [Fact]
    public void ElementCreationResult_Serializes_Failure_With_No_Parent()
    {
        var result = new ElementCreationResult
        {
            ElementId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            Type = "RequirementUsage",
            ParentId = null,
            AppliedAttributes = [],
            InvalidAttributes = ["@id (reserved)"],
            Success = false,
            Message = "No valid attributes to apply."
        };

        var json = JsonSerializer.Serialize(result);
        using var doc = JsonDocument.Parse(json);

        Assert.False(doc.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(JsonValueKind.Null, doc.RootElement.GetProperty("parentId").ValueKind);
    }

    // ── ElementUpdateResult ───────────────────────────────────────────────────

    [Fact]
    public void ElementUpdateResult_Serializes_Success()
    {
        var result = new ElementUpdateResult
        {
            ElementId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            Type = "RequirementUsage",
            UpdatedAttributes = ["name", "reqId"],
            InvalidAttributes = [],
            Success = true,
            Message = "Successfully updated 2 attribute(s)."
        };

        var json = JsonSerializer.Serialize(result);
        using var doc = JsonDocument.Parse(json);

        Assert.True(doc.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(2, doc.RootElement.GetProperty("updatedAttributes").GetArrayLength());
        Assert.Equal(0, doc.RootElement.GetProperty("invalidAttributes").GetArrayLength());
    }

    [Fact]
    public void ElementUpdateResult_Serializes_Failure_With_Invalid_Attributes()
    {
        var result = new ElementUpdateResult
        {
            ElementId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            Type = "Package",
            UpdatedAttributes = [],
            InvalidAttributes = ["@id (read-only: element identity cannot be changed)"],
            Success = false,
            Message = "No valid attributes to update."
        };

        var json = JsonSerializer.Serialize(result);
        using var doc = JsonDocument.Parse(json);

        Assert.False(doc.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(1, doc.RootElement.GetProperty("invalidAttributes").GetArrayLength());
    }

    // ── ElementSchemaInfo ─────────────────────────────────────────────────────

    [Fact]
    public void ElementSchemaInfo_Serializes_Required_And_Optional_Attributes()
    {
        var info = new ElementSchemaInfo
        {
            ElementId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            Type = "Package",
            RequiredAttributes = new Dictionary<string, string>
            {
                ["@id"] = "string",
                ["@type"] = "const:Package"
            },
            OptionalAttributes = new Dictionary<string, string>
            {
                ["name"] = "string | null",
                ["isAbstract"] = "boolean"
            }
        };

        var json = JsonSerializer.Serialize(info);
        using var doc = JsonDocument.Parse(json);

        Assert.Equal("Package", doc.RootElement.GetProperty("type").GetString());
        var required = doc.RootElement.GetProperty("requiredAttributes");
        Assert.Equal("string", required.GetProperty("@id").GetString());
        var optional = doc.RootElement.GetProperty("optionalAttributes");
        Assert.True(optional.TryGetProperty("name", out _));
    }

    [Fact]
    public void ElementSchemaInfo_EmptyAttributes_SerializeAsEmptyObjects()
    {
        var info = new ElementSchemaInfo
        {
            ElementId = "",
            Type = "Unknown",
            RequiredAttributes = new(),
            OptionalAttributes = new()
        };

        var json = JsonSerializer.Serialize(info);
        using var doc = JsonDocument.Parse(json);

        Assert.Equal(JsonValueKind.Object, doc.RootElement.GetProperty("requiredAttributes").ValueKind);
        Assert.Equal(0, doc.RootElement.GetProperty("requiredAttributes").EnumerateObject().Count());
    }
}
