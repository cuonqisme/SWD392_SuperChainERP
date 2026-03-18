using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SupperChainErpDemo.Web.ViewModels.Products;

public class ProductFormViewModel
{
    [Required]
    [Display(Name = "Category")]
    public string CategoryId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Product name")]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Barcode")]
    public string Barcode { get; set; } = string.Empty;

    [Range(0, 999999999)]
    [Display(Name = "Base price")]
    public decimal BasePrice { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> CategoryOptions { get; set; } = [];
}
