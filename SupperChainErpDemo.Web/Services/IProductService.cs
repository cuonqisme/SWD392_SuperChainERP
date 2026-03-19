using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Products;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SupperChainErpDemo.Web.Services;

public interface IProductService
{
    ProductIndexViewModel GetProductList(string? statusFilter = null, string? categoryId = null);

    ProductDetailsViewModel? GetProductDetails(string id);

    ProductFormViewModel PrepareCreateProduct();

    ProductFormViewModel? PrepareUpdateProduct(string id);

    IReadOnlyList<SelectListItem> GetProductOptions(IEnumerable<string>? includeProductIds = null);

    IReadOnlyDictionary<string, Product> GetProductCatalog(IEnumerable<string> productIds);

    ServiceResult CreateProduct(ProductFormViewModel model);

    ServiceResult UpdateProduct(string id, ProductFormViewModel model);

    ServiceResult UpdateProductStatus(string id, RecordStatus status);
}
