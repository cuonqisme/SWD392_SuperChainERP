using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.ViewModels.Products;

public class ProductIndexViewModel
{
    public string? StatusFilter { get; set; }

    public string? CategoryFilter { get; set; }

    public IReadOnlyList<Product> Products { get; init; } = [];

    public IReadOnlyDictionary<string, string> CategoryNames { get; init; } = new Dictionary<string, string>();
}
