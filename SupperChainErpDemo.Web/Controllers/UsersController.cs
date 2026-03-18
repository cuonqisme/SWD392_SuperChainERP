using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Users;

namespace SupperChainErpDemo.Web.Controllers;

public class UsersController : Controller
{
    private readonly DemoDataStore _dataStore;
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;

    public UsersController(
        IUserService userService,
        DemoDataStore dataStore,
        INotificationService notificationService)
    {
        _userService = userService;
        _dataStore = dataStore;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? keyword, string? statusFilter)
    {
        return View(new UserIndexViewModel
        {
            Keyword = keyword,
            StatusFilter = statusFilter,
            Users = _userService.GetAll(keyword, statusFilter),
            RoleNames = _dataStore.Roles.ToDictionary(role => role.RoleId, role => role.RoleName)
        });
    }

    public IActionResult Details(string id)
    {
        var user = _userService.GetById(id);
        if (user is null)
        {
            _notificationService.Error("User not found", "The requested user does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        ViewBag.RoleName = _dataStore.Roles.FirstOrDefault(role => role.RoleId == user.RoleId)?.RoleName ?? "Unknown";
        return View(user);
    }

    public IActionResult Create() => View(BuildForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UserFormViewModel model)
    {
        model = BuildForm(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = _userService.Create(model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("User created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var user = _userService.GetById(id);
        if (user is null)
        {
            _notificationService.Error("User not found", "The requested user does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(BuildForm(new UserFormViewModel
        {
            RoleId = user.RoleId,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, UserFormViewModel model)
    {
        model = BuildForm(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = _userService.Update(id, model);
        if (!result.Succeeded)
        {
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

    private UserFormViewModel BuildForm(UserFormViewModel? model = null)
    {
        model ??= new UserFormViewModel();
        model.RoleOptions = _dataStore.Roles
            .Where(role => role.Status == RecordStatus.Active || role.RoleId == model.RoleId)
            .OrderBy(role => role.RoleName)
            .Select(role => new SelectListItem(role.RoleName, role.RoleId))
            .ToList();
        return model;
    }
}
