using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OffroadVehicleRentals.Shared.Models;
using OffroadVehicleRentals.Web.Services;

namespace OffroadVehicleRentals.Web.Components.Pages;

public partial class Rentals
{
    [Inject] private ApiService ApiService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<Rental>? allRentals;
    private List<Rental>? upcomingRentals;
    private List<Rental>? activeRentals;
    private List<Rental>? displayedRentals;
    private List<Vehicle>? availableVehicles;
    private bool isLoading = true;
    private bool showForm;
    private Rental? editingRental;
    private string activeTab = "upcoming";

    // Filter fields
    private string filterRentalVehicle = "";
    private string filterRentalCustomer = "";
    private string filterRentalContact = "";
    private string filterPreCheck = "";
    private string filterPostCheck = "";

    private IEnumerable<Rental> FilteredRentals =>
        (displayedRentals ?? new List<Rental>()).Where(r =>
            (string.IsNullOrEmpty(filterRentalVehicle) || (r.Vehicle?.VehicleNumber ?? "").Contains(filterRentalVehicle, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterRentalCustomer) || r.CustomerName.Contains(filterRentalCustomer, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterRentalContact) ||
                (r.CustomerPhone ?? "").Contains(filterRentalContact, StringComparison.OrdinalIgnoreCase) ||
                (r.CustomerEmail ?? "").Contains(filterRentalContact, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterPreCheck) ||
                (filterPreCheck == "Complete" && r.PreRentalChecklistCompleted) ||
                (filterPreCheck == "Pending" && !r.PreRentalChecklistCompleted)) &&
            (string.IsNullOrEmpty(filterPostCheck) ||
                (filterPostCheck == "Complete" && r.PostRentalChecklistCompleted) ||
                (filterPostCheck == "Pending" && !r.PostRentalChecklistCompleted))
        );

    protected override async Task OnInitializedAsync()
    {
        await LoadRentals();
        await LoadAvailableVehicles();
    }

    private async Task LoadRentals()
    {
        isLoading = true;
        try
        {
            allRentals = await ApiService.GetRentalsAsync();
            upcomingRentals = await ApiService.GetUpcomingRentalsAsync();
            activeRentals = await ApiService.GetActiveRentalsAsync();
            SetDisplayedRentals();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading rentals: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task LoadAvailableVehicles()
    {
        try
        {
            availableVehicles = await ApiService.GetAvailableVehiclesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading vehicles: {ex.Message}");
        }
    }

    private void SetActiveTab(string tab)
    {
        activeTab = tab;
        SetDisplayedRentals();
    }

    private void SetDisplayedRentals()
    {
        displayedRentals = activeTab switch
        {
            "upcoming" => upcomingRentals,
            "active" => activeRentals,
            "all" => allRentals,
            _ => upcomingRentals
        };
    }

    private void ShowAddRentalForm()
    {
        editingRental = new Rental
        {
            CustomerName = "",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1),
            StartHours = 0,
            StartMileage = 0
        };
        showForm = true;
    }

    private void EditRental(Rental rental)
    {
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
        showForm = true;
    }

    private void ViewRental(int id)
    {
        Navigation.NavigateTo($"/rentals/{id}");
    }

    private async Task SaveRental()
    {
        if (editingRental == null) return;

        try
        {
            if (editingRental.Id > 0)
            {
                await ApiService.UpdateRentalAsync(editingRental.Id, editingRental);
            }
            else
            {
                await ApiService.CreateRentalAsync(editingRental);
            }

            await LoadRentals();
            CloseForm();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving rental: {ex.Message}");
        }
    }

    private async Task DeleteRental(int id)
    {
        if (!await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this rental?"))
            return;

        try
        {
            await ApiService.DeleteRentalAsync(id);
            await LoadRentals();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting rental: {ex.Message}");
        }
    }

    private void CloseForm()
    {
        showForm = false;
        editingRental = null;
    }
}

