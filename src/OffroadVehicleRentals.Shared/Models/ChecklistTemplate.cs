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
    public string Name { get; set; } = string.Empty;
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
    public string ItemText { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsRequired { get; set; }

    public ChecklistTemplate? Template { get; set; }
}
