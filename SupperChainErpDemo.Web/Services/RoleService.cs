using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Roles;

namespace SupperChainErpDemo.Web.Services;

public class RoleService : IRoleService
{
    private readonly DemoDataStore _dataStore;
    private readonly IPermissionService _permissionService;

    public RoleService(DemoDataStore dataStore, IPermissionService permissionService)
    {
        _dataStore = dataStore;
        _permissionService = permissionService;
    }

    public RoleIndexViewModel BuildIndex(string? statusFilter = null)
    {
        var query = _dataStore.Roles.AsEnumerable();

        if (Enum.TryParse<RecordStatus>(statusFilter, true, out var status))
        {
            query = query.Where(role => role.Status == status);
        }

        return new RoleIndexViewModel
        {
            StatusFilter = statusFilter,
            Roles = query.OrderBy(role => role.RoleName).ToList()
        };
    }

    public Role? GetById(string id) =>
        _dataStore.Roles.FirstOrDefault(role => role.RoleId.Equals(id, StringComparison.OrdinalIgnoreCase));

    public RoleFormViewModel BuildCreateForm() => new()
    {
        AvailablePermissions = _permissionService.GetCatalog()
    };

    public RoleFormViewModel? BuildEditForm(string id)
    {
        var role = GetById(id);
        if (role is null)
        {
            return null;
        }

        return new RoleFormViewModel
        {
            RoleName = role.RoleName,
            Description = role.Description,
            SelectedPermissions = role.Permissions.ToList(),
            AvailablePermissions = _permissionService.GetCatalog()
        };
    }

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

    public ServiceResult Deactivate(string id)
    {
        var role = GetById(id);
        if (role is null)
        {
            return ServiceResult.Failure("The selected role could not be found.");
        }

        if (_dataStore.Users.Any(user => user.RoleId == role.RoleId && user.Status == RecordStatus.Active))
        {
            return ServiceResult.Failure("This role is still assigned to active users. Reassign or deactivate those users first.");
        }

        role.Status = RecordStatus.Inactive;
        role.UpdatedDate = DateTime.UtcNow;
        return ServiceResult.Success($"Role {role.RoleName} was deactivated successfully.");
    }

    private string? Validate(RoleFormViewModel model, string? currentId = null)
    {
        var normalizedName = model.RoleName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return "Role name is required.";
        }

        var permissionValidation = _permissionService.ValidateSelection(model.SelectedPermissions);
        if (permissionValidation is not null)
        {
            return permissionValidation;
        }

        var nameExists = _dataStore.Roles.Any(role =>
            !role.RoleId.Equals(currentId, StringComparison.OrdinalIgnoreCase) &&
            role.RoleName.Equals(normalizedName, StringComparison.OrdinalIgnoreCase));

        return nameExists ? "Role name must be unique." : null;
    }
}
