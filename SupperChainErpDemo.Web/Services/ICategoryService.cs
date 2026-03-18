using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Categories;

namespace SupperChainErpDemo.Web.Services;

public interface ICategoryService
{
    CategoryIndexViewModel GetCategoryList(string? statusFilter = null);

    CategoryDetailsViewModel? GetCategoryDetails(string id);

    CategoryFormViewModel PrepareCreateCategory();

    CategoryFormViewModel? PrepareUpdateCategory(string id);

    ServiceResult CreateCategory(CategoryFormViewModel model);

    ServiceResult UpdateCategory(string id, CategoryFormViewModel model);

    ServiceResult DeactivateCategory(string id);
}
