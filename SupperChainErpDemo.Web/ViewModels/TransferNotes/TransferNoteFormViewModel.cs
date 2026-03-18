using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SupperChainErpDemo.Web.ViewModels.TransferNotes;

public class TransferNoteFormViewModel
{
    [Required]
    [Display(Name = "Source location")]
    public string SourceLocId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Destination location")]
    public string DestinationLocId { get; set; } = string.Empty;

    [Display(Name = "Transfer note")]
    [StringLength(255)]
    public string Note { get; set; } = string.Empty;

    public List<TransferNoteLineInputViewModel> Items { get; set; } = [];

    public IReadOnlyList<SelectListItem> LocationOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> ProductOptions { get; set; } = [];

    public IReadOnlyList<string> InventoryWarnings { get; set; } = [];
}
