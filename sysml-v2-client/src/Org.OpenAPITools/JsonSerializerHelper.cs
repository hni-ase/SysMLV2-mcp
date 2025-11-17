using System.Text.Json;
using Org.OpenAPITools.Model;

namespace Org.OpenAPITools
{
    /// <summary>
    /// Helper class for configuring JsonSerializerOptions with custom converters
    /// that support ignoring unknown properties during deserialization.
    /// </summary>
    public static class JsonSerializerHelper
    {
        /// <summary>
        /// Creates JsonSerializerOptions configured with all custom converters
        /// that properly handle unknown properties by skipping them during deserialization.
        /// </summary>
        /// <returns>Configured JsonSerializerOptions</returns>
        public static JsonSerializerOptions CreateOptionsWithUnknownPropertyHandling()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Add all custom converters that handle unknown properties
            options.Converters.Add(new ProjectJsonConverter());
            options.Converters.Add(new ProjectDefaultBranchJsonConverter());
            options.Converters.Add(new CommitJsonConverter());
            options.Converters.Add(new BranchJsonConverter());
            options.Converters.Add(new BranchHeadJsonConverter());
            options.Converters.Add(new BranchOwningProjectJsonConverter());
            options.Converters.Add(new DataVersionJsonConverter());

            return options;
        }
    }
}