using Microsoft.AspNetCore.Mvc.Rendering;
using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public interface ILocationService
{
    IReadOnlyList<SelectListItem> GetLocationOptions(IEnumerable<string>? includeLocationIds = null);

    IReadOnlyDictionary<string, Location> GetLocationLookup();

    string? ValidateActiveTransferLocations(string sourceLocId, string destinationLocId);
}
