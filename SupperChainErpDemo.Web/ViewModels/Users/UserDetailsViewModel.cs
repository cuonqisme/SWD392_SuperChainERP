using SupperChainErpDemo.Web.Models;

namespace SupperChainErpDemo.Web.ViewModels.Users;

public class UserDetailsViewModel
{
    public required UserAccount User { get; init; }

    public required string RoleName { get; init; }
}
