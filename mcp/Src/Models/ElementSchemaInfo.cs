using System.Text.Json.Serialization;

namespace SysMLV2.MCP.Models;

/// <summary>
/// Describes the schema-defined attributes of a SysML V2 element type.
/// RequiredAttributes and OptionalAttributes are keyed by attribute name
/// with the JSON Schema type description as the value.
/// </summary>
public class ElementSchemaInfo
{
    [JsonPropertyName("elementId")]
    public string ElementId { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    /// <summary>
    /// Attributes listed in the schema's required arrays, with their type descriptions.
    /// </summary>
    [JsonPropertyName("requiredAttributes")]
    public Dictionary<string, string> RequiredAttributes { get; set; } = new();

    /// <summary>
    /// Attributes present in schema properties but not in any required array.
    /// These are typically derived/computed or fully optional fields.
    /// </summary>
    [JsonPropertyName("optionalAttributes")]
    public Dictionary<string, string> OptionalAttributes { get; set; } = new();
}
