namespace OffroadVehicleRentals.Shared.Models;

public class MaintenanceRecord
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal HoursAtMaintenance { get; set; }
    public decimal MileageAtMaintenance { get; set; }
    public decimal? Cost { get; set; }
    public string? PerformedBy { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public Vehicle? Vehicle { get; set; }
}
