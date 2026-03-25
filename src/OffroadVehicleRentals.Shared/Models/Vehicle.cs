namespace OffroadVehicleRentals.Shared.Models;

public class Vehicle
{
    public int Id { get; set; }
    public required string VehicleNumber { get; set; }
    public required string Make { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public VehicleType Type { get; set; }
    public VehicleStatus Status { get; set; }
    public decimal CurrentHours { get; set; }
    public decimal CurrentMileage { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public decimal? NextMaintenanceHours { get; set; }
    public decimal? NextMaintenanceMileage { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    public ICollection<RepairRecord> RepairRecords { get; set; } = new List<RepairRecord>();
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
