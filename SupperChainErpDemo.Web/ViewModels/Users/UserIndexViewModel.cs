using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.ViewModels.Users;

public class UserIndexViewModel
{
    public string? Keyword { get; set; }

    public string? StatusFilter { get; set; }

    public IReadOnlyList<UserAccount> Users { get; init; } = [];

    public IReadOnlyDictionary<string, string> RoleNames { get; init; } = new Dictionary<string, string>();
}
