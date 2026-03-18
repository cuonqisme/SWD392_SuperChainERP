using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.ViewModels.Products;

public class ProductDetailsViewModel
{
    public required Product Product { get; init; }

    public required string CategoryName { get; init; }
}
