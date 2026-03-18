using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Categories;

namespace SupperChainErpDemo.Web.Services;

public interface ICategoryService
{
    IReadOnlyList<Category> GetAll(string? statusFilter = null);

    Category? GetById(string id);

    ServiceResult Create(CategoryFormViewModel model);

    ServiceResult Update(string id, CategoryFormViewModel model);

    ServiceResult ChangeStatus(string id, RecordStatus status);
}
