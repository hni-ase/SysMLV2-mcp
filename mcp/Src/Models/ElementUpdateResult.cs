using System.Text.Json.Serialization;

namespace SysMLV2.MCP.Models;

/// <summary>
/// Reports the outcome of an UpdateElementAttributes operation.
/// </summary>
public class ElementUpdateResult
{
    [JsonPropertyName("elementId")]
    public string ElementId { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    /// <summary>Attribute names that were successfully applied to the element.</summary>
    [JsonPropertyName("updatedAttributes")]
    public List<string> UpdatedAttributes { get; set; } = new();

    /// <summary>
    /// Attribute names that were rejected because they do not exist in the element's
    /// schema (or are read-only, e.g. @id).
    /// </summary>
    [JsonPropertyName("invalidAttributes")]
    public List<string> InvalidAttributes { get; set; } = new();

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
