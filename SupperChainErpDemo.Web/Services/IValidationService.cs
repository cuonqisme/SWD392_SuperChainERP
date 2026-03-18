using SupperChainErpDemo.Web.ViewModels.TransferNotes;

namespace SupperChainErpDemo.Web.Services;

public interface IValidationService
{
    TransferDraftValidationResult ValidateTransferDraft(TransferNoteFormViewModel model);
}
