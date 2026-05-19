using System.Text.Json.Serialization;

namespace SysMLV2.MCP.Models;

/// <summary>
/// Represents a JSON reference of the form {"@id": "uuid"} used throughout the SysML V2 API.
/// </summary>
public class SysMLRef
{
    [JsonPropertyName("@id")]
    public Guid Id { get; set; }

    public SysMLRef() { }

    public SysMLRef(Guid id) { Id = id; }
}
