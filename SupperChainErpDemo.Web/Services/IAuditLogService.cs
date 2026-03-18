using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public interface IAuditLogService
{
    void LogTransferOut(TransferNote note, IReadOnlyCollection<TransferNoteItem> items);
}
