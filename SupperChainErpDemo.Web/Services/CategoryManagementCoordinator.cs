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
        _categoryService.GetCategoryList(statusFilter);

    public CategoryDetailsViewModel? ShowCategoryDetails(string id) =>
        _categoryService.GetCategoryDetails(id);

    public CategoryFormViewModel PrepareCreateCategory() =>
        _categoryService.PrepareCreateCategory();

    public CategoryFormViewModel? PrepareUpdateCategory(string id) =>
        _categoryService.PrepareUpdateCategory(id);

    public ServiceResult CreateCategory(CategoryFormViewModel model) =>
        _categoryService.CreateCategory(model);

    public ServiceResult UpdateCategory(string id, CategoryFormViewModel model) =>
        _categoryService.UpdateCategory(id, model);

    public ServiceResult UpdateCategoryStatus(string id, Models.RecordStatus status) =>
        _categoryService.UpdateCategoryStatus(id, status);
}
