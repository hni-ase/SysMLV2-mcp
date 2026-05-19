using System.Text.Json.Serialization;

namespace SysMLV2.MCP.Models;

/// <summary>
/// Reports the outcome of a CreateElementOfType operation.
/// </summary>
public class ElementCreationResult
{
    /// <summary>The newly generated element ID.</summary>
    [JsonPropertyName("elementId")]
    public string ElementId { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    /// <summary>The parent element ID the new element was nested under, if any.</summary>
    [JsonPropertyName("parentId")]
    public string? ParentId { get; set; }

    /// <summary>Attribute names that were applied to the new element.</summary>
    [JsonPropertyName("appliedAttributes")]
    public List<string> AppliedAttributes { get; set; } = new();

    /// <summary>
    /// Attribute names that were excluded because they are not defined in the schema
    /// for this element type, or are reserved (@id, @type).
    /// </summary>
    [JsonPropertyName("invalidAttributes")]
    public List<string> InvalidAttributes { get; set; } = new();

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
