using SupperChainErpDemo.Web.ViewModels.Roles;

namespace SupperChainErpDemo.Web.Services;

public class RoleManagementCoordinator : IRoleManagementCoordinator
{
    private readonly IRoleService _roleService;

    public RoleManagementCoordinator(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public RoleIndexViewModel ShowRoleList(string? statusFilter = null) =>
        _roleService.GetRoleList(statusFilter);

    public Models.Role? ShowRoleDetails(string id) =>
        _roleService.GetRoleDetails(id);

    public RoleFormViewModel PrepareCreateRole() =>
        _roleService.PrepareCreateRole();

    public RoleFormViewModel? PrepareUpdateRole(string id) =>
        _roleService.PrepareUpdateRole(id);

    public ServiceResult CreateRole(RoleFormViewModel model) =>
        _roleService.CreateRole(model);

    public ServiceResult UpdateRole(string id, RoleFormViewModel model) =>
        _roleService.UpdateRole(id, model);

    public ServiceResult DeactivateRole(string id) =>
        _roleService.DeactivateRole(id);
}
