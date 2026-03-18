using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Products;

namespace SupperChainErpDemo.Web.Controllers;

public class ProductsController : Controller
{
    private readonly DemoDataStore _dataStore;
    private readonly INotificationService _notificationService;
    private readonly IProductService _productService;

    public ProductsController(
        IProductService productService,
        DemoDataStore dataStore,
        INotificationService notificationService)
    {
        _productService = productService;
        _dataStore = dataStore;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? statusFilter, string? categoryFilter)
    {
        return View(new ProductIndexViewModel
        {
            StatusFilter = statusFilter,
            CategoryFilter = categoryFilter,
            Products = _productService.GetAll(statusFilter, categoryFilter),
            CategoryNames = _dataStore.Categories.ToDictionary(category => category.CategoryId, category => category.CategoryName)
        });
    }

    public IActionResult Details(string id)
    {
        var product = _productService.GetById(id);
        if (product is null)
        {
            _notificationService.Error("Product not found", "The requested product does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CategoryName = _dataStore.Categories.FirstOrDefault(category => category.CategoryId == product.CategoryId)?.CategoryName ?? "Unknown";
        return View(product);
    }

    public IActionResult Create() => View(BuildForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProductFormViewModel model)
    {
        model = BuildForm(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = _productService.Create(model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Product created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var product = _productService.GetById(id);
        if (product is null)
        {
            _notificationService.Error("Product not found", "The requested product does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(BuildForm(new ProductFormViewModel
        {
            CategoryId = product.CategoryId,
            ProductName = product.ProductName,
            Barcode = product.Barcode,
            BasePrice = product.BasePrice,
            Description = product.Description
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, ProductFormViewModel model)
    {
        model = BuildForm(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = _productService.Update(id, model);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Product updated", result.Message);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeStatus(string id, RecordStatus status)
    {
        var result = _productService.ChangeStatus(id, status);
        if (!result.Succeeded)
        {
            _notificationService.Error("Product status blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("Product status updated", result.Message);
        return RedirectToAction(nameof(Index));
    }

    private ProductFormViewModel BuildForm(ProductFormViewModel? model = null)
    {
        model ??= new ProductFormViewModel();
        model.CategoryOptions = _dataStore.Categories
            .Where(category => category.Status == RecordStatus.Active || category.CategoryId == model.CategoryId)
            .OrderBy(category => category.CategoryName)
            .Select(category => new SelectListItem(category.CategoryName, category.CategoryId))
            .ToList();
        return model;
    }
}
