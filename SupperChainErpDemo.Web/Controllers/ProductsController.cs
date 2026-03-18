using Microsoft.AspNetCore.Mvc;
using SupperChainErpDemo.Web.Services;
using SupperChainErpDemo.Web.ViewModels.Products;

namespace SupperChainErpDemo.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductManagementCoordinator _productManagementCoordinator;
    private readonly INotificationService _notificationService;

    public ProductsController(
        IProductManagementCoordinator productManagementCoordinator,
        INotificationService notificationService)
    {
        _productManagementCoordinator = productManagementCoordinator;
        _notificationService = notificationService;
    }

    public IActionResult Index(string? statusFilter, string? categoryFilter)
    {
        return View(_productManagementCoordinator.ShowProductList(statusFilter, categoryFilter));
    }

    public IActionResult Details(string id)
    {
        var viewModel = _productManagementCoordinator.ShowProductDetails(id);
        if (viewModel is null)
        {
            _notificationService.Error("Product not found", "The requested product does not exist anymore.");
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    public IActionResult Create() => View(_productManagementCoordinator.PrepareCreateProduct());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = MergeForm(_productManagementCoordinator.PrepareCreateProduct(), model);
            return View(model);
        }

        var result = _productManagementCoordinator.CreateProduct(model);
        if (!result.Succeeded)
        {
            model = MergeForm(_productManagementCoordinator.PrepareCreateProduct(), model);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        _notificationService.Success("Product created", result.Message);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var viewModel = _productManagementCoordinator.PrepareUpdateProduct(id);
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
            model = MergeForm(_productManagementCoordinator.PrepareUpdateProduct(id) ?? _productManagementCoordinator.PrepareCreateProduct(), model);
            return View(model);
        }

        var result = _productManagementCoordinator.UpdateProduct(id, model);
        if (!result.Succeeded)
        {
            model = MergeForm(_productManagementCoordinator.PrepareUpdateProduct(id) ?? _productManagementCoordinator.PrepareCreateProduct(), model);
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
        var result = _productManagementCoordinator.DeactivateProduct(id);
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
