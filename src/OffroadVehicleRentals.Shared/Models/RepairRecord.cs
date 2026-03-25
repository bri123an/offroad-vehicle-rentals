namespace OffroadVehicleRentals.Shared.Models;

public class RepairRecord
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime RepairDate { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
    public string RepairDescription { get; set; } = string.Empty;
    public decimal? Cost { get; set; }
    public string? PerformedBy { get; set; }
    public bool IsWarrantyClaim { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public Vehicle? Vehicle { get; set; }
}
