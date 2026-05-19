using System.Text.Json.Serialization;

namespace SysMLV2.MCP.Models;

/// <summary>Represents a SysML V2 Project returned by the API.</summary>
public class SysMLProject
{
    [JsonPropertyName("@id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("@type")]
    public string Type { get; set; } = "Project";

    [JsonPropertyName("alias")]
    public List<string>? Alias { get; set; }

    [JsonPropertyName("created")]
    public string? Created { get; set; }

    [JsonPropertyName("defaultBranch")]
    public SysMLRef? DefaultBranch { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
