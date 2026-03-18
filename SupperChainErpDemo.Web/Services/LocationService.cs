using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupperChainErpDemo.Web.Data;
using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public class LocationService : ILocationService
{
    private readonly AppDbContext _dbContext;

    public LocationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyList<SelectListItem> GetLocationOptions(IEnumerable<string>? includeLocationIds = null)
    {
        var selectedIds = includeLocationIds?
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        return _dbContext.Locations.AsNoTracking()
            .Where(location => location.Status == RecordStatus.Active || selectedIds.Contains(location.LocationId))
            .OrderBy(location => location.LocationType)
            .ThenBy(location => location.LocationName)
            .Select(location => new SelectListItem(
                $"{location.LocationName} ({location.LocationType})",
                location.LocationId))
            .ToList();
    }

    public IReadOnlyDictionary<string, Location> GetLocationLookup() =>
        _dbContext.Locations.AsNoTracking()
            .ToDictionary(location => location.LocationId, StringComparer.OrdinalIgnoreCase);

    public string? ValidateActiveTransferLocations(string sourceLocId, string destinationLocId)
    {
        if (string.IsNullOrWhiteSpace(sourceLocId) || string.IsNullOrWhiteSpace(destinationLocId))
        {
            return "Source and destination locations are required.";
        }

        if (sourceLocId.Equals(destinationLocId, StringComparison.OrdinalIgnoreCase))
        {
            return "Source and destination locations must be different.";
        }

        var locations = _dbContext.Locations.AsNoTracking()
            .Where(location => location.LocationId == sourceLocId || location.LocationId == destinationLocId)
            .ToList();

        if (locations.Count != 2)
        {
            return "Please choose valid source and destination locations.";
        }

        return locations.Any(location => location.Status != RecordStatus.Active)
            ? "Transfer notes can only use active locations."
            : null;
    }
}
