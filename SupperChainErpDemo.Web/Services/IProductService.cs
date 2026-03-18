using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Products;

namespace SupperChainErpDemo.Web.Services;

public interface IProductService
{
    IReadOnlyList<Product> GetAll(string? statusFilter = null, string? categoryId = null);

    Product? GetById(string id);

    ServiceResult Create(ProductFormViewModel model);

    ServiceResult Update(string id, ProductFormViewModel model);

    ServiceResult ChangeStatus(string id, RecordStatus status);
}
