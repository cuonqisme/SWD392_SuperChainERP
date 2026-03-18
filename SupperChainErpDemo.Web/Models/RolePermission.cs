namespace SupperChainErpDemo.Web.Models;

public class RolePermission
{
    public required string RoleId { get; set; }

    public required string PermissionCode { get; set; }

    public Role? Role { get; set; }
}
