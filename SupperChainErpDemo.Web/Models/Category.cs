namespace SupperChainErpDemo.Web.Models;

public class Category
{
    public required string CategoryId { get; set; }

    public required string CategoryName { get; set; }

    public string Description { get; set; } = string.Empty;

    public required string SkuPrefix { get; set; }

    public RecordStatus Status { get; set; } = RecordStatus.Active;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
