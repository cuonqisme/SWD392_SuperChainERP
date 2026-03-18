using SupperChainErpDemo.Web.ViewModels.Categories;

namespace SupperChainErpDemo.Web.Services;

public class CategoryManagementCoordinator : ICategoryManagementCoordinator
{
    private readonly ICategoryService _categoryService;

    public CategoryManagementCoordinator(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public CategoryIndexViewModel ShowCategoryList(string? statusFilter = null) =>
        _categoryService.BuildIndex(statusFilter);

    public CategoryDetailsViewModel? ShowCategoryDetails(string id) =>
        _categoryService.BuildDetails(id);

    public CategoryFormViewModel PrepareCreateCategory() =>
        _categoryService.BuildCreateForm();

    public CategoryFormViewModel? PrepareUpdateCategory(string id) =>
        _categoryService.BuildEditForm(id);

    public ServiceResult CreateCategory(CategoryFormViewModel model) =>
        _categoryService.Create(model);

    public ServiceResult UpdateCategory(string id, CategoryFormViewModel model) =>
        _categoryService.Update(id, model);

    public ServiceResult DeactivateCategory(string id) =>
        _categoryService.Deactivate(id);
}
