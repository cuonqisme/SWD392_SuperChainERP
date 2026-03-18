namespace SupperChainErpDemo.Web.Models;

public class TransferNoteItem
{
    public required string TransferNoteId { get; set; }

    public required string ProId { get; set; }

    public int Quantity { get; set; }
}
