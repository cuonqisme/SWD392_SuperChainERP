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
        _productService.BuildIndex(statusFilter, categoryId);

    public ProductDetailsViewModel? ShowProductDetails(string id) =>
        _productService.BuildDetails(id);

    public ProductFormViewModel PrepareCreateProduct() =>
        _productService.BuildCreateForm();

    public ProductFormViewModel? PrepareUpdateProduct(string id) =>
        _productService.BuildEditForm(id);

    public ServiceResult CreateProduct(ProductFormViewModel model) =>
        _productService.Create(model);

    public ServiceResult UpdateProduct(string id, ProductFormViewModel model) =>
        _productService.Update(id, model);

    public ServiceResult DeactivateProduct(string id) =>
        _productService.Deactivate(id);
}
