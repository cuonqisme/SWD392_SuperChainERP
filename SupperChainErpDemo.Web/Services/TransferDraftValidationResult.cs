namespace SupperChainErpDemo.Web.Services;

public sealed class TransferDraftValidationResult
{
    private TransferDraftValidationResult(
        bool succeeded,
        string message,
        IReadOnlyList<TransferLineRequest>? requestedLines = null,
        IReadOnlyList<string>? inventoryWarnings = null)
    {
        Succeeded = succeeded;
        Message = message;
        RequestedLines = requestedLines ?? [];
        InventoryWarnings = inventoryWarnings ?? [];
    }

    public bool Succeeded { get; }

    public string Message { get; }

    public IReadOnlyList<TransferLineRequest> RequestedLines { get; }

    public IReadOnlyList<string> InventoryWarnings { get; }

    public static TransferDraftValidationResult Failure(string message) => new(false, message);

    public static TransferDraftValidationResult Success(
        IReadOnlyList<TransferLineRequest> requestedLines,
        IReadOnlyList<string> inventoryWarnings) =>
        new(true, string.Empty, requestedLines, inventoryWarnings);
}
