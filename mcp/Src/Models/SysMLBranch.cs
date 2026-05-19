using System.Text.Json.Serialization;

namespace SysMLV2.MCP.Models;

/// <summary>Represents a SysML V2 Branch returned by the API.</summary>
public class SysMLBranch
{
    [JsonPropertyName("@id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("@type")]
    public string Type { get; set; } = "Branch";

    [JsonPropertyName("alias")]
    public List<string>? Alias { get; set; }

    [JsonPropertyName("created")]
    public string? Created { get; set; }

    [JsonPropertyName("deleted")]
    public string? Deleted { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("head")]
    public SysMLRef? Head { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("owningProject")]
    public SysMLRef? OwningProject { get; set; }

    [JsonPropertyName("referencedCommit")]
    public SysMLRef? ReferencedCommit { get; set; }
}
