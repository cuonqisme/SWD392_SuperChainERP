using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.ViewModels.Categories;

public class CategoryIndexViewModel
{
    public string? StatusFilter { get; set; }

    public IReadOnlyList<Category> Categories { get; init; } = [];

    public IReadOnlyDictionary<string, int> ProductCountByCategory { get; init; } = new Dictionary<string, int>();
}
