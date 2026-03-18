using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.ViewModels.Categories;

public class CategoryDetailsViewModel
{
    public required Category Category { get; init; }

    public IReadOnlyList<Product> Products { get; init; } = [];
}
