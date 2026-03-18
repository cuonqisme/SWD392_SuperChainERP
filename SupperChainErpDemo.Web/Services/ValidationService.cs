using SupperChainErpDemo.Web.Models;
using SupperChainErpDemo.Web.ViewModels.TransferNotes;

namespace SupperChainErpDemo.Web.Services;

public class ValidationService : IValidationService
{
    private readonly ILocationService _locationService;
    private readonly IProductService _productService;
    private readonly IInventoryQueryService _inventoryQueryService;

    public ValidationService(
        ILocationService locationService,
        IProductService productService,
        IInventoryQueryService inventoryQueryService)
    {
        _locationService = locationService;
        _productService = productService;
        _inventoryQueryService = inventoryQueryService;
    }

    public TransferDraftValidationResult ValidateTransferDraft(TransferNoteFormViewModel model)
    {
        var locationValidation = _locationService.ValidateActiveTransferLocations(model.SourceLocId, model.DestinationLocId);
        if (locationValidation is not null)
        {
            return TransferDraftValidationResult.Failure(locationValidation);
        }

        var requestedLines = model.Items
            .Where(item => !string.IsNullOrWhiteSpace(item.ProductId) || item.Quantity.HasValue)
            .ToList();

        if (requestedLines.Count == 0)
        {
            return TransferDraftValidationResult.Failure("Please add at least one transfer item.");
        }

        if (requestedLines.Any(item => string.IsNullOrWhiteSpace(item.ProductId) || !item.Quantity.HasValue || item.Quantity.Value <= 0))
        {
            return TransferDraftValidationResult.Failure("Each transfer line must include a product and a quantity greater than zero.");
        }

        if (requestedLines.GroupBy(item => item.ProductId, StringComparer.OrdinalIgnoreCase).Any(group => group.Count() > 1))
        {
            return TransferDraftValidationResult.Failure("Each product can only appear once in the same transfer note.");
        }

        var productCatalog = _productService.GetProductCatalog(requestedLines.Select(item => item.ProductId!));
        if (productCatalog.Count != requestedLines.Count || productCatalog.Values.Any(product => product.Status != RecordStatus.Active))
        {
            return TransferDraftValidationResult.Failure("Please choose valid active products for every transfer line.");
        }

        var normalizedLines = requestedLines
            .Select(item => new TransferLineRequest(item.ProductId!, item.Quantity!.Value))
            .ToList();

        var warnings = _inventoryQueryService.GetWarnings(model.SourceLocId, normalizedLines, productCatalog);
        return TransferDraftValidationResult.Success(normalizedLines, warnings);
    }
}
