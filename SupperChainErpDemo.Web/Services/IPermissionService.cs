namespace SupperChainErpDemo.Web.Services;

public interface IPermissionService
{
    IReadOnlyList<string> GetCatalog();

    string? ValidateSelection(IReadOnlyCollection<string> selectedPermissions);
}
