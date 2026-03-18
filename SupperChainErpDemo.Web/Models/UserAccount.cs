namespace SupperChainErpDemo.Web.Models;

public class UserAccount
{
    public required string UserId { get; set; }

    public required string RoleId { get; set; }

    public required string Username { get; set; }

    public required string PasswordHash { get; set; }

    public required string FullName { get; set; }

    public required string Email { get; set; }

    public string Phone { get; set; } = string.Empty;

    public RecordStatus Status { get; set; } = RecordStatus.Active;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
