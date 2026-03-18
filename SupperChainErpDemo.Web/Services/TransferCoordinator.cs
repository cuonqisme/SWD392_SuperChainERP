using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.TransferNotes;

namespace SupperChainErpDemo.Web.Services;

public class TransferCoordinator : ITransferCoordinator
{
    private readonly ITransferService _transferService;
    private readonly ILocationService _locationService;
    private readonly IProductService _productService;
    private readonly IInventoryQueryService _inventoryQueryService;
    private readonly IInventoryService _inventoryService;
    private readonly IValidationService _validationService;
    private readonly IFilterService _filterService;
    private readonly IAuditLogService _auditLogService;

    public TransferCoordinator(
        ITransferService transferService,
        ILocationService locationService,
        IProductService productService,
        IInventoryQueryService inventoryQueryService,
        IInventoryService inventoryService,
        IValidationService validationService,
        IFilterService filterService,
        IAuditLogService auditLogService)
    {
        _transferService = transferService;
        _locationService = locationService;
        _productService = productService;
        _inventoryQueryService = inventoryQueryService;
        _inventoryService = inventoryService;
        _validationService = validationService;
        _filterService = filterService;
        _auditLogService = auditLogService;
    }

    public TransferNoteIndexViewModel ShowTransferNoteList(
        string? searchTerm = null,
        string? statusFilter = null,
        string? sourceLocId = null,
        string? destinationLocId = null,
        DateTime? createdFrom = null,
        DateTime? createdTo = null)
    {
        var locationLookup = _locationService.GetLocationLookup();
        var actorLookup = _transferService.GetActorLookup();
        var itemGroups = _transferService.GetAllTransferNoteItems()
            .GroupBy(item => item.TransferNoteId)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.OrdinalIgnoreCase);

        var criteria = new TransferNoteFilterCriteria
        {
            SearchTerm = searchTerm,
            StatusFilter = statusFilter,
            SourceLocId = sourceLocId,
            DestinationLocId = destinationLocId,
            CreatedFrom = createdFrom,
            CreatedTo = createdTo
        };
        var notes = _filterService.ApplyTransferNoteFilters(_transferService.GetTransferNotes(), criteria);

        return new TransferNoteIndexViewModel
        {
            SearchTerm = searchTerm,
            StatusFilter = statusFilter,
            SourceFilter = sourceLocId,
            DestinationFilter = destinationLocId,
            CreatedFrom = createdFrom,
            CreatedTo = createdTo,
            LocationOptions = _locationService.GetLocationOptions(),
            EmptyStateMessage = notes.Count == 0
                ? "No transfer notes matched the selected search and filter criteria."
                : null,
            TransferNotes = notes.Select(note =>
            {
                var noteItems = itemGroups.GetValueOrDefault(note.TransferNoteId, []);
                return new TransferNoteIndexItemViewModel
                {
                    TransferNoteId = note.TransferNoteId,
                    TransferNo = note.TransferNo,
                    SourceLocationName = locationLookup.GetValueOrDefault(note.SourceLocId)?.LocationName ?? note.SourceLocId,
                    DestinationLocationName = locationLookup.GetValueOrDefault(note.DestinationLocId)?.LocationName ?? note.DestinationLocId,
                    CreatedByName = actorLookup.GetValueOrDefault(note.CreatedBy, note.CreatedBy),
                    ApprovedByName = string.IsNullOrWhiteSpace(note.ApprovedBy)
                        ? "-"
                        : actorLookup.GetValueOrDefault(note.ApprovedBy, note.ApprovedBy),
                    CreatedDate = note.CreatedDate,
                    ApprovedDate = note.ApprovedDate,
                    TransferOutDate = note.TransferOutDate,
                    Status = note.Status,
                    StatusLabel = FormatStatus(note.Status),
                    TotalItems = noteItems.Count,
                    TotalQuantity = noteItems.Sum(item => item.Quantity),
                    CanEdit = note.Status == TransferNoteStatus.Draft,
                    CanApprove = note.Status == TransferNoteStatus.Draft,
                    CanReject = note.Status == TransferNoteStatus.Draft,
                    CanConfirmTransferOut = note.Status == TransferNoteStatus.Approved
                };
            }).ToList()
        };
    }

    public TransferNoteDetailsViewModel? ShowTransferNoteDetails(string id)
    {
        var note = _transferService.GetTransferNote(id);
        if (note is null)
        {
            return null;
        }

        var items = _transferService.GetTransferNoteItems(id);
        var productCatalog = _productService.GetProductCatalog(items.Select(item => item.ProId));
        var actorLookup = _transferService.GetActorLookup();
        var locationLookup = _locationService.GetLocationLookup();
        var snapshot = _inventoryQueryService.GetSnapshot(note.SourceLocId, items.Select(item => item.ProId));

        return new TransferNoteDetailsViewModel
        {
            TransferNoteId = note.TransferNoteId,
            TransferNo = note.TransferNo,
            SourceLocationName = locationLookup.GetValueOrDefault(note.SourceLocId)?.LocationName ?? note.SourceLocId,
            DestinationLocationName = locationLookup.GetValueOrDefault(note.DestinationLocId)?.LocationName ?? note.DestinationLocId,
            CreatedByName = actorLookup.GetValueOrDefault(note.CreatedBy, note.CreatedBy),
            ApprovedByName = string.IsNullOrWhiteSpace(note.ApprovedBy)
                ? "-"
                : actorLookup.GetValueOrDefault(note.ApprovedBy, note.ApprovedBy),
            CreatedDate = note.CreatedDate,
            ApprovedDate = note.ApprovedDate,
            TransferOutDate = note.TransferOutDate,
            TransferInDate = note.TransferInDate,
            Status = note.Status,
            StatusLabel = FormatStatus(note.Status),
            Note = note.Note,
            Items = items.Select(item =>
            {
                var product = productCatalog.GetValueOrDefault(item.ProId);
                return new TransferNoteLineDisplayViewModel
                {
                    ProductId = item.ProId,
                    ProductName = product?.ProductName ?? item.ProId,
                    Sku = product?.Sku ?? "-",
                    Quantity = item.Quantity,
                    SourceAvailableQuantity = snapshot.GetValueOrDefault(item.ProId, 0)
                };
            }).ToList(),
            CanEdit = note.Status == TransferNoteStatus.Draft,
            CanApprove = note.Status == TransferNoteStatus.Draft,
            CanReject = note.Status == TransferNoteStatus.Draft,
            CanConfirmTransferOut = note.Status == TransferNoteStatus.Approved,
            Timeline =
            [
                new TransferTimelineEntryViewModel
                {
                    Title = "Draft created",
                    Description = $"Transfer note created by {actorLookup.GetValueOrDefault(note.CreatedBy, note.CreatedBy)}.",
                    EventDate = note.CreatedDate
                },
                new TransferTimelineEntryViewModel
                {
                    Title = "Approval milestone",
                    Description = string.IsNullOrWhiteSpace(note.ApprovedBy)
                        ? "Waiting for chain manager review."
                        : $"Reviewed by {actorLookup.GetValueOrDefault(note.ApprovedBy, note.ApprovedBy)}.",
                    EventDate = note.ApprovedDate
                },
                new TransferTimelineEntryViewModel
                {
                    Title = "Transfer-out confirmation",
                    Description = note.TransferOutDate.HasValue
                        ? "Goods were confirmed as dispatched from the source location."
                        : "Transfer-out has not been confirmed yet.",
                    EventDate = note.TransferOutDate
                },
                new TransferTimelineEntryViewModel
                {
                    Title = "Transfer-in confirmation",
                    Description = note.TransferInDate.HasValue
                        ? "Destination confirmation was completed."
                        : "Awaiting transfer-in confirmation at the destination.",
                    EventDate = note.TransferInDate
                }
            ]
        };
    }

    public TransferNoteFormViewModel InitializeTransferDraft() =>
        PopulateTransferDraft(new TransferNoteFormViewModel());

    public TransferNoteFormViewModel HydrateTransferDraft(TransferNoteFormViewModel model) =>
        PopulateTransferDraft(model);

    public TransferNoteFormViewModel? PrepareTransferUpdate(string id)
    {
        var note = _transferService.GetTransferNote(id);
        if (note is null || note.Status != TransferNoteStatus.Draft)
        {
            return null;
        }

        var items = _transferService.GetTransferNoteItems(id);
        var model = new TransferNoteFormViewModel
        {
            SourceLocId = note.SourceLocId,
            DestinationLocId = note.DestinationLocId,
            Note = note.Note,
            Items = items.Select(item => new TransferNoteLineInputViewModel
            {
                ProductId = item.ProId,
                Quantity = item.Quantity
            }).ToList()
        };

        return PopulateTransferDraft(model);
    }

    public TransferNoteFormViewModel HydrateTransferUpdate(string id, TransferNoteFormViewModel model)
    {
        _ = id;
        return PopulateTransferDraft(model);
    }

    public ServiceResult CreateTransferNote(TransferNoteFormViewModel model)
    {
        var validation = _validationService.ValidateTransferDraft(model);
        if (!validation.Succeeded)
        {
            return ServiceResult.Failure(validation.Message);
        }

        return _transferService.CreateTransferNote(
            model.SourceLocId,
            model.DestinationLocId,
            model.Note,
            validation.RequestedLines,
            validation.InventoryWarnings);
    }

    public ServiceResult UpdateTransferNote(string id, TransferNoteFormViewModel model)
    {
        var note = _transferService.GetTransferNote(id);
        if (note is null)
        {
            return ServiceResult.Failure("The selected transfer note could not be found.");
        }

        if (note.Status != TransferNoteStatus.Draft)
        {
            return ServiceResult.Failure("This transfer note is no longer in Draft status and cannot be updated.");
        }

        var validation = _validationService.ValidateTransferDraft(model);
        if (!validation.Succeeded)
        {
            return ServiceResult.Failure(validation.Message);
        }

        return _transferService.UpdateTransferNote(
            id,
            model.SourceLocId,
            model.DestinationLocId,
            model.Note,
            validation.RequestedLines,
            validation.InventoryWarnings);
    }

    public ServiceResult ApproveTransferNote(string id)
    {
        var note = _transferService.GetTransferNote(id);
        if (note is null)
        {
            return ServiceResult.Failure("The selected transfer note could not be found.");
        }

        if (note.Status != TransferNoteStatus.Draft)
        {
            return ServiceResult.Failure("This transfer note is no longer in Draft status and cannot be approved.");
        }

        var items = _transferService.GetTransferNoteItems(id);
        var requestedLines = items.Select(item => new TransferLineRequest(item.ProId, item.Quantity)).ToList();
        var productCatalog = _productService.GetProductCatalog(items.Select(item => item.ProId));
        var inventoryValidation = _inventoryService.ValidateAvailability(
            note.SourceLocId,
            requestedLines,
            productCatalog,
            "Approval cannot continue because inventory is no longer sufficient.");

        return inventoryValidation is null
            ? _transferService.ReviewTransferNote(id, approve: true)
            : ServiceResult.Failure(inventoryValidation);
    }

    public ServiceResult RejectTransferNote(string id, string? reason = null) =>
        _transferService.ReviewTransferNote(id, approve: false, reason);

    public ServiceResult ConfirmTransferOut(string id)
    {
        var note = _transferService.GetTransferNote(id);
        if (note is null)
        {
            return ServiceResult.Failure("The selected transfer note could not be found.");
        }

        if (note.Status != TransferNoteStatus.Approved)
        {
            return ServiceResult.Failure("Transfer-out can only be confirmed for transfer notes in Approved status.");
        }

        var items = _transferService.GetTransferNoteItems(id);
        var requestedLines = items.Select(item => new TransferLineRequest(item.ProId, item.Quantity)).ToList();
        var productCatalog = _productService.GetProductCatalog(items.Select(item => item.ProId));
        var inventoryValidation = _inventoryService.ValidateAvailability(
            note.SourceLocId,
            requestedLines,
            productCatalog,
            "Transfer-out cannot be confirmed because source stock is insufficient.");
        if (inventoryValidation is not null)
        {
            return ServiceResult.Failure(inventoryValidation);
        }

        var inventoryUpdate = _inventoryService.DeductSourceInventory(note.SourceLocId, requestedLines);
        if (!inventoryUpdate.Succeeded)
        {
            return inventoryUpdate;
        }

        var result = _transferService.ConfirmTransferOut(id);
        if (result.Succeeded)
        {
            var refreshedNote = _transferService.GetTransferNote(id);
            if (refreshedNote is not null)
            {
                _auditLogService.LogTransferOut(refreshedNote, items);
            }
        }

        return result;
    }

    private TransferNoteFormViewModel PopulateTransferDraft(TransferNoteFormViewModel model)
    {
        model.Items ??= [];
        model.Items = model.Items
            .Where(item => !string.IsNullOrWhiteSpace(item.ProductId) || item.Quantity.HasValue)
            .ToList();

        while (model.Items.Count < 4)
        {
            model.Items.Add(new TransferNoteLineInputViewModel());
        }

        var selectedLocationIds = new[] { model.SourceLocId, model.DestinationLocId };
        model.LocationOptions = _locationService.GetLocationOptions(selectedLocationIds);

        var selectedProductIds = model.Items
            .Where(item => !string.IsNullOrWhiteSpace(item.ProductId))
            .Select(item => item.ProductId!);
        model.ProductOptions = _productService.GetProductOptions(selectedProductIds);

        var productCatalog = _productService.GetProductCatalog(selectedProductIds);
        var snapshot = string.IsNullOrWhiteSpace(model.SourceLocId)
            ? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            : _inventoryQueryService.GetSnapshot(model.SourceLocId, selectedProductIds);

        foreach (var item in model.Items)
        {
            item.AvailableQuantity = string.IsNullOrWhiteSpace(item.ProductId)
                ? null
                : snapshot.GetValueOrDefault(item.ProductId, 0);
        }

        var draftLines = model.Items
            .Where(item => !string.IsNullOrWhiteSpace(item.ProductId) && item.Quantity.HasValue && item.Quantity.Value > 0)
            .Select(item => new TransferLineRequest(item.ProductId!, item.Quantity!.Value))
            .ToList();

        model.InventoryWarnings = string.IsNullOrWhiteSpace(model.SourceLocId)
            ? []
            : _inventoryQueryService.GetWarnings(model.SourceLocId, draftLines, productCatalog);

        return model;
    }

    private static string FormatStatus(TransferNoteStatus status) => status switch
    {
        TransferNoteStatus.Draft => "Draft",
        TransferNoteStatus.TransferOutConfirmed => "Transfer Out Confirmed",
        TransferNoteStatus.Deactivated => "Deactivated",
        _ => status.ToString()
    };
}
