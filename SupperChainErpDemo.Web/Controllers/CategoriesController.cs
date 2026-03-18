using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Categories;

namespace SupperChainErpDemo.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly DemoDataStore _dataStore;
    private readonly ICategoryService _categoryService;
    private readonly INotificationService _notificationService;

    public CategoriesController(
        ICategoryService categoryService,
        DemoDataStore dataStore,
        INotificationService notificationService)
    {
        _categoryService = categoryService;
        _dataStore = dataStore;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? statusFilter)
    {
        return View(new CategoryIndexViewModel
        {
            StatusFilter = statusFilter,
            Categories = _categoryService.GetAll(statusFilter),
            ProductCountByCategory = _dataStore.Products
                .GroupBy(product => product.CategoryId)
                .ToDictionary(group => group.Key, group => group.Count())
        });
    }

    public IActionResult Details(string id)
    {
        var category = _categoryService.GetById(id);
        if (category is null)
        {
            _notificationService.Error("Category not found", "The requested category does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Products = _dataStore.Products
            .Where(product => product.CategoryId == category.CategoryId)
            .OrderBy(product => product.ProductName)
            .ToList();

        return View(category);
    }

    public IActionResult Create() => View(new CategoryFormViewModel());

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
        var category = _categoryService.GetById(id);
        if (category is null)
        {
            _notificationService.Error("Category not found", "The requested category does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(new CategoryFormViewModel
        {
            CategoryName = category.CategoryName,
            Description = category.Description,
            SkuPrefix = category.SkuPrefix
        });
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
    public IActionResult ChangeStatus(string id, RecordStatus status)
    {
        var result = _categoryService.ChangeStatus(id, status);
        if (!result.Succeeded)
        {
            _notificationService.Error("Category status blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("Category status updated", result.Message);
        return RedirectToAction(nameof(Index));
    }
}
