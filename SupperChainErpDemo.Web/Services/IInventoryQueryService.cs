using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public interface IInventoryQueryService
{
    IReadOnlyDictionary<string, int> GetSnapshot(string sourceLocId, IEnumerable<string> productIds);

    IReadOnlyList<string> GetWarnings(
        string sourceLocId,
        IReadOnlyCollection<TransferLineRequest> requestedLines,
        IReadOnlyDictionary<string, Product> productCatalog);
}
