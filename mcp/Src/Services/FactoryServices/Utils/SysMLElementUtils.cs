
using Org.OpenAPITools.Model;

namespace mcp.Src.Services.FactoryServices.Utils;


public static class SysMLElementUtils
{


    public static string GetName(this Element element)
    {
        var nameKeyValuePair = element.AdditionalProperties.FirstOrDefault(prop => prop.Key == "name");
        return nameKeyValuePair.Value.GetString() ?? "N/A";
    }
    


}