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
        _roleService.BuildIndex(statusFilter);

    public Models.Role? ShowRoleDetails(string id) =>
        _roleService.GetById(id);

    public RoleFormViewModel PrepareCreateRole() =>
        _roleService.BuildCreateForm();

    public RoleFormViewModel? PrepareUpdateRole(string id) =>
        _roleService.BuildEditForm(id);

    public ServiceResult CreateRole(RoleFormViewModel model) =>
        _roleService.Create(model);

    public ServiceResult UpdateRole(string id, RoleFormViewModel model) =>
        _roleService.Update(id, model);

    public ServiceResult DeactivateRole(string id) =>
        _roleService.Deactivate(id);
}
