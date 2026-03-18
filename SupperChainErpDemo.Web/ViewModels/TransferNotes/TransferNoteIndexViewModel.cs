using Microsoft.AspNetCore.Mvc.Rendering;

namespace SupperChainErpDemo.Web.ViewModels.TransferNotes;

public class TransferNoteIndexViewModel
{
    public string? SearchTerm { get; init; }

    public string? StatusFilter { get; init; }

    public string? SourceFilter { get; init; }

    public string? DestinationFilter { get; init; }

    public DateTime? CreatedFrom { get; init; }

    public DateTime? CreatedTo { get; init; }

    public IReadOnlyList<SelectListItem> LocationOptions { get; init; } = [];

    public string? EmptyStateMessage { get; init; }

    public IReadOnlyList<TransferNoteIndexItemViewModel> TransferNotes { get; init; } = [];
}
