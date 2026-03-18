using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public interface IFilterService
{
    IReadOnlyList<TransferNote> ApplyTransferNoteFilters(
        IEnumerable<TransferNote> notes,
        TransferNoteFilterCriteria criteria);
}
