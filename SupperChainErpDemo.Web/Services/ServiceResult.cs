namespace SupperChainErpDemo.Web.Services;

public class ServiceResult
{
    private ServiceResult(bool succeeded, string message)
    {
        Succeeded = succeeded;
        Message = message;
    }

    public bool Succeeded { get; }

    public string Message { get; }

    public static ServiceResult Success(string message) => new(true, message);

    public static ServiceResult Failure(string message) => new(false, message);
}
