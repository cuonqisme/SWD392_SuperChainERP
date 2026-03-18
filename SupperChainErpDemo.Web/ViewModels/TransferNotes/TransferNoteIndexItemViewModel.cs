using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.ViewModels.TransferNotes;

public class TransferNoteIndexItemViewModel
{
    public string TransferNoteId { get; init; } = string.Empty;

    public string TransferNo { get; init; } = string.Empty;

    public string SourceLocationName { get; init; } = string.Empty;

    public string DestinationLocationName { get; init; } = string.Empty;

    public string CreatedByName { get; init; } = string.Empty;

    public string ApprovedByName { get; init; } = "-";

    public DateTime CreatedDate { get; init; }

    public DateTime? ApprovedDate { get; init; }

    public DateTime? TransferOutDate { get; init; }

    public TransferNoteStatus Status { get; init; }

    public string StatusLabel { get; init; } = string.Empty;

    public int TotalItems { get; init; }

    public int TotalQuantity { get; init; }

    public bool CanEdit { get; init; }

    public bool CanApprove { get; init; }

    public bool CanReject { get; init; }

    public bool CanConfirmTransferOut { get; init; }
}
