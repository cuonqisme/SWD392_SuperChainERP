using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Roles;

namespace SupperChainErpDemo.Web.Controllers;

public class RolesController : Controller
{
    private readonly IRoleManagementCoordinator _roleManagementCoordinator;
    private readonly INotificationService _notificationService;

    public RolesController(IRoleManagementCoordinator roleManagementCoordinator, INotificationService notificationService)
    {
        _roleManagementCoordinator = roleManagementCoordinator;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? statusFilter)
    {
        return View(_roleManagementCoordinator.ShowRoleList(statusFilter));
    }

    public IActionResult Details(string id)
    {
        var role = _roleManagementCoordinator.ShowRoleDetails(id);
        if (role is null)
        {
            _notificationService.Error("Role not found", "The requested role does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(role);
    }

    public IActionResult Create() => View(_roleManagementCoordinator.PrepareCreateRole());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(RoleFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = MergeForm(_roleManagementCoordinator.PrepareCreateRole(), model);
            return View(model);
        }

        var result = _roleManagementCoordinator.CreateRole(model);
        if (!result.Succeeded)
        {
            model = MergeForm(_roleManagementCoordinator.PrepareCreateRole(), model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Role created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var viewModel = _roleManagementCoordinator.PrepareUpdateRole(id);
        if (viewModel is null)
        {
            _notificationService.Error("Role not found", "The requested role does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, RoleFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = MergeForm(_roleManagementCoordinator.PrepareUpdateRole(id) ?? _roleManagementCoordinator.PrepareCreateRole(), model);
            return View(model);
        }

        var result = _roleManagementCoordinator.UpdateRole(id, model);
        if (!result.Succeeded)
        {
            model = MergeForm(_roleManagementCoordinator.PrepareUpdateRole(id) ?? _roleManagementCoordinator.PrepareCreateRole(), model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Role updated", result.Message);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Deactivate(string id)
    {
        var result = _roleManagementCoordinator.DeactivateRole(id);
        if (!result.Succeeded)
        {
            _notificationService.Error("Role status blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("Role status updated", result.Message);
        return RedirectToAction(nameof(Index));
    }

    private static RoleFormViewModel MergeForm(RoleFormViewModel target, RoleFormViewModel source)
    {
        target.RoleName = source.RoleName;
        target.Description = source.Description;
        target.SelectedPermissions = source.SelectedPermissions;
        return target;
    }
}
