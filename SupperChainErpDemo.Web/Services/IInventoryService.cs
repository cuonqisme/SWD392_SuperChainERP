using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public interface IInventoryService
{
    IReadOnlyDictionary<string, int> GetSnapshot(string sourceLocId, IEnumerable<string> productIds);

    IReadOnlyList<string> GetWarnings(
        string sourceLocId,
        IReadOnlyCollection<TransferLineRequest> requestedLines,
        IReadOnlyDictionary<string, Product> productCatalog);

    string? ValidateAvailability(
        string sourceLocId,
        IReadOnlyCollection<TransferLineRequest> requestedLines,
        IReadOnlyDictionary<string, Product> productCatalog,
        string failurePrefix);

    ServiceResult DeductSourceInventory(
        string sourceLocId,
        IReadOnlyCollection<TransferLineRequest> requestedLines);
}
