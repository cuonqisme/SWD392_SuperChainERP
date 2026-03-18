using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Roles;

namespace SupperChainErpDemo.Web.Services;

public interface IRoleService
{
    RoleIndexViewModel GetRoleList(string? statusFilter = null);

    Role? GetRoleDetails(string id);

    RoleFormViewModel PrepareCreateRole();

    RoleFormViewModel? PrepareUpdateRole(string id);

    ServiceResult CreateRole(RoleFormViewModel model);

    ServiceResult UpdateRole(string id, RoleFormViewModel model);

    ServiceResult DeactivateRole(string id);
}
