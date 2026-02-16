using ModelContextProtocol.Server;

namespace Src.Resources
{

    [McpServerResourceType]
    public class SysMLV2DatabaseResource
    {

        [McpServerResource(MimeType = "application/json", Name = "GetInformationFromSysMLV2", Title = "Sysml V2 Databse resource", UriTemplate ="")]
        public static string GetSysMLV2Data()
        {
            return "";
        }
        

    }
    
}