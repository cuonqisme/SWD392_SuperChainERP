using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SupperChainErpDemo.Web.ViewModels.Users;

public class UserFormViewModel
{
    [Required]
    [Display(Name = "Assigned role")]
    public string RoleId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "Temporary password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone")]
    public string Phone { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> RoleOptions { get; set; } = [];
}
