using System.ComponentModel.DataAnnotations;

namespace SupperChainErpDemo.Web.ViewModels.Categories;

public class CategoryFormViewModel
{
    [Required]
    [Display(Name = "Category name")]
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "SKU prefix")]
    public string SkuPrefix { get; set; } = string.Empty;
}
