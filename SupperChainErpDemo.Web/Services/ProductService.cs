using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Products;

namespace SupperChainErpDemo.Web.Services;

public class ProductService : IProductService
{
    private readonly DemoDataStore _dataStore;

    public ProductService(DemoDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IReadOnlyList<Product> GetAll(string? statusFilter = null, string? categoryId = null)
    {
        var query = _dataStore.Products.AsEnumerable();

        if (Enum.TryParse<RecordStatus>(statusFilter, true, out var status))
        {
            query = query.Where(product => product.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            query = query.Where(product => product.CategoryId.Equals(categoryId, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderByDescending(product => product.UpdatedDate)
            .ThenBy(product => product.ProductName)
            .ToList();
    }

    public Product? GetById(string id) =>
        _dataStore.Products.FirstOrDefault(product => product.ProductId.Equals(id, StringComparison.OrdinalIgnoreCase));

    public ServiceResult Create(ProductFormViewModel model)
    {
        var category = Validate(model);
        if (category is null)
        {
            return ServiceResult.Failure("Please use a valid active category and a unique barcode before creating the product.");
        }

        var productId = _dataStore.NextProductId();
        var product = new Product
        {
            ProductId = productId,
            CategoryId = model.CategoryId,
            ProductName = model.ProductName.Trim(),
            Sku = CategoryService.BuildSku(category.SkuPrefix, productId),
            Barcode = model.Barcode.Trim(),
            BasePrice = model.BasePrice,
            Description = model.Description.Trim(),
            Status = RecordStatus.Active,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _dataStore.Products.Add(product);
        return ServiceResult.Success($"Product {product.ProductName} was created with SKU {product.Sku}.");
    }

    public ServiceResult Update(string id, ProductFormViewModel model)
    {
        var product = GetById(id);
        if (product is null)
        {
            return ServiceResult.Failure("The selected product could not be found.");
        }

        var category = Validate(model, id);
        if (category is null)
        {
            return ServiceResult.Failure("Please use a valid active category and a unique barcode before saving the product.");
        }

        product.CategoryId = model.CategoryId;
        product.ProductName = model.ProductName.Trim();
        product.Barcode = model.Barcode.Trim();
        product.BasePrice = model.BasePrice;
        product.Description = model.Description.Trim();
        product.Sku = CategoryService.BuildSku(category.SkuPrefix, product.ProductId);
        product.UpdatedDate = DateTime.UtcNow;

        return ServiceResult.Success($"Product {product.ProductName} was updated successfully.");
    }

    public ServiceResult ChangeStatus(string id, RecordStatus status)
    {
        var product = GetById(id);
        if (product is null)
        {
            return ServiceResult.Failure("The selected product could not be found.");
        }

        product.Status = status;
        product.UpdatedDate = DateTime.UtcNow;
        return ServiceResult.Success($"Product {product.ProductName} status changed to {status}.");
    }

    private Category? Validate(ProductFormViewModel model, string? currentId = null)
    {
        if (string.IsNullOrWhiteSpace(model.ProductName) || string.IsNullOrWhiteSpace(model.Barcode))
        {
            return null;
        }

        var category = _dataStore.Categories.FirstOrDefault(item => item.CategoryId == model.CategoryId);
        if (category?.Status != RecordStatus.Active)
        {
            return null;
        }

        var duplicateBarcode = _dataStore.Products.Any(product =>
            !product.ProductId.Equals(currentId, StringComparison.OrdinalIgnoreCase) &&
            product.Barcode.Equals(model.Barcode.Trim(), StringComparison.OrdinalIgnoreCase));

        return duplicateBarcode ? null : category;
    }
}
