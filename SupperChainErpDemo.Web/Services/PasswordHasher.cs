namespace SupperChainErpDemo.Web.Services;

public static class PasswordHasher
{
    public static string Hash(string password) => $"HASH::{password}";
}
