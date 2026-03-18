using System.Collections.Concurrent;
using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public class AuditLogService : IAuditLogService
{
    private static readonly ConcurrentQueue<string> Entries = new();
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(ILogger<AuditLogService> logger)
    {
        _logger = logger;
    }

    public void LogTransferOut(TransferNote note, IReadOnlyCollection<TransferNoteItem> items)
    {
        var entry = $"Transfer-out confirmed for {note.TransferNo} from {note.SourceLocId} to {note.DestinationLocId} with {items.Count} line(s) at {DateTime.UtcNow:O}.";
        Entries.Enqueue(entry);
        _logger.LogInformation(entry);
    }
}
