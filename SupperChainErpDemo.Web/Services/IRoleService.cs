using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Roles;

namespace SupperChainErpDemo.Web.Services;

public interface IRoleService
{
    IReadOnlyList<Role> GetAll(string? statusFilter = null);

    Role? GetById(string id);

    IReadOnlyList<string> GetPermissionCatalog();

    ServiceResult Create(RoleFormViewModel model);

    ServiceResult Update(string id, RoleFormViewModel model);

    ServiceResult ChangeStatus(string id, RecordStatus status);
}
