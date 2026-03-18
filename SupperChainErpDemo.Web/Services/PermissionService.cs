using SupperChainErpDemo.Web.Data;

namespace SupperChainErpDemo.Web.Services;

public class PermissionService : IPermissionService
{
    public IReadOnlyList<string> GetCatalog() => DbSeeder.PermissionCatalog;

    public string? ValidateSelection(IReadOnlyCollection<string> selectedPermissions)
    {
        if (selectedPermissions.Count == 0)
        {
            return "Pick at least one permission so the role can be used in access control.";
        }

        var unknownPermission = selectedPermissions
            .FirstOrDefault(permission => !DbSeeder.PermissionCatalog.Contains(permission, StringComparer.OrdinalIgnoreCase));

        return unknownPermission is null ? null : $"Permission {unknownPermission} is not part of the system catalog.";
    }
}
