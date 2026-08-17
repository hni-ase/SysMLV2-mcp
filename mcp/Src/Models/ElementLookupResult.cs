namespace SysMLV2.MCP.Models;

public class ElementLookupResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? OwnerId { get; set; }
    public string? OwningNamespaceId { get; set; }
    public string? OwningMembershipId { get; set; }
}