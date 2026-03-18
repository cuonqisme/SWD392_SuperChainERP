namespace SupperChainErpDemo.Web.Models;

public class TransferNote
{
    public required string TransferNoteId { get; set; }

    public required string TransferNo { get; set; }

    public required string SourceLocId { get; set; }

    public required string DestinationLocId { get; set; }

    public required string CreatedBy { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ApprovedDate { get; set; }

    public DateTime? TransferOutDate { get; set; }

    public DateTime? TransferInDate { get; set; }

    public TransferNoteStatus Status { get; set; } = TransferNoteStatus.Draft;

    public string Note { get; set; } = string.Empty;
}
