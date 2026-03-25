namespace OffroadVehicleRentals.Shared.Models;

public class ChecklistItem
{
    public int Id { get; set; }
    public int? RentalId { get; set; }
    public int? VehicleId { get; set; }
    public ChecklistType Type { get; set; }
    public string ItemText { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletedBy { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }

    public Rental? Rental { get; set; }
    public Vehicle? Vehicle { get; set; }
}
