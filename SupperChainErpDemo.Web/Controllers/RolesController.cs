using Microsoft.AspNetCore.Mvc;
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
        return View(_roleService.BuildIndex(statusFilter));
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

    public IActionResult Create() => View(_roleService.BuildCreateForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(RoleFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = MergeForm(_roleService.BuildCreateForm(), model);
            return View(model);
        }

        var result = _roleService.Create(model);
        if (!result.Succeeded)
        {
            model = MergeForm(_roleService.BuildCreateForm(), model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Role created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var viewModel = _roleService.BuildEditForm(id);
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
            model = MergeForm(_roleService.BuildEditForm(id) ?? _roleService.BuildCreateForm(), model);
            return View(model);
        }

        var result = _roleService.Update(id, model);
        if (!result.Succeeded)
        {
            model = MergeForm(_roleService.BuildEditForm(id) ?? _roleService.BuildCreateForm(), model);
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
        var result = _roleService.Deactivate(id);
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
