using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Products;

namespace SupperChainErpDemo.Web.Controllers;

public class ProductsController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly IProductService _productService;

    public ProductsController(
        IProductService productService,
        INotificationService notificationService)
    {
        _productService = productService;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? statusFilter, string? categoryFilter)
    {
        return View(_productService.BuildIndex(statusFilter, categoryFilter));
    }

    public IActionResult Details(string id)
    {
        var viewModel = _productService.BuildDetails(id);
        if (viewModel is null)
        {
            _notificationService.Error("Product not found", "The requested product does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    public IActionResult Create() => View(_productService.BuildCreateForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = MergeForm(_productService.BuildCreateForm(), model);
            return View(model);
        }

        var result = _productService.Create(model);
        if (!result.Succeeded)
        {
            model = MergeForm(_productService.BuildCreateForm(), model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Product created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var viewModel = _productService.BuildEditForm(id);
        if (viewModel is null)
        {
            _notificationService.Error("Product not found", "The requested product does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = MergeForm(_productService.BuildEditForm(id) ?? _productService.BuildCreateForm(), model);
            return View(model);
        }

        var result = _productService.Update(id, model);
        if (!result.Succeeded)
        {
            model = MergeForm(_productService.BuildEditForm(id) ?? _productService.BuildCreateForm(), model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Product updated", result.Message);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Deactivate(string id)
    {
        var result = _productService.Deactivate(id);
        if (!result.Succeeded)
        {
            _notificationService.Error("Product status blocked", result.Message);
            return RedirectToAction(nameof(Index));
        }

        _notificationService.Success("Product status updated", result.Message);
        return RedirectToAction(nameof(Index));
    }

    private static ProductFormViewModel MergeForm(ProductFormViewModel target, ProductFormViewModel source)
    {
        target.CategoryId = source.CategoryId;
        target.ProductName = source.ProductName;
        target.Barcode = source.Barcode;
        target.BasePrice = source.BasePrice;
        target.Description = source.Description;
        return target;
    }
}
