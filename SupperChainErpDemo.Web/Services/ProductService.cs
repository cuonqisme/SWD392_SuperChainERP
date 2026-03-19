using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.Products;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupperChainErpDemo.Web.Data;

namespace SupperChainErpDemo.Web.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _dbContext;

    public ProductService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ProductIndexViewModel GetProductList(string? statusFilter = null, string? categoryId = null)
    {
        var query = _dbContext.Products.AsNoTracking().AsEnumerable();

        if (Enum.TryParse<RecordStatus>(statusFilter, true, out var status))
        {
            query = query.Where(product => product.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            query = query.Where(product => product.CategoryId.Equals(categoryId, StringComparison.OrdinalIgnoreCase));
        }

        return new ProductIndexViewModel
        {
            StatusFilter = statusFilter,
            CategoryFilter = categoryId,
            Products = query
                .OrderByDescending(product => product.UpdatedDate)
                .ThenBy(product => product.ProductName)
                .ToList(),
            CategoryNames = _dbContext.Categories.AsNoTracking().ToDictionary(category => category.CategoryId, category => category.CategoryName)
        };
    }

    public ProductDetailsViewModel? GetProductDetails(string id)
    {
        var product = GetById(id);
        if (product is null)
        {
            return null;
        }

        return new ProductDetailsViewModel
        {
            Product = product,
            CategoryName = _dbContext.Categories.AsNoTracking().FirstOrDefault(category => category.CategoryId == product.CategoryId)?.CategoryName ?? "Unknown"
        };
    }

    public ProductFormViewModel PrepareCreateProduct() => PopulateCategoryOptions(new ProductFormViewModel());

    public ProductFormViewModel? PrepareUpdateProduct(string id)
    {
        var product = GetById(id);
        if (product is null)
        {
            return null;
        }

        return PopulateCategoryOptions(new ProductFormViewModel
        {
            CategoryId = product.CategoryId,
            ProductName = product.ProductName,
            Barcode = product.Barcode,
            BasePrice = product.BasePrice,
            Description = product.Description
        });
    }

    public IReadOnlyList<SelectListItem> GetProductOptions(IEnumerable<string>? includeProductIds = null)
    {
        var selectedIds = includeProductIds?
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        return _dbContext.Products.AsNoTracking()
            .Where(product => product.Status == RecordStatus.Active || selectedIds.Contains(product.ProductId))
            .OrderBy(product => product.ProductName)
            .Select(product => new SelectListItem(
                $"{product.ProductName} ({product.Sku})",
                product.ProductId))
            .ToList();
    }

    public IReadOnlyDictionary<string, Product> GetProductCatalog(IEnumerable<string> productIds)
    {
        var ids = productIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (ids.Count == 0)
        {
            return new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);
        }

        return _dbContext.Products.AsNoTracking()
            .Where(product => ids.Contains(product.ProductId))
            .ToDictionary(product => product.ProductId, StringComparer.OrdinalIgnoreCase);
    }

    private Product? GetById(string id) =>
        _dbContext.Products.FirstOrDefault(product => product.ProductId == id);

    public ServiceResult CreateProduct(ProductFormViewModel model)
    {
        PopulateCategoryOptions(model);
        var category = Validate(model);
        if (category is null)
        {
            return ServiceResult.Failure("Please use a valid active category and a unique barcode before creating the product.");
        }

        var productId = IdGenerator.NextId(_dbContext.Products.AsNoTracking().Select(item => item.ProductId).ToList(), "PRO-");
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

        _dbContext.Products.Add(product);
        _dbContext.SaveChanges();
        return ServiceResult.Success($"Product {product.ProductName} was created with SKU {product.Sku}.");
    }

    public ServiceResult UpdateProduct(string id, ProductFormViewModel model)
    {
        PopulateCategoryOptions(model);
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

        _dbContext.SaveChanges();
        return ServiceResult.Success($"Product {product.ProductName} was updated successfully.");
    }

    public ServiceResult UpdateProductStatus(string id, RecordStatus status)
    {
        var product = GetById(id);
        if (product is null)
        {
            return ServiceResult.Failure("The selected product could not be found.");
        }

        if (status == RecordStatus.Active)
        {
            var category = _dbContext.Categories.FirstOrDefault(item => item.CategoryId == product.CategoryId);
            if (category?.Status != RecordStatus.Active)
            {
                return ServiceResult.Failure("Products can only be activated when their category is active.");
            }
        }

        product.Status = status;
        product.UpdatedDate = DateTime.UtcNow;
        _dbContext.SaveChanges();
        return ServiceResult.Success($"Product {product.ProductName} status changed to {status}.");
    }

    private Category? Validate(ProductFormViewModel model, string? currentId = null)
    {
        if (string.IsNullOrWhiteSpace(model.ProductName) || string.IsNullOrWhiteSpace(model.Barcode))
        {
            return null;
        }

        var category = _dbContext.Categories.FirstOrDefault(item => item.CategoryId == model.CategoryId);
        if (category?.Status != RecordStatus.Active)
        {
            return null;
        }

        var normalizedBarcode = model.Barcode.Trim().ToUpperInvariant();

        var duplicateBarcode = _dbContext.Products.Any(product =>
            product.ProductId != currentId &&
            product.Barcode.ToUpper() == normalizedBarcode);

        return duplicateBarcode ? null : category;
    }

    private ProductFormViewModel PopulateCategoryOptions(ProductFormViewModel model)
    {
        model.CategoryOptions = _dbContext.Categories.AsNoTracking()
            .Where(category => category.Status == RecordStatus.Active || category.CategoryId == model.CategoryId)
            .OrderBy(category => category.CategoryName)
            .Select(category => new SelectListItem(category.CategoryName, category.CategoryId))
            .ToList();
        return model;
    }
}
