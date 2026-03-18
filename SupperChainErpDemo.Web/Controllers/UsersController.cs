using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Users;

namespace SupperChainErpDemo.Web.Controllers;

public class UsersController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;

    public UsersController(
        IUserService userService,
        INotificationService notificationService)
    {
        _userService = userService;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? keyword, string? statusFilter)
    {
        return View(_userService.BuildIndex(keyword, statusFilter));
    }

    public IActionResult Details(string id)
    {
        var viewModel = _userService.BuildDetails(id);
        if (viewModel is null)
        {
            _notificationService.Error("User not found", "The requested user does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    public IActionResult Create() => View(_userService.BuildCreateForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UserFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = MergeForm(_userService.BuildCreateForm(), model);
            return View(model);
        }

        var result = _userService.Create(model);
        if (!result.Succeeded)
        {
            model = MergeForm(_userService.BuildCreateForm(), model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("User created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var viewModel = _userService.BuildEditForm(id);
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
            model = MergeForm(_userService.BuildEditForm(id) ?? _userService.BuildCreateForm(), model);
            return View(model);
        }

        var result = _userService.Update(id, model);
        if (!result.Succeeded)
        {
            model = MergeForm(_userService.BuildEditForm(id) ?? _userService.BuildCreateForm(), model);
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
        var result = _userService.ChangeStatus(id, status);
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
