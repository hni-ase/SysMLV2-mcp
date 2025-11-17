using System;
using System.Text.Json;
using Org.OpenAPITools.Model;

public class ProjectSerializationTest
{
    [Fact]
    public void TestSerialization()
    {
        string json = @"{
            ""@id"": ""0a5e28cc-97d3-4f4f-9f49-89bcd4ef65b3"",
            ""@type"": ""Project"",
            ""alias"": [
                ""string""
            ],
            ""created"": ""2025-10-22T08:37:08.158329Z"",
            ""defaultBranch"": {
                ""@id"": ""e590cd47-7ae7-4a24-b66f-c40b7512f119""
            },
            ""description"": ""string"",
            ""name"": ""string""
        }";

        try
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new ProjectJsonConverter() }
            };
            
            Project? project = JsonSerializer.Deserialize<Project>(json, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}