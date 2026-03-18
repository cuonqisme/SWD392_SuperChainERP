using Microsoft.EntityFrameworkCore;
using SupperChainErpDemo.Web.Data;
using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public class TransferService : ITransferService
{
    private const string WarehouseStaffActorId = "USR-004";
    private const string ChainManagerActorId = "USR-005";

    private readonly AppDbContext _dbContext;

    public TransferService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyList<TransferNote> GetTransferNotes() =>
        _dbContext.TransferNotes.AsNoTracking()
            .OrderByDescending(note => note.CreatedDate)
            .ThenByDescending(note => note.TransferNo)
            .ToList();

    public IReadOnlyList<TransferNoteItem> GetAllTransferNoteItems() =>
        _dbContext.TransferNoteItems.AsNoTracking().ToList();

    public TransferNote? GetTransferNote(string id) =>
        _dbContext.TransferNotes.AsNoTracking()
            .FirstOrDefault(note => note.TransferNoteId == id);

    public IReadOnlyList<TransferNoteItem> GetTransferNoteItems(string transferNoteId) =>
        _dbContext.TransferNoteItems.AsNoTracking()
            .Where(item => item.TransferNoteId == transferNoteId)
            .OrderBy(item => item.ProId)
            .ToList();

    public IReadOnlyDictionary<string, string> GetActorLookup() =>
        _dbContext.Users.AsNoTracking()
            .ToDictionary(user => user.UserId, user => user.FullName, StringComparer.OrdinalIgnoreCase);

    public ServiceResult CreateTransferNote(
        string sourceLocId,
        string destinationLocId,
        string note,
        IReadOnlyCollection<TransferLineRequest> requestedLines,
        IReadOnlyCollection<string> warnings)
    {
        var transferNoteId = IdGenerator.NextId(
            _dbContext.TransferNotes.AsNoTracking().Select(item => item.TransferNoteId).ToList(),
            "TRN-");
        var transferNo = IdGenerator.NextId(
            _dbContext.TransferNotes.AsNoTracking().Select(item => item.TransferNo).ToList(),
            "TN-");

        var transferNote = new TransferNote
        {
            TransferNoteId = transferNoteId,
            TransferNo = transferNo,
            SourceLocId = sourceLocId,
            DestinationLocId = destinationLocId,
            CreatedBy = WarehouseStaffActorId,
            CreatedDate = DateTime.UtcNow,
            Status = TransferNoteStatus.Draft,
            Note = note.Trim()
        };

        _dbContext.TransferNotes.Add(transferNote);
        _dbContext.TransferNoteItems.AddRange(requestedLines.Select(line => new TransferNoteItem
        {
            TransferNoteId = transferNoteId,
            ProId = line.ProductId,
            Quantity = line.Quantity
        }));
        _dbContext.SaveChanges();

        var warningSuffix = warnings.Count == 0
            ? string.Empty
            : $" Stock warning recorded for {warnings.Count} line(s); review is recommended before approval.";
        return ServiceResult.Success($"Transfer note {transferNo} was created with Draft status.{warningSuffix}");
    }

    public ServiceResult UpdateTransferNote(
        string id,
        string sourceLocId,
        string destinationLocId,
        string note,
        IReadOnlyCollection<TransferLineRequest> requestedLines,
        IReadOnlyCollection<string> warnings)
    {
        var transferNote = GetTrackedTransferNote(id);
        if (transferNote is null)
        {
            return ServiceResult.Failure("The selected transfer note could not be found.");
        }

        if (transferNote.Status != TransferNoteStatus.Draft)
        {
            return ServiceResult.Failure("Only transfer notes in Draft status can be updated.");
        }

        transferNote.SourceLocId = sourceLocId;
        transferNote.DestinationLocId = destinationLocId;
        transferNote.Note = note.Trim();

        var currentItems = _dbContext.TransferNoteItems.Where(item => item.TransferNoteId == transferNote.TransferNoteId);
        _dbContext.TransferNoteItems.RemoveRange(currentItems);
        _dbContext.TransferNoteItems.AddRange(requestedLines.Select(line => new TransferNoteItem
        {
            TransferNoteId = transferNote.TransferNoteId,
            ProId = line.ProductId,
            Quantity = line.Quantity
        }));
        _dbContext.SaveChanges();

        var warningSuffix = warnings.Count == 0
            ? string.Empty
            : $" Stock warning recorded for {warnings.Count} line(s); review is recommended before approval.";
        return ServiceResult.Success($"Transfer note {transferNote.TransferNo} was updated successfully.{warningSuffix}");
    }

    public ServiceResult ReviewTransferNote(string id, bool approve, string? reviewNote = null)
    {
        var transferNote = GetTrackedTransferNote(id);
        if (transferNote is null)
        {
            return ServiceResult.Failure("The selected transfer note could not be found.");
        }

        if (transferNote.Status != TransferNoteStatus.Draft)
        {
            return ServiceResult.Failure("This transfer note is no longer in Draft status and cannot be reviewed.");
        }

        transferNote.Status = approve ? TransferNoteStatus.Approved : TransferNoteStatus.Rejected;
        transferNote.ApprovedBy = ChainManagerActorId;
        transferNote.ApprovedDate = DateTime.UtcNow;
        if (!approve && !string.IsNullOrWhiteSpace(reviewNote))
        {
            transferNote.Note = reviewNote.Trim();
        }

        _dbContext.SaveChanges();

        var decision = approve ? "approved" : "rejected";
        return ServiceResult.Success($"Transfer note {transferNote.TransferNo} was {decision} successfully.");
    }

    public ServiceResult ConfirmTransferOut(string id)
    {
        var transferNote = GetTrackedTransferNote(id);
        if (transferNote is null)
        {
            return ServiceResult.Failure("The selected transfer note could not be found.");
        }

        if (transferNote.Status != TransferNoteStatus.Approved)
        {
            return ServiceResult.Failure("Transfer-out can only be confirmed for transfer notes in Approved status.");
        }

        transferNote.Status = TransferNoteStatus.TransferOutConfirmed;
        transferNote.TransferOutDate = DateTime.UtcNow;
        _dbContext.SaveChanges();

        return ServiceResult.Success($"Transfer-out for {transferNote.TransferNo} was confirmed successfully.");
    }

    private TransferNote? GetTrackedTransferNote(string id) =>
        _dbContext.TransferNotes.FirstOrDefault(note => note.TransferNoteId == id);
}
