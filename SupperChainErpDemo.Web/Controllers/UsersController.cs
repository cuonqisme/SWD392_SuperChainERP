using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Users;

namespace SupperChainErpDemo.Web.Controllers;

public class UsersController : Controller
{
    private readonly IUserManagementCoordinator _userManagementCoordinator;
    private readonly INotificationService _notificationService;

    public UsersController(
        IUserManagementCoordinator userManagementCoordinator,
        INotificationService notificationService)
    {
        _userManagementCoordinator = userManagementCoordinator;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? keyword, string? statusFilter)
    {
        return View(_userManagementCoordinator.ShowUserList(keyword, statusFilter));
    }

    public IActionResult Details(string id)
    {
        var viewModel = _userManagementCoordinator.ShowUserDetails(id);
        if (viewModel is null)
        {
            _notificationService.Error("User not found", "The requested user does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    public IActionResult Create() => View(_userManagementCoordinator.PrepareCreateUser());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UserFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = MergeForm(_userManagementCoordinator.PrepareCreateUser(), model);
            return View(model);
        }

        var result = _userManagementCoordinator.CreateUser(model);
        if (!result.Succeeded)
        {
            model = MergeForm(_userManagementCoordinator.PrepareCreateUser(), model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("User created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var viewModel = _userManagementCoordinator.PrepareUpdateUser(id);
        if (viewModel is null)
        {
            _notificationService.Error("User not found", "The requested user does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, UserFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = MergeForm(_userManagementCoordinator.PrepareUpdateUser(id) ?? _userManagementCoordinator.PrepareCreateUser(), model);
            return View(model);
        }

        var result = _userManagementCoordinator.UpdateUser(id, model);
        if (!result.Succeeded)
        {
            model = MergeForm(_userManagementCoordinator.PrepareUpdateUser(id) ?? _userManagementCoordinator.PrepareCreateUser(), model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("User updated", result.Message);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeStatus(string id, RecordStatus status)
    {
        var result = _userManagementCoordinator.UpdateUserStatus(id, status);
        if (!result.Succeeded)
        {
            _notificationService.Error("User status blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("User status updated", result.Message);
        return RedirectToAction(nameof(Index));
    }

    private static UserFormViewModel MergeForm(UserFormViewModel target, UserFormViewModel source)
    {
        target.RoleId = source.RoleId;
        target.Username = source.Username;
        target.Password = source.Password;
        target.FullName = source.FullName;
        target.Email = source.Email;
        target.Phone = source.Phone;
        return target;
    }
}
