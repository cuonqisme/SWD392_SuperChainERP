using System.ComponentModel.DataAnnotations;

namespace SupperChainErpDemo.Web.ViewModels.TransferNotes;

public class TransferNoteLineInputViewModel
{
    [Display(Name = "Product")]
    public string? ProductId { get; set; }

    [Display(Name = "Quantity")]
    public int? Quantity { get; set; }

    public int? AvailableQuantity { get; set; }
}
