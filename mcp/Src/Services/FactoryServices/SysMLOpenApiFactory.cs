using System.Text.Json;
using System.Text.Json.Nodes;

namespace mcp.Src.Services
{
    public class SysMLMetaModelFactory
    {
        public readonly Dictionary<string, JsonNode> _schemas;
        private readonly string _schemasPath;

        public SysMLMetaModelFactory(string? schemasPath = null)
        {
            _schemasPath = schemasPath ?? Path.Combine(Directory.GetCurrentDirectory(), "sysmlv2-api-spec", "metamodels");
            _schemas = new Dictionary<string, JsonNode>();
            LoadSchemas();
        }

        private void LoadSchemas()
        {
            if (!Directory.Exists(_schemasPath))
            {
                throw new DirectoryNotFoundException($"Schemas directory not found: {_schemasPath}");
            }

            var jsonFiles = Directory.GetFiles(_schemasPath, "*.json");
            
            foreach (var file in jsonFiles)
            {
                try
                {
                    var content = File.ReadAllText(file);
                    var schema = JsonNode.Parse(content);
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    _schemas[fileName] = schema!;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to load schema {file}: {ex.Message}");
                }
            }
        }

        public string CreateJsonRequestBody(string elementName, params (string propertyName, object? value)[] parameters)
        {
            var parameterDict = parameters.ToDictionary(p => p.propertyName, p => p.value);
            return CreateJsonRequestBody(elementName, parameterDict);
        }

        public string CreateJsonRequestBody(string elementName, Dictionary<string, object?> parameters)
        {
            if (!_schemas.ContainsKey(elementName))
            {
                throw new ArgumentException($"Schema for element '{elementName}' not found. Available schemas: {string.Join(", ", _schemas.Keys)}");
            }

            var schema = _schemas[elementName];
            var result = new JsonObject();

            // Get all properties from schema recursively
            var allProperties = GetAllSchemaProperties(schema);

            foreach (var property in allProperties)
            {
                var propertyName = property.Key;
                var propertySchema = property.Value;

                // Only include property if it's provided in parameters
                if (parameters.ContainsKey(propertyName))
                {
                    var value = parameters[propertyName];
                    var jsonValue = ConvertToJsonValue(value, propertySchema);
                    
                    if (jsonValue != null)
                    {
                        result[propertyName] = jsonValue;
                    }
                }
            }

            return result.ToJsonString(new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        /// <summary>
        /// Gets all properties from a schema, including those in anyOf definitions
        /// </summary>
        /// <param name="schema">The schema JSON node</param>
        /// <returns>Dictionary mapping property names to their schema definitions</returns>
        private Dictionary<string, JsonNode?> GetAllSchemaProperties(JsonNode? schema)
        {
            var result = new Dictionary<string, JsonNode?>();
            
            if (schema == null)
            {
                return result;
            }

            // Get properties from direct properties section
            var directProperties = schema["properties"]?.AsObject();
            if (directProperties != null)
            {
                foreach (var prop in directProperties)
                {
                    result[prop.Key] = prop.Value;
                }
            }

            // Get properties from anyOf definitions
            var anyOfArray = schema["anyOf"]?.AsArray();
            if (anyOfArray != null)
            {
                foreach (var typeDefinition in anyOfArray)
                {
                    if (typeDefinition != null)
                    {
                        var typeProperties = typeDefinition["properties"]?.AsObject();
                        if (typeProperties != null)
                        {
                            foreach (var prop in typeProperties)
                            {
                                // Don't overwrite existing properties
                                if (!result.ContainsKey(prop.Key))
                                {
                                    result[prop.Key] = prop.Value;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private JsonNode? ConvertToJsonValue(object? value, JsonNode? propertySchema)
        {
            if (value == null)
            {
                return null;
            }

            var type = propertySchema?["type"]?.GetValue<string>();
            
            return type switch
            {
                "string" => JsonValue.Create(value.ToString()),
                "integer" => JsonValue.Create(Convert.ToInt32(value)),
                "number" => JsonValue.Create(Convert.ToDouble(value)),
                "boolean" => JsonValue.Create(Convert.ToBoolean(value)),
                "array" => ConvertArrayToJsonArray(value, propertySchema),
                "object" => ConvertObjectToJsonObject(value, propertySchema),
                _ => JsonValue.Create(value.ToString())
            };
        }

        private JsonNode? ConvertArrayToJsonArray(object? value, JsonNode? propertySchema)
        {
            if (value is not System.Collections.IEnumerable enumerable)
            {
                return null;
            }

            var jsonArray = new JsonArray();
            var itemSchema = propertySchema?["items"];

            foreach (var item in enumerable)
            {
                var jsonItem = ConvertToJsonValue(item, itemSchema);
                if (jsonItem != null)
                {
                    jsonArray.Add(jsonItem);
                }
            }

            return jsonArray;
        }

        private JsonNode? ConvertObjectToJsonObject(object? value, JsonNode? propertySchema)
        {
            if (value == null)
            {
                return null;
            }

            // If it's already a dictionary, use it directly
            if (value is Dictionary<string, object?> dict)
            {
                var jsonObject = new JsonObject();
                var objectProperties = propertySchema?["properties"]?.AsObject();

                foreach (var kvp in dict)
                {
                    var propSchema = objectProperties?[kvp.Key];
                    var jsonValue = ConvertToJsonValue(kvp.Value, propSchema);
                    if (jsonValue != null)
                    {
                        jsonObject[kvp.Key] = jsonValue;
                    }
                }
                return jsonObject;
            }

            // For other objects, try to serialize as string or convert to JSON
            return JsonValue.Create(value.ToString());
        }

        public IEnumerable<string> GetAvailableSchemas()
        {
            return _schemas.Keys;
        }

        public JsonNode? GetSchema(string elementName)
        {
            return _schemas.TryGetValue(elementName, out var schema) ? schema : null;
        }

        public Dictionary<string, string> GetSchemaProperties(string elementName)
        {
            if (!_schemas.ContainsKey(elementName))
            {
                return new Dictionary<string, string>();
            }

            var schema = _schemas[elementName];
            return GetSchemaPropertiesRecursive(schema);
        }

        /// <summary>
        /// Recursively extracts all properties from a schema JSON node, including properties from nested type definitions
        /// </summary>
        /// <param name="schemaNode">The schema JSON node to analyze</param>
        /// <param name="visitedTypes">Set of visited type names to prevent infinite recursion</param>
        /// <returns>Dictionary mapping property names to their types</returns>
        public Dictionary<string, string> GetSchemaPropertiesRecursive(JsonNode? schemaNode, HashSet<string>? visitedTypes = null)
        {
            var result = new Dictionary<string, string>();
            
            if (schemaNode == null)
            {
                return result;
            }

            visitedTypes ??= new HashSet<string>();

            // Handle direct properties at the root level
            ExtractPropertiesFromNode(schemaNode, result);

            // Handle anyOf array - contains multiple type definitions
            var anyOfArray = schemaNode["anyOf"]?.AsArray();
            if (anyOfArray != null)
            {
                foreach (var typeDefinition in anyOfArray)
                {
                    if (typeDefinition != null)
                    {
                        ExtractPropertiesFromNode(typeDefinition, result);
                        
                        // Handle references to other schemas
                        var refValue = typeDefinition["$ref"]?.GetValue<string>();
                        if (!string.IsNullOrEmpty(refValue))
                        {
                            var referencedTypeName = ExtractTypeNameFromRef(refValue);
                            if (!string.IsNullOrEmpty(referencedTypeName) && 
                                !visitedTypes.Contains(referencedTypeName) && 
                                _schemas.ContainsKey(referencedTypeName))
                            {
                                visitedTypes.Add(referencedTypeName);
                                var referencedProperties = GetSchemaPropertiesRecursive(_schemas[referencedTypeName], visitedTypes);
                                
                                // Merge properties, giving priority to the current schema's properties
                                foreach (var kvp in referencedProperties)
                                {
                                    if (!result.ContainsKey(kvp.Key))
                                    {
                                        result[kvp.Key] = kvp.Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Handle $defs - nested type definitions
            var defsNode = schemaNode["$defs"]?.AsObject();
            if (defsNode != null)
            {
                foreach (var defPair in defsNode)
                {
                    var defTypeName = defPair.Key;
                    if (!visitedTypes.Contains(defTypeName))
                    {
                        visitedTypes.Add(defTypeName);
                        var defProperties = GetSchemaPropertiesRecursive(defPair.Value, visitedTypes);
                        
                        // Merge properties from definitions
                        foreach (var kvp in defProperties)
                        {
                            if (!result.ContainsKey(kvp.Key))
                            {
                                result[kvp.Key] = kvp.Value;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts properties directly from a JSON node's properties section
        /// </summary>
        /// <param name="node">The JSON node to extract properties from</param>
        /// <param name="result">The dictionary to add extracted properties to</param>
        private void ExtractPropertiesFromNode(JsonNode node, Dictionary<string, string> result)
        {
            var properties = node["properties"]?.AsObject();
            if (properties == null)
            {
                return;
            }

            foreach (var property in properties)
            {
                var propertyName = property.Key;
                var propertySchema = property.Value;
                var propertyType = GetPropertyType(propertySchema);
                
                // Only add if not already present (gives priority to earlier definitions)
                if (!result.ContainsKey(propertyName))
                {
                    result[propertyName] = propertyType;
                }
            }
        }

        /// <summary>
        /// Determines the type of a property from its schema definition
        /// </summary>
        /// <param name="propertySchema">The property schema JSON node</param>
        /// <returns>A string representation of the property type</returns>
        private string GetPropertyType(JsonNode? propertySchema)
        {
            if (propertySchema == null)
            {
                return "unknown";
            }

            // Handle const values first (higher priority than type)
            var constValue = propertySchema["const"]?.GetValue<string>();
            if (!string.IsNullOrEmpty(constValue))
            {
                return $"const:{constValue}";
            }

            // Handle direct type
            var directType = propertySchema["type"]?.GetValue<string>();
            if (!string.IsNullOrEmpty(directType))
            {
                if (directType == "array")
                {
                    var itemsType = GetPropertyType(propertySchema["items"]);
                    return $"array<{itemsType}>";
                }
                return directType;
            }

            // Handle oneOf (union types)
            var oneOfArray = propertySchema["oneOf"]?.AsArray();
            if (oneOfArray != null && oneOfArray.Count > 0)
            {
                var types = new List<string>();
                foreach (var option in oneOfArray)
                {
                    var optionType = GetPropertyType(option);
                    if (!string.IsNullOrEmpty(optionType) && optionType != "unknown")
                    {
                        types.Add(optionType);
                    }
                }
                return types.Count > 0 ? string.Join(" | ", types) : "unknown";
            }

            // Handle $ref (reference to another schema)
            var refValue = propertySchema["$ref"]?.GetValue<string>();
            if (!string.IsNullOrEmpty(refValue))
            {
                var referencedType = ExtractTypeNameFromRef(refValue);
                return !string.IsNullOrEmpty(referencedType) ? $"ref:{referencedType}" : "ref:unknown";
            }

            return "unknown";
        }

        /// <summary>
        /// Extracts the type name from a JSON Schema $ref URL
        /// </summary>
        /// <param name="refUrl">The $ref URL (e.g., "https://www.omg.org/spec/SysML/20250201/Element")</param>
        /// <returns>The extracted type name (e.g., "Element")</returns>
        private string ExtractTypeNameFromRef(string refUrl)
        {
            if (string.IsNullOrEmpty(refUrl))
            {
                return string.Empty;
            }

            // Extract the last part of the URL after the last slash
            var lastSlashIndex = refUrl.LastIndexOf('/');
            return lastSlashIndex >= 0 && lastSlashIndex < refUrl.Length - 1 
                ? refUrl.Substring(lastSlashIndex + 1) 
                : refUrl;
        }
    }
}