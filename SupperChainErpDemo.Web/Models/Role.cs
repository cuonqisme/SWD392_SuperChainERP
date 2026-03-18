namespace SupperChainErpDemo.Web.Models;

public class Role
{
    public required string RoleId { get; set; }

    public required string RoleName { get; set; }

    public string Description { get; set; } = string.Empty;

    public RecordStatus Status { get; set; } = RecordStatus.Active;

    public List<string> Permissions { get; set; } = [];

    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
