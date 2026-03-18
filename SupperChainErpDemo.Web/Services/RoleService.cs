using Microsoft.EntityFrameworkCore;
using SupperChainErpDemo.Web.Data;
using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Roles;

namespace SupperChainErpDemo.Web.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _dbContext;
    private readonly IPermissionService _permissionService;

    public RoleService(AppDbContext dbContext, IPermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public RoleIndexViewModel GetRoleList(string? statusFilter = null)
    {
        var query = _dbContext.Roles.AsNoTracking().AsEnumerable();

        if (Enum.TryParse<RecordStatus>(statusFilter, true, out var status))
        {
            query = query.Where(role => role.Status == status);
        }

        var rolePermissions = _dbContext.RolePermissions
            .AsNoTracking()
            .GroupBy(item => item.RoleId)
            .ToDictionary(group => group.Key, group => group.Select(item => item.PermissionCode).Order().ToList());

        var roles = query.OrderBy(role => role.RoleName).ToList();
        foreach (var role in roles)
        {
            role.Permissions = rolePermissions.GetValueOrDefault(role.RoleId, []);
        }

        return new RoleIndexViewModel
        {
            StatusFilter = statusFilter,
            Roles = roles
        };
    }

    public Role? GetRoleDetails(string id) =>
        LoadRole(id);

    public RoleFormViewModel PrepareCreateRole() => new()
    {
        AvailablePermissions = _permissionService.GetCatalog()
    };

    public RoleFormViewModel? PrepareUpdateRole(string id)
    {
        var role = GetRoleDetails(id);
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

    public ServiceResult CreateRole(RoleFormViewModel model)
    {
        var validationError = Validate(model);
        if (validationError is not null)
        {
            return ServiceResult.Failure(validationError);
        }

        var selectedPermissions = model.SelectedPermissions
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order()
            .ToList();

        var role = new Role
        {
            RoleId = IdGenerator.NextId(_dbContext.Roles.AsNoTracking().Select(item => item.RoleId).ToList(), "ROL-"),
            RoleName = model.RoleName.Trim(),
            Description = model.Description.Trim(),
            Status = RecordStatus.Active,
            UpdatedDate = DateTime.UtcNow
        };

        _dbContext.Roles.Add(role);
        _dbContext.RolePermissions.AddRange(selectedPermissions.Select(permission => new RolePermission
            {
                RoleId = role.RoleId,
                PermissionCode = permission
            }));
        _dbContext.SaveChanges();
        return ServiceResult.Success($"Role {role.RoleName} was created and linked with {selectedPermissions.Count} permission(s).");
    }

    public ServiceResult UpdateRole(string id, RoleFormViewModel model)
    {
        var role = GetRoleDetails(id);
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
        role.UpdatedDate = DateTime.UtcNow;
        var selectedPermissions = model.SelectedPermissions.Distinct(StringComparer.OrdinalIgnoreCase).Order().ToList();
        var currentPermissions = _dbContext.RolePermissions.Where(item => item.RoleId == role.RoleId);
        _dbContext.RolePermissions.RemoveRange(currentPermissions);
        _dbContext.RolePermissions.AddRange(selectedPermissions.Select(permission => new RolePermission
        {
            RoleId = role.RoleId,
            PermissionCode = permission
        }));
        _dbContext.SaveChanges();

        return ServiceResult.Success($"Role {role.RoleName} was updated successfully.");
    }

    public ServiceResult DeactivateRole(string id)
    {
        var role = GetRoleDetails(id);
        if (role is null)
        {
            return ServiceResult.Failure("The selected role could not be found.");
        }

        if (_dbContext.Users.Any(user => user.RoleId == role.RoleId && user.Status == RecordStatus.Active))
        {
            return ServiceResult.Failure("This role is still assigned to active users. Reassign or deactivate those users first.");
        }

        role.Status = RecordStatus.Inactive;
        role.UpdatedDate = DateTime.UtcNow;
        _dbContext.SaveChanges();
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

        var nameExists = _dbContext.Roles.Any(role =>
            !role.RoleId.Equals(currentId, StringComparison.OrdinalIgnoreCase) &&
            role.RoleName.Equals(normalizedName, StringComparison.OrdinalIgnoreCase));

        return nameExists ? "Role name must be unique." : null;
    }

    private Role? LoadRole(string id)
    {
        var role = _dbContext.Roles.FirstOrDefault(item => item.RoleId.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (role is null)
        {
            return null;
        }

        role.Permissions = _dbContext.RolePermissions
            .Where(item => item.RoleId == role.RoleId)
            .Select(item => item.PermissionCode)
            .OrderBy(item => item)
            .ToList();

        return role;
    }
}
