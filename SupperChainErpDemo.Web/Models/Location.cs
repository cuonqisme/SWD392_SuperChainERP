namespace SupperChainErpDemo.Web.Models;

public class Location
{
    public required string LocationId { get; set; }

    public required string LocationCode { get; set; }

    public required string LocationName { get; set; }

    public required string LocationType { get; set; }

    public RecordStatus Status { get; set; } = RecordStatus.Active;
}
