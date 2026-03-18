using SupperChainErpDemo.Web.ViewModels.TransferNotes;

namespace SupperChainErpDemo.Web.Services;

public interface ITransferCoordinator
{
    TransferNoteIndexViewModel ShowTransferNoteList(
        string? searchTerm = null,
        string? statusFilter = null,
        string? sourceLocId = null,
        string? destinationLocId = null,
        DateTime? createdFrom = null,
        DateTime? createdTo = null);

    TransferNoteDetailsViewModel? ShowTransferNoteDetails(string id);

    TransferNoteFormViewModel InitializeTransferDraft();

    TransferNoteFormViewModel HydrateTransferDraft(TransferNoteFormViewModel model);

    TransferNoteFormViewModel? PrepareTransferUpdate(string id);

    TransferNoteFormViewModel HydrateTransferUpdate(string id, TransferNoteFormViewModel model);

    ServiceResult CreateTransferNote(TransferNoteFormViewModel model);

    ServiceResult UpdateTransferNote(string id, TransferNoteFormViewModel model);

    ServiceResult ApproveTransferNote(string id);

    ServiceResult RejectTransferNote(string id, string? reason = null);

    ServiceResult ConfirmTransferOut(string id);
}
