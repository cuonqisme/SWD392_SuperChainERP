using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Products;

namespace SupperChainErpDemo.Web.Services;

public interface IProductService
{
    ProductIndexViewModel BuildIndex(string? statusFilter = null, string? categoryId = null);

    ProductDetailsViewModel? BuildDetails(string id);

    ProductFormViewModel BuildCreateForm();

    ProductFormViewModel? BuildEditForm(string id);

    ServiceResult Create(ProductFormViewModel model);

    ServiceResult Update(string id, ProductFormViewModel model);

    ServiceResult Deactivate(string id);
}
