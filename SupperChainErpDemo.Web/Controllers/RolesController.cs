using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Roles;

namespace SupperChainErpDemo.Web.Controllers;

public class RolesController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService, INotificationService notificationService)
    {
        _roleService = roleService;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? statusFilter)
    {
        return View(new RoleIndexViewModel
        {
            StatusFilter = statusFilter,
            Roles = _roleService.GetAll(statusFilter)
        });
    }

    public IActionResult Details(string id)
    {
        var role = _roleService.GetById(id);
        if (role is null)
        {
            _notificationService.Error("Role not found", "The requested role does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(role);
    }

    public IActionResult Create() => View(BuildForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(RoleFormViewModel model)
    {
        model = BuildForm(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = _roleService.Create(model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Role created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var role = _roleService.GetById(id);
        if (role is null)
        {
            _notificationService.Error("Role not found", "The requested role does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(BuildForm(new RoleFormViewModel
        {
            RoleName = role.RoleName,
            Description = role.Description,
            SelectedPermissions = role.Permissions.ToList()
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, RoleFormViewModel model)
    {
        model = BuildForm(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = _roleService.Update(id, model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Role updated", result.Message);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeStatus(string id, RecordStatus status)
    {
        var result = _roleService.ChangeStatus(id, status);
        if (!result.Succeeded)
        {
            _notificationService.Error("Role status blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("Role status updated", result.Message);
        return RedirectToAction(nameof(Index));
    }

    private RoleFormViewModel BuildForm(RoleFormViewModel? model = null)
    {
        model ??= new RoleFormViewModel();
        model.AvailablePermissions = _roleService.GetPermissionCatalog();
        return model;
    }
}
