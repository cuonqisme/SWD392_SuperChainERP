using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Categories;

namespace SupperChainErpDemo.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly INotificationService _notificationService;

    public CategoriesController(
        ICategoryService categoryService,
        INotificationService notificationService)
    {
        _categoryService = categoryService;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? statusFilter)
    {
        return View(_categoryService.BuildIndex(statusFilter));
    }

    public IActionResult Details(string id)
    {
        var viewModel = _categoryService.BuildDetails(id);
        if (viewModel is null)
        {
            _notificationService.Error("Category not found", "The requested category does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    public IActionResult Create() => View(_categoryService.BuildCreateForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = _categoryService.Create(model);
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
        var viewModel = _categoryService.BuildEditForm(id);
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

        var result = _categoryService.Update(id, model);
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
        var result = _categoryService.Deactivate(id);
        if (!result.Succeeded)
        {
            _notificationService.Error("Category status blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("Category status updated", result.Message);
        return RedirectToAction(nameof(Index));
    }
}
