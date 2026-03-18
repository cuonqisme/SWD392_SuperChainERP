using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public class InventoryQueryService : IInventoryQueryService
{
    private readonly IInventoryService _inventoryService;

    public InventoryQueryService(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public IReadOnlyDictionary<string, int> GetSnapshot(string sourceLocId, IEnumerable<string> productIds)
        => _inventoryService.GetSnapshot(sourceLocId, productIds);

    public IReadOnlyList<string> GetWarnings(
        string sourceLocId,
        IReadOnlyCollection<TransferLineRequest> requestedLines,
        IReadOnlyDictionary<string, Product> productCatalog)
        => _inventoryService.GetWarnings(sourceLocId, requestedLines, productCatalog);
}
