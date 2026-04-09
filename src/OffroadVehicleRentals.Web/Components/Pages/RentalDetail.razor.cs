using Microsoft.AspNetCore.Components;
using OffroadVehicleRentals.Shared.Models;
using OffroadVehicleRentals.Web.Services;

namespace OffroadVehicleRentals.Web.Components.Pages;

public partial class RentalDetail
{
    [Parameter] public int Id { get; set; }

    [Inject] private ApiService ApiService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private Rental? rental;
    private List<ChecklistItem>? checklistItems;
    private List<Vehicle>? availableVehicles;
    private bool isLoading = true;
    private bool showEditForm;
    private Rental? editingRental;

    protected override async Task OnInitializedAsync()
    {
        await LoadRentalData();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadRentalData();
    }

    private async Task LoadRentalData()
    {
        isLoading = true;
        try
        {
            rental = await ApiService.GetRentalAsync(Id);

            if (rental != null)
            {
                try
                {
                    checklistItems = await ApiService.GetRentalChecklistItemsAsync(Id);
                }
                catch
                {
                    checklistItems = new List<ChecklistItem>();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading rental details: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task EditRental()
    {
        if (rental == null) return;

        // Load available vehicles for the dropdown
        try
        {
            availableVehicles = await ApiService.GetVehiclesAsync();
        }
        catch
        {
            availableVehicles = new List<Vehicle>();
        }

        editingRental = new Rental
        {
            Id = rental.Id,
            VehicleId = rental.VehicleId,
            CustomerName = rental.CustomerName,
            CustomerPhone = rental.CustomerPhone,
            CustomerEmail = rental.CustomerEmail,
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            StartHours = rental.StartHours,
            StartMileage = rental.StartMileage,
            EndHours = rental.EndHours,
            EndMileage = rental.EndMileage,
            Notes = rental.Notes
        };
        showEditForm = true;
    }

    private async Task SaveRental()
    {
        if (editingRental == null) return;

        try
        {
            await ApiService.UpdateRentalAsync(editingRental.Id, editingRental);
            CloseEditForm();
            await LoadRentalData();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving rental: {ex.Message}");
        }
    }

    private void CloseEditForm()
    {
        showEditForm = false;
        editingRental = null;
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

    private string GetDurationText()
    {
        if (rental == null) return "";
        var duration = rental.EndDate - rental.StartDate;
        if (duration.TotalDays >= 1)
        {
            var days = (int)Math.Ceiling(duration.TotalDays);
            return $"{days} day{(days != 1 ? "s" : "")}";
        }
        return $"{(int)duration.TotalHours}h {duration.Minutes}m";
    }

    private string GetRentalStatusText()
    {
        if (rental == null) return "";
        var now = DateTime.UtcNow;
        if (rental.EndDate < now) return "Completed";
        if (rental.StartDate <= now && rental.EndDate >= now) return "Active";
        return "Upcoming";
    }

    private string GetRentalStatusColor()
    {
        if (rental == null) return "secondary";
        var now = DateTime.UtcNow;
        if (rental.EndDate < now) return "secondary";
        if (rental.StartDate <= now && rental.EndDate >= now) return "success";
        return "primary";
    }
}

