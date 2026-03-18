using System.ComponentModel.DataAnnotations;

namespace SupperChainErpDemo.Web.ViewModels.Roles;

public class RoleFormViewModel
{
    [Required]
    [Display(Name = "Role name")]
    public string RoleName { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    public List<string> SelectedPermissions { get; set; } = [];

    public IReadOnlyList<string> AvailablePermissions { get; set; } = [];
}
