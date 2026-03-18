using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Roles;

namespace SupperChainErpDemo.Web.Services;

public class RoleService : IRoleService
{
    private readonly DemoDataStore _dataStore;

    public RoleService(DemoDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IReadOnlyList<Role> GetAll(string? statusFilter = null)
    {
        var query = _dataStore.Roles.AsEnumerable();

        if (Enum.TryParse<RecordStatus>(statusFilter, true, out var status))
        {
            query = query.Where(role => role.Status == status);
        }

        return query.OrderBy(role => role.RoleName).ToList();
    }

    public Role? GetById(string id) =>
        _dataStore.Roles.FirstOrDefault(role => role.RoleId.Equals(id, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<string> GetPermissionCatalog() => _dataStore.PermissionCatalog;

    public ServiceResult Create(RoleFormViewModel model)
    {
        var validationError = Validate(model);
        if (validationError is not null)
        {
            return ServiceResult.Failure(validationError);
        }

        var role = new Role
        {
            RoleId = _dataStore.NextRoleId(),
            RoleName = model.RoleName.Trim(),
            Description = model.Description.Trim(),
            Status = RecordStatus.Active,
            Permissions = model.SelectedPermissions.Distinct(StringComparer.OrdinalIgnoreCase).Order().ToList(),
            UpdatedDate = DateTime.UtcNow
        };

        _dataStore.Roles.Add(role);
        return ServiceResult.Success($"Role {role.RoleName} was created and linked with {role.Permissions.Count} permission(s).");
    }

    public ServiceResult Update(string id, RoleFormViewModel model)
    {
        var role = GetById(id);
        if (role is null)
        {
            return ServiceResult.Failure("The selected role could not be found.");
        }

        var validationError = Validate(model, id);
        if (validationError is not null)
        {
            return ServiceResult.Failure(validationError);
        }

        role.RoleName = model.RoleName.Trim();
        role.Description = model.Description.Trim();
        role.Permissions = model.SelectedPermissions.Distinct(StringComparer.OrdinalIgnoreCase).Order().ToList();
        role.UpdatedDate = DateTime.UtcNow;

        return ServiceResult.Success($"Role {role.RoleName} was updated successfully.");
    }

    public ServiceResult ChangeStatus(string id, RecordStatus status)
    {
        var role = GetById(id);
        if (role is null)
        {
            return ServiceResult.Failure("The selected role could not be found.");
        }

        if (status == RecordStatus.Inactive &&
            _dataStore.Users.Any(user => user.RoleId == role.RoleId && user.Status == RecordStatus.Active))
        {
            return ServiceResult.Failure("This role is still assigned to active users. Reassign or deactivate those users first.");
        }

        role.Status = status;
        role.UpdatedDate = DateTime.UtcNow;
        return ServiceResult.Success($"Role {role.RoleName} status changed to {status}.");
    }

    private string? Validate(RoleFormViewModel model, string? currentId = null)
    {
        var normalizedName = model.RoleName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return "Role name is required.";
        }

        if (model.SelectedPermissions.Count == 0)
        {
            return "Pick at least one permission so the role can be used in access control.";
        }

        var nameExists = _dataStore.Roles.Any(role =>
            !role.RoleId.Equals(currentId, StringComparison.OrdinalIgnoreCase) &&
            role.RoleName.Equals(normalizedName, StringComparison.OrdinalIgnoreCase));

        return nameExists ? "Role name must be unique." : null;
    }
}
