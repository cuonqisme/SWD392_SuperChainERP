using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SupperChainErpDemo.Web.Services;

public class NotificationService : INotificationService
{
    public const string TempDataKey = "App.Notification";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

    public NotificationService(
        IHttpContextAccessor httpContextAccessor,
        ITempDataDictionaryFactory tempDataDictionaryFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _tempDataDictionaryFactory = tempDataDictionaryFactory;
    }

    public void Success(string title, string message) => SetMessage(title, message, NotificationType.Success);

    public void Error(string title, string message) => SetMessage(title, message, NotificationType.Error);

    public void Info(string title, string message) => SetMessage(title, message, NotificationType.Info);

    private void SetMessage(string title, string message, NotificationType type)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }

        var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);
        tempData[TempDataKey] = JsonSerializer.Serialize(new NotificationMessage
        {
            Title = title,
            Message = message,
            Type = type
        });
    }
}
