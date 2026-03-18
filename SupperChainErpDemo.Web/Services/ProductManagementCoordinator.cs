using SupperChainErpDemo.Web.ViewModels.Products;

namespace SupperChainErpDemo.Web.Services;

public class ProductManagementCoordinator : IProductManagementCoordinator
{
    private readonly IProductService _productService;

    public ProductManagementCoordinator(IProductService productService)
    {
        _productService = productService;
    }

    public ProductIndexViewModel ShowProductList(string? statusFilter = null, string? categoryId = null) =>
        _productService.GetProductList(statusFilter, categoryId);

    public ProductDetailsViewModel? ShowProductDetails(string id) =>
        _productService.GetProductDetails(id);

    public ProductFormViewModel PrepareCreateProduct() =>
        _productService.PrepareCreateProduct();

    public ProductFormViewModel? PrepareUpdateProduct(string id) =>
        _productService.PrepareUpdateProduct(id);

    public ServiceResult CreateProduct(ProductFormViewModel model) =>
        _productService.CreateProduct(model);

    public ServiceResult UpdateProduct(string id, ProductFormViewModel model) =>
        _productService.UpdateProduct(id, model);

    public ServiceResult DeactivateProduct(string id) =>
        _productService.DeactivateProduct(id);
}
