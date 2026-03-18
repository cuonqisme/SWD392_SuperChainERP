using SupperChainErpDemo.Web.ViewModels.Categories;

namespace SupperChainErpDemo.Web.Services;

public interface ICategoryManagementCoordinator
{
    CategoryIndexViewModel ShowCategoryList(string? statusFilter = null);

    CategoryDetailsViewModel? ShowCategoryDetails(string id);

    CategoryFormViewModel PrepareCreateCategory();

    CategoryFormViewModel? PrepareUpdateCategory(string id);

    ServiceResult CreateCategory(CategoryFormViewModel model);

    ServiceResult UpdateCategory(string id, CategoryFormViewModel model);

    ServiceResult DeactivateCategory(string id);
}
