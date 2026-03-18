using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public class InventoryService : IInventoryService
{
    private readonly object _snapshotLock = new();

    private readonly Dictionary<string, Dictionary<string, int>> _inventoryByLocation =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["LOC-001"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["PRO-001"] = 70,
                ["PRO-002"] = 20
            },
            ["LOC-002"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["PRO-001"] = 14,
                ["PRO-002"] = 9
            },
            ["LOC-003"] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["PRO-001"] = 8,
                ["PRO-002"] = 5
            }
        };

    public IReadOnlyDictionary<string, int> GetSnapshot(string sourceLocId, IEnumerable<string> productIds)
    {
        lock (_snapshotLock)
        {
            var locationInventory = _inventoryByLocation.GetValueOrDefault(sourceLocId, []);
            return productIds
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    productId => productId,
                    productId => locationInventory.GetValueOrDefault(productId, 0),
                    StringComparer.OrdinalIgnoreCase);
        }
    }

    public IReadOnlyList<string> GetWarnings(
        string sourceLocId,
        IReadOnlyCollection<TransferLineRequest> requestedLines,
        IReadOnlyDictionary<string, Product> productCatalog)
    {
        var snapshot = GetSnapshot(sourceLocId, requestedLines.Select(line => line.ProductId));
        return requestedLines
            .Where(line => line.Quantity > snapshot.GetValueOrDefault(line.ProductId, 0))
            .Select(line =>
            {
                var productName = productCatalog.GetValueOrDefault(line.ProductId)?.ProductName ?? line.ProductId;
                var available = snapshot.GetValueOrDefault(line.ProductId, 0);
                return $"{productName}: requested {line.Quantity}, available {available} at the source location.";
            })
            .ToList();
    }

    public string? ValidateAvailability(
        string sourceLocId,
        IReadOnlyCollection<TransferLineRequest> requestedLines,
        IReadOnlyDictionary<string, Product> productCatalog,
        string failurePrefix)
    {
        var warning = GetWarnings(sourceLocId, requestedLines, productCatalog).FirstOrDefault();
        return warning is null
            ? null
            : $"{failurePrefix} {warning}";
    }

    public ServiceResult DeductSourceInventory(
        string sourceLocId,
        IReadOnlyCollection<TransferLineRequest> requestedLines)
    {
        lock (_snapshotLock)
        {
            if (!_inventoryByLocation.TryGetValue(sourceLocId, out var locationInventory))
            {
                locationInventory = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                _inventoryByLocation[sourceLocId] = locationInventory;
            }

            foreach (var line in requestedLines)
            {
                var currentQuantity = locationInventory.GetValueOrDefault(line.ProductId, 0);
                if (currentQuantity < line.Quantity)
                {
                    return ServiceResult.Failure($"Inventory update failed for product {line.ProductId} because the source quantity changed during processing.");
                }
            }

            foreach (var line in requestedLines)
            {
                locationInventory[line.ProductId] = locationInventory.GetValueOrDefault(line.ProductId, 0) - line.Quantity;
            }
        }

        return ServiceResult.Success("Source inventory was deducted successfully.");
    }
}
