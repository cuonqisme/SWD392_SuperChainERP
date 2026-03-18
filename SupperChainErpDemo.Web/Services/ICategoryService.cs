using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Categories;

namespace SupperChainErpDemo.Web.Services;

public interface ICategoryService
{
    CategoryIndexViewModel BuildIndex(string? statusFilter = null);

    CategoryDetailsViewModel? BuildDetails(string id);

    CategoryFormViewModel BuildCreateForm();

    CategoryFormViewModel? BuildEditForm(string id);

    ServiceResult Create(CategoryFormViewModel model);

    ServiceResult Update(string id, CategoryFormViewModel model);

    ServiceResult Deactivate(string id);
}
