using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Categories;

namespace SupperChainErpDemo.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryManagementCoordinator _categoryManagementCoordinator;
    private readonly ICategoryService _categoryService;
    private readonly INotificationService _notificationService;

    public CategoriesController(
        ICategoryManagementCoordinator categoryManagementCoordinator,
        ICategoryService categoryService,
        INotificationService notificationService)
    {
        _categoryManagementCoordinator = categoryManagementCoordinator;
        _categoryService = categoryService;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? statusFilter)
    {
        return View(_categoryManagementCoordinator.ShowCategoryList(statusFilter));
    }

    public IActionResult Details(string id)
    {
        var viewModel = _categoryManagementCoordinator.ShowCategoryDetails(id);
        if (viewModel is null)
        {
            _notificationService.Error("Category not found", "The requested category does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    public IActionResult Create() => View(_categoryManagementCoordinator.PrepareCreateCategory());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = _categoryManagementCoordinator.CreateCategory(model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Category created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var viewModel = _categoryManagementCoordinator.PrepareUpdateCategory(id);
        if (viewModel is null)
        {
            _notificationService.Error("Category not found", "The requested category does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = _categoryManagementCoordinator.UpdateCategory(id, model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Category updated", result.Message);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Deactivate(string id)
    {
        var result = _categoryManagementCoordinator.DeactivateCategory(id);
        if (!result.Succeeded)
        {
            _notificationService.Error("Category status blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("Category status updated", result.Message);
        return RedirectToAction(nameof(Index));
    }
}
