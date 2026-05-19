using System.Text.Json;
using System.Text.Json.Serialization;

namespace SysMLV2.MCP.Models;

/// <summary>
/// Represents any SysML V2 element returned by the API.
/// The @id and @type are explicit properties; all other fields (name, elementId, etc.)
/// are captured in AdditionalProperties via [JsonExtensionData].
/// </summary>
public class SysMLElement
{
    [JsonPropertyName("@id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("@type")]
    public string? Type { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }

    /// <summary>Returns the element's "name" field from AdditionalProperties.</summary>
    public string GetName()
    {
        if (AdditionalProperties != null &&
            AdditionalProperties.TryGetValue("name", out var nameJson) &&
            nameJson.ValueKind == JsonValueKind.String)
        {
            return nameJson.GetString() ?? string.Empty;
        }
        return string.Empty;
    }
}
