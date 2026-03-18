namespace SupperChainErpDemo.Web.Services;

public class TransferNoteFilterCriteria
{
    public string? SearchTerm { get; init; }

    public string? StatusFilter { get; init; }

    public string? SourceLocId { get; init; }

    public string? DestinationLocId { get; init; }

    public DateTime? CreatedFrom { get; init; }

    public DateTime? CreatedTo { get; init; }
}
