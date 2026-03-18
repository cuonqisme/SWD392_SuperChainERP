using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.ViewModels.TransferNotes;

public class TransferNoteDetailsViewModel
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

    public DateTime? TransferInDate { get; init; }

    public TransferNoteStatus Status { get; init; }

    public string StatusLabel { get; init; } = string.Empty;

    public string Note { get; init; } = string.Empty;

    public IReadOnlyList<TransferNoteLineDisplayViewModel> Items { get; init; } = [];

    public bool CanEdit { get; init; }

    public bool CanApprove { get; init; }

    public bool CanReject { get; init; }

    public bool CanConfirmTransferOut { get; init; }

    public IReadOnlyList<TransferTimelineEntryViewModel> Timeline { get; init; } = [];
}
