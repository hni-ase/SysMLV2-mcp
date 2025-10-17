using System.Text.Json;
using System.Text.Json.Nodes;

namespace mcp.Src.Services
{
    public class SysMLOpenApiFactory
    {
        private readonly Dictionary<string, JsonNode> _schemas;
        private readonly string _schemasPath;

        public SysMLOpenApiFactory(string? schemasPath = null)
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

            // Get properties from schema
            var properties = schema?["properties"]?.AsObject();
            if (properties == null)
            {
                return "{}";
            }

            foreach (var property in properties)
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
            var properties = schema?["properties"]?.AsObject();
            
            if (properties == null)
            {
                return new Dictionary<string, string>();
            }

            var result = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                var type = property.Value?["type"]?.GetValue<string>() ?? "unknown";
                result[property.Key] = type;
            }

            return result;
        }
    }
}