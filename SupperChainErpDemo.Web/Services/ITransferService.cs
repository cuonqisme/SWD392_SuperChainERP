using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public interface ITransferService
{
    IReadOnlyList<TransferNote> GetTransferNotes();

    IReadOnlyList<TransferNoteItem> GetAllTransferNoteItems();

    TransferNote? GetTransferNote(string id);

    IReadOnlyList<TransferNoteItem> GetTransferNoteItems(string transferNoteId);

    IReadOnlyDictionary<string, string> GetActorLookup();

    ServiceResult CreateTransferNote(
        string sourceLocId,
        string destinationLocId,
        string note,
        IReadOnlyCollection<TransferLineRequest> requestedLines,
        IReadOnlyCollection<string> warnings);

    ServiceResult UpdateTransferNote(
        string id,
        string sourceLocId,
        string destinationLocId,
        string note,
        IReadOnlyCollection<TransferLineRequest> requestedLines,
        IReadOnlyCollection<string> warnings);

    ServiceResult ReviewTransferNote(string id, bool approve, string? reviewNote = null);

    ServiceResult ConfirmTransferOut(string id);
}
