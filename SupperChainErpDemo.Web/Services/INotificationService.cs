namespace SupperChainErpDemo.Web.Services;

public interface INotificationService
{
    void Success(string title, string message);

    void Error(string title, string message);

    void Info(string title, string message);
}
