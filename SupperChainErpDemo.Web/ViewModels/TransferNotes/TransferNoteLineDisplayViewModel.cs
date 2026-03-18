namespace SupperChainErpDemo.Web.ViewModels.TransferNotes;

public class TransferNoteLineDisplayViewModel
{
    public string ProductId { get; init; } = string.Empty;

    public string ProductName { get; init; } = string.Empty;

    public string Sku { get; init; } = string.Empty;

    public int Quantity { get; init; }

    public int SourceAvailableQuantity { get; init; }
}
