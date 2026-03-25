namespace OffroadVehicleRentals.Shared.Models;

public class Rental
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public required string CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal StartHours { get; set; }
    public decimal? EndHours { get; set; }
    public decimal StartMileage { get; set; }
    public decimal? EndMileage { get; set; }
    public bool PreRentalChecklistCompleted { get; set; }
    public bool PostRentalChecklistCompleted { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public Vehicle Vehicle { get; set; } = null!;
    public ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();
}
