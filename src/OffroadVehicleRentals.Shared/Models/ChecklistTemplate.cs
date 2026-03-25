namespace OffroadVehicleRentals.Shared.Models;

public enum ChecklistType
{
    DailyInspection,
    PreRental,
    PostRental
}

public class ChecklistTemplate
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ChecklistType Type { get; set; }
    public VehicleType? VehicleType { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public ICollection<ChecklistTemplateItem> TemplateItems { get; set; } = new List<ChecklistTemplateItem>();
}

public class ChecklistTemplateItem
{
    public int Id { get; set; }
    public int ChecklistTemplateId { get; set; }
    public required string ItemText { get; set; }
    public int SortOrder { get; set; }
    public bool IsRequired { get; set; }

    public ChecklistTemplate Template { get; set; } = null!;
}
