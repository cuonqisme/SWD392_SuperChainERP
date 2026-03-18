namespace SupperChainErpDemo.Web.Services;

public class NotificationMessage
{
    public required string Title { get; init; }

    public required string Message { get; init; }

    public required NotificationType Type { get; init; }
}
