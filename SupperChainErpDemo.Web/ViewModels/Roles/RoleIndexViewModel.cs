using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.ViewModels.Roles;

public class RoleIndexViewModel
{
    public string? StatusFilter { get; set; }

    public IReadOnlyList<Role> Roles { get; init; } = [];
}
