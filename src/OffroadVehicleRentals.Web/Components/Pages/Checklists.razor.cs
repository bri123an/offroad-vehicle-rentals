using Microsoft.AspNetCore.Components;
using OffroadVehicleRentals.Shared.Models;
using OffroadVehicleRentals.Web.Services;

namespace OffroadVehicleRentals.Web.Components.Pages;

public partial class Checklists
{
    [Inject] private ApiService ApiService { get; set; } = default!;

    private List<Vehicle>? vehicles;
    private List<Rental>? upcomingRentals;
    private List<ChecklistTemplate>? dailyTemplates;
    private List<ChecklistTemplate>? allTemplates;
    private List<ChecklistItem>? currentChecklistItems;

    private int selectedDailyVehicleId;
    private int selectedDailyTemplateId;
    private int selectedRentalId;
    private int selectedRentalChecklistType = 1;
    private string currentChecklistTitle = "";
    private string activeTab = "templates";
    private int expandedTemplateId;
    private bool isLoading = true;
    private string? errorMessage;

    private string GetTypeBadgeClass(ChecklistType type) => type switch
    {
        ChecklistType.DailyInspection => "bg-primary",
        ChecklistType.PreRental => "bg-success",
        ChecklistType.PostRental => "bg-warning text-dark",
        _ => "bg-secondary"
    };

    private void SetTabDaily() => activeTab = "daily";
    private void SetTabRental() => activeTab = "rental";
    private void SetTabTemplates() => activeTab = "templates";

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        errorMessage = null;
        StateHasChanged();

        try
        {
            Console.WriteLine("[LoadData] Starting...");
            vehicles = await ApiService.GetVehiclesAsync();
            Console.WriteLine($"[LoadData] Loaded {vehicles?.Count ?? 0} vehicles");

            upcomingRentals = await ApiService.GetUpcomingRentalsAsync();
            Console.WriteLine($"[LoadData] Loaded {upcomingRentals?.Count ?? 0} rentals");

            allTemplates = await ApiService.GetChecklistTemplatesAsync();
            Console.WriteLine($"[LoadData] Loaded {allTemplates?.Count ?? 0} allTemplates");

            dailyTemplates = allTemplates;
            Console.WriteLine($"[LoadData] Set dailyTemplates to {dailyTemplates?.Count ?? 0} templates");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LoadData] ERROR: {ex.Message}");
            Console.WriteLine($"[LoadData] Stack: {ex.StackTrace}");
            errorMessage = $"Failed to load data: {ex.Message}";
            allTemplates = new List<ChecklistTemplate>();
            dailyTemplates = new List<ChecklistTemplate>();
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void OnDailyVehicleChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var vehicleId))
        {
            selectedDailyVehicleId = vehicleId;
            selectedDailyTemplateId = 0;

            if (vehicles != null && dailyTemplates != null)
            {
                var vehicle = vehicles.FirstOrDefault(v => v.Id == vehicleId);
                if (vehicle != null)
                {
                    var matchingTemplate = dailyTemplates
                        .FirstOrDefault(t => t.VehicleType == vehicle.Type);
                    if (matchingTemplate != null)
                    {
                        selectedDailyTemplateId = matchingTemplate.Id;
                    }
                }
            }
        }
    }

    private void OnRentalChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var rentalId))
        {
            selectedRentalId = rentalId;
        }
    }

    private async Task CreateDailyChecklist()
    {
        if (selectedDailyVehicleId == 0 || selectedDailyTemplateId == 0) return;

        try
        {
            currentChecklistItems = await ApiService.CreateChecklistItemsFromTemplateAsync(
                selectedDailyTemplateId, null, selectedDailyVehicleId);

            var vehicle = vehicles?.FirstOrDefault(v => v.Id == selectedDailyVehicleId);
            currentChecklistTitle = $"Daily Inspection - {vehicle?.VehicleNumber}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating checklist: {ex.Message}");
        }
    }

    private async Task CreateRentalChecklist()
    {
        if (selectedRentalId == 0) return;

        try
        {
            var rental = upcomingRentals?.FirstOrDefault(r => r.Id == selectedRentalId);
            if (rental == null) return;

            var checklistType = (ChecklistType)selectedRentalChecklistType;

            var template = allTemplates?.FirstOrDefault(t =>
                t.Type == checklistType &&
                (t.VehicleType == rental.Vehicle?.Type || t.VehicleType == null));

            if (template == null)
            {
                template = allTemplates?.FirstOrDefault(t => t.Type == checklistType);
            }

            if (template != null)
            {
                currentChecklistItems = await ApiService.CreateChecklistItemsFromTemplateAsync(
                    template.Id, selectedRentalId, rental.VehicleId);

                currentChecklistTitle = $"{(checklistType == ChecklistType.PreRental ? "Pre" : "Post")}-Rental Checklist - {rental.CustomerName}";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating rental checklist: {ex.Message}");
        }
    }

    private async Task ToggleChecklistItem(ChecklistItem item)
    {
        item.IsCompleted = !item.IsCompleted;
        item.CompletedDate = item.IsCompleted ? DateTime.UtcNow : null;
        item.CompletedBy = item.IsCompleted ? "Current User" : null;

        try
        {
            await ApiService.UpdateChecklistItemAsync(item.Id, item);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating checklist item: {ex.Message}");
            item.IsCompleted = !item.IsCompleted;
        }
    }

    private async Task CompleteChecklist()
    {
        currentChecklistItems = null;
        currentChecklistTitle = "";
        selectedDailyVehicleId = 0;
        selectedDailyTemplateId = 0;
        selectedRentalId = 0;

        await LoadData();
    }

    private void CancelChecklist()
    {
        currentChecklistItems = null;
        currentChecklistTitle = "";
        selectedDailyVehicleId = 0;
        selectedDailyTemplateId = 0;
        selectedRentalId = 0;
    }
}

