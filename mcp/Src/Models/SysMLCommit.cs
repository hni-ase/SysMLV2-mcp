using System.Text.Json;
using System.Text.Json.Serialization;

namespace SysMLV2.MCP.Models;

/// <summary>Represents a Commit returned by the SysML V2 API.</summary>
public class SysMLCommit
{
    [JsonPropertyName("@id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("@type")]
    public string Type { get; set; } = "Commit";

    [JsonPropertyName("alias")]
    public List<string>? Alias { get; set; }

    [JsonPropertyName("created")]
    public string? Created { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("owningProject")]
    public SysMLRef? OwningProject { get; set; }

    [JsonPropertyName("previousCommit")]
    public JsonElement? PreviousCommit { get; set; }
}

/// <summary>Request body sent to POST /projects/{projectId}/commits.</summary>
public class CommitRequest
{
    [JsonPropertyName("@type")]
    public string Type { get; set; } = "Commit";

    [JsonPropertyName("change")]
    public List<DataVersionRequest> Change { get; set; } = new();
}

/// <summary>A single change entry within a CommitRequest (DataVersion).</summary>
public class DataVersionRequest
{
    [JsonPropertyName("@type")]
    public string Type { get; set; } = "DataVersion";

    /// <summary>For element deletion: reference to the element identity to delete.</summary>
    [JsonPropertyName("identity")]
    public SysMLRef? Identity { get; set; }

    /// <summary>The element payload to create or update. Null for deletions.</summary>
    [JsonPropertyName("payload")]
    public JsonElement? Payload { get; set; }
}
