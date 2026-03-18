namespace SupperChainErpDemo.Web.Services;

public class PermissionService : IPermissionService
{
    private readonly DemoDataStore _dataStore;

    public PermissionService(DemoDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IReadOnlyList<string> GetCatalog() => _dataStore.PermissionCatalog;

    public string? ValidateSelection(IReadOnlyCollection<string> selectedPermissions)
    {
        if (selectedPermissions.Count == 0)
        {
            return "Pick at least one permission so the role can be used in access control.";
        }

        var unknownPermission = selectedPermissions
            .FirstOrDefault(permission => !_dataStore.PermissionCatalog.Contains(permission, StringComparer.OrdinalIgnoreCase));

        return unknownPermission is null ? null : $"Permission {unknownPermission} is not part of the system catalog.";
    }
}
