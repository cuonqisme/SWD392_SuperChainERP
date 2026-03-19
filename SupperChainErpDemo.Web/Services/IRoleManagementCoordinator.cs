using SupperChainErpDemo.Web.ViewModels.Roles;

namespace SupperChainErpDemo.Web.Services;

public interface IRoleManagementCoordinator
{
    RoleIndexViewModel ShowRoleList(string? statusFilter = null);

    Models.Role? ShowRoleDetails(string id);

    RoleFormViewModel PrepareCreateRole();

    RoleFormViewModel? PrepareUpdateRole(string id);

    ServiceResult CreateRole(RoleFormViewModel model);

    ServiceResult UpdateRole(string id, RoleFormViewModel model);

    ServiceResult UpdateRoleStatus(string id, Models.RecordStatus status);
}
