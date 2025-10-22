using System.Reflection.Metadata;
using Org.OpenAPITools.Model;

namespace mcp.Src.Models
{
    public class Package: Data
    {
        public string Name { get; set; }
        public string Documentation { get; set; }
    }
}