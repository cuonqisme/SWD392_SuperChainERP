using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.Services;

public class FilterService : IFilterService
{
    public IReadOnlyList<TransferNote> ApplyTransferNoteFilters(
        IEnumerable<TransferNote> notes,
        TransferNoteFilterCriteria criteria)
    {
        var query = notes;

        if (Enum.TryParse<TransferNoteStatus>(criteria.StatusFilter, true, out var status))
        {
            query = query.Where(note => note.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SourceLocId))
        {
            query = query.Where(note => note.SourceLocId.Equals(criteria.SourceLocId, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.DestinationLocId))
        {
            query = query.Where(note => note.DestinationLocId.Equals(criteria.DestinationLocId, StringComparison.OrdinalIgnoreCase));
        }

        if (criteria.CreatedFrom.HasValue)
        {
            var createdFrom = criteria.CreatedFrom.Value.Date;
            query = query.Where(note => note.CreatedDate.Date >= createdFrom);
        }

        if (criteria.CreatedTo.HasValue)
        {
            var createdTo = criteria.CreatedTo.Value.Date;
            query = query.Where(note => note.CreatedDate.Date <= createdTo);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var keyword = criteria.SearchTerm.Trim();
            query = query.Where(note =>
                note.TransferNo.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                note.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderByDescending(note => note.CreatedDate)
            .ThenByDescending(note => note.TransferNo)
            .ToList();
    }
}
