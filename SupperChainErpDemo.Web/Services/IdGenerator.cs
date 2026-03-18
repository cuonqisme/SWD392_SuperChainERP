namespace SupperChainErpDemo.Web.Services;

public static class IdGenerator
{
    public static string NextId(IEnumerable<string> existingIds, string prefix)
    {
        var nextNumber = existingIds
            .Select(id => id.Replace(prefix, string.Empty))
            .Select(raw => int.TryParse(raw, out var value) ? value : 0)
            .DefaultIfEmpty()
            .Max() + 1;

        return $"{prefix}{nextNumber:000}";
    }
}
