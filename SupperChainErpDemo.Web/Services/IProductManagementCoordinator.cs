using SupperChainErpDemo.Web.ViewModels.Products;

namespace SupperChainErpDemo.Web.Services;

public interface IProductManagementCoordinator
{
    ProductIndexViewModel ShowProductList(string? statusFilter = null, string? categoryId = null);

    ProductDetailsViewModel? ShowProductDetails(string id);

    ProductFormViewModel PrepareCreateProduct();

    ProductFormViewModel? PrepareUpdateProduct(string id);

    ServiceResult CreateProduct(ProductFormViewModel model);

    ServiceResult UpdateProduct(string id, ProductFormViewModel model);

    ServiceResult DeactivateProduct(string id);
}
