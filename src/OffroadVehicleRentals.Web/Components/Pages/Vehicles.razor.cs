using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OffroadVehicleRentals.Shared.Models;
using OffroadVehicleRentals.Web.Services;

namespace OffroadVehicleRentals.Web.Components.Pages;

public partial class Vehicles
{
    [Inject] private ApiService ApiService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<Vehicle>? vehicles;
    private bool isLoading = true;
    private bool showForm;
    private Vehicle? editingVehicle;

    // Filter fields
    private string filterVehicleNumber = "";
    private string filterType = "";
    private string filterMakeModel = "";
    private string filterYear = "";
    private string filterStatus = "";

    private IEnumerable<Vehicle> FilteredVehicles =>
        (vehicles ?? new List<Vehicle>()).Where(v =>
            (string.IsNullOrEmpty(filterVehicleNumber) || v.VehicleNumber.Contains(filterVehicleNumber, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterType) || v.Type.ToString() == filterType) &&
            (string.IsNullOrEmpty(filterMakeModel) || $"{v.Make} {v.Model}".Contains(filterMakeModel, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterYear) || v.Year.ToString().Contains(filterYear)) &&
            (string.IsNullOrEmpty(filterStatus) || v.Status.ToString() == filterStatus)
        );

    protected override async Task OnInitializedAsync()
    {
        await LoadVehicles();
    }

    private async Task LoadVehicles()
    {
        isLoading = true;
        try
        {
            vehicles = await ApiService.GetVehiclesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading vehicles: {ex.Message}");
            vehicles = new List<Vehicle>();
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ShowAddVehicleForm()
    {
        editingVehicle = new Vehicle
        {
            VehicleNumber = "",
            Make = "",
            Model = "",
            Year = DateTime.Now.Year,
            Type = VehicleType.FourWheeler,
            Status = VehicleStatus.Available,
            CurrentHours = 0,
            CurrentMileage = 0
        };
        showForm = true;
    }

    private void EditVehicle(Vehicle vehicle)
    {
        editingVehicle = new Vehicle
        {
            Id = vehicle.Id,
            VehicleNumber = vehicle.VehicleNumber,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Type = vehicle.Type,
            Status = vehicle.Status,
            CurrentHours = vehicle.CurrentHours,
            CurrentMileage = vehicle.CurrentMileage,
            Notes = vehicle.Notes
        };
        showForm = true;
    }

    private void ViewVehicle(int id)
    {
        Navigation.NavigateTo($"/vehicles/{id}");
    }

    private async Task SaveVehicle()
    {
        if (editingVehicle == null) return;

        try
        {
            if (editingVehicle.Id > 0)
            {
                await ApiService.UpdateVehicleAsync(editingVehicle.Id, editingVehicle);
            }
            else
            {
                await ApiService.CreateVehicleAsync(editingVehicle);
            }

            await LoadVehicles();
            CloseForm();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving vehicle: {ex.Message}");
        }
    }

    private async Task DeleteVehicle(int id)
    {
        if (!await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this vehicle?"))
            return;

        try
        {
            await ApiService.DeleteVehicleAsync(id);
            await LoadVehicles();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting vehicle: {ex.Message}");
        }
    }

    private void CloseForm()
    {
        showForm = false;
        editingVehicle = null;
    }

    private string GetStatusColor(VehicleStatus status)
    {
        return status switch
        {
            VehicleStatus.Available => "success",
            VehicleStatus.Reserved => "info",
            VehicleStatus.InUse => "primary",
            VehicleStatus.InMaintenance => "warning",
            VehicleStatus.InRepair => "warning",
            VehicleStatus.OutOfService => "danger",
            _ => "secondary"
        };
    }
}

