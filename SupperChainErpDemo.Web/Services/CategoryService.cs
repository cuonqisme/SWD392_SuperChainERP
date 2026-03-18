using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Categories;

namespace SupperChainErpDemo.Web.Services;

public class CategoryService : ICategoryService
{
    private readonly DemoDataStore _dataStore;

    public CategoryService(DemoDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public CategoryIndexViewModel BuildIndex(string? statusFilter = null)
    {
        var query = _dataStore.Categories.AsEnumerable();

        if (Enum.TryParse<RecordStatus>(statusFilter, true, out var status))
        {
            query = query.Where(category => category.Status == status);
        }

        return new CategoryIndexViewModel
        {
            StatusFilter = statusFilter,
            Categories = query.OrderBy(category => category.CategoryName).ToList(),
            ProductCountByCategory = _dataStore.Products
                .GroupBy(product => product.CategoryId)
                .ToDictionary(group => group.Key, group => group.Count())
        };
    }

    public CategoryDetailsViewModel? BuildDetails(string id)
    {
        var category = GetById(id);
        if (category is null)
        {
            return null;
        }

        return new CategoryDetailsViewModel
        {
            Category = category,
            Products = _dataStore.Products
                .Where(product => product.CategoryId == category.CategoryId)
                .OrderBy(product => product.ProductName)
                .ToList()
        };
    }

    public CategoryFormViewModel BuildCreateForm() => new();

    public CategoryFormViewModel? BuildEditForm(string id)
    {
        var category = GetById(id);
        if (category is null)
        {
            return null;
        }

        return new CategoryFormViewModel
        {
            CategoryName = category.CategoryName,
            Description = category.Description,
            SkuPrefix = category.SkuPrefix
        };
    }

    private Category? GetById(string id) =>
        _dataStore.Categories.FirstOrDefault(category => category.CategoryId.Equals(id, StringComparison.OrdinalIgnoreCase));

    public ServiceResult Create(CategoryFormViewModel model)
    {
        var validationError = Validate(model);
        if (validationError is not null)
        {
            return ServiceResult.Failure(validationError);
        }

        var category = new Category
        {
            CategoryId = _dataStore.NextCategoryId(),
            CategoryName = model.CategoryName.Trim(),
            Description = model.Description.Trim(),
            SkuPrefix = model.SkuPrefix.Trim().ToUpperInvariant(),
            Status = RecordStatus.Active,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _dataStore.Categories.Add(category);
        return ServiceResult.Success($"Category {category.CategoryName} was created successfully.");
    }

    public ServiceResult Update(string id, CategoryFormViewModel model)
    {
        var category = GetById(id);
        if (category is null)
        {
            return ServiceResult.Failure("The selected category could not be found.");
        }

        var validationError = Validate(model, id);
        if (validationError is not null)
        {
            return ServiceResult.Failure(validationError);
        }

        category.CategoryName = model.CategoryName.Trim();
        category.Description = model.Description.Trim();
        category.SkuPrefix = model.SkuPrefix.Trim().ToUpperInvariant();
        category.UpdatedDate = DateTime.UtcNow;

        foreach (var product in _dataStore.Products.Where(item => item.CategoryId == category.CategoryId))
        {
            product.Sku = BuildSku(category.SkuPrefix, product.ProductId);
            product.UpdatedDate = DateTime.UtcNow;
        }

        return ServiceResult.Success($"Category {category.CategoryName} was updated successfully.");
    }

    public ServiceResult Deactivate(string id)
    {
        var category = GetById(id);
        if (category is null)
        {
            return ServiceResult.Failure("The selected category could not be found.");
        }

        if (_dataStore.Products.Any(product => product.CategoryId == category.CategoryId && product.Status == RecordStatus.Active))
        {
            return ServiceResult.Failure("Deactivate or move active products before deactivating this category.");
        }

        category.Status = RecordStatus.Inactive;
        category.UpdatedDate = DateTime.UtcNow;
        return ServiceResult.Success($"Category {category.CategoryName} was deactivated successfully.");
    }

    public static string BuildSku(string prefix, string productId) => $"{prefix}-{productId}";

    private string? Validate(CategoryFormViewModel model, string? currentId = null)
    {
        if (string.IsNullOrWhiteSpace(model.CategoryName))
        {
            return "Category name is required.";
        }

        if (string.IsNullOrWhiteSpace(model.SkuPrefix))
        {
            return "SKU prefix is required.";
        }

        var normalizedName = model.CategoryName.Trim();
        var normalizedPrefix = model.SkuPrefix.Trim().ToUpperInvariant();

        var duplicateName = _dataStore.Categories.Any(category =>
            !category.CategoryId.Equals(currentId, StringComparison.OrdinalIgnoreCase) &&
            category.CategoryName.Equals(normalizedName, StringComparison.OrdinalIgnoreCase));

        if (duplicateName)
        {
            return "Category name must be unique.";
        }

        var duplicatePrefix = _dataStore.Categories.Any(category =>
            !category.CategoryId.Equals(currentId, StringComparison.OrdinalIgnoreCase) &&
            category.SkuPrefix.Equals(normalizedPrefix, StringComparison.OrdinalIgnoreCase));

        return duplicatePrefix ? "SKU prefix must be unique." : null;
    }
}
