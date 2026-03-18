namespace SupperChainErpDemo.Web.Models;

public class Product
{
    public required string ProductId { get; set; }

    public required string CategoryId { get; set; }

    public required string ProductName { get; set; }

    public required string Sku { get; set; }

    public required string Barcode { get; set; }

    public decimal BasePrice { get; set; }

    public string Description { get; set; } = string.Empty;

    public RecordStatus Status { get; set; } = RecordStatus.Active;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
