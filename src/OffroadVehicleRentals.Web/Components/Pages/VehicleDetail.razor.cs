using Microsoft.AspNetCore.Components;
using OffroadVehicleRentals.Shared.Models;
using OffroadVehicleRentals.Web.Services;

namespace OffroadVehicleRentals.Web.Components.Pages;

public partial class VehicleDetail
{
    [Parameter] public int Id { get; set; }

    [Inject] private ApiService ApiService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private Vehicle? vehicle;
    private List<Rental>? rentals;
    private List<MaintenanceRecord>? maintenanceRecords;
    private List<RepairRecord>? repairRecords;
    private bool isLoading = true;
    private string activeTab = "rentals";
    private bool showEditForm;
    private Vehicle? editingVehicle;

    protected override async Task OnInitializedAsync()
    {
        await LoadVehicleData();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadVehicleData();
    }

    private async Task LoadVehicleData()
    {
        isLoading = true;
        try
        {
            vehicle = await ApiService.GetVehicleAsync(Id);

            if (vehicle != null)
            {
                // Load related records in parallel
                var rentalsTask = ApiService.GetRentalsAsync();
                var maintenanceTask = ApiService.GetVehicleMaintenanceRecordsAsync(Id);
                var repairsTask = ApiService.GetVehicleRepairRecordsAsync(Id);

                await Task.WhenAll(rentalsTask, maintenanceTask, repairsTask);

                rentals = (await rentalsTask).Where(r => r.VehicleId == Id).ToList();
                maintenanceRecords = await maintenanceTask;
                repairRecords = await repairsTask;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading vehicle details: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
    }

    private void EditVehicle()
    {
        if (vehicle == null) return;

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
        showEditForm = true;
    }

    private async Task SaveVehicle()
    {
        if (editingVehicle == null) return;

        try
        {
            await ApiService.UpdateVehicleAsync(editingVehicle.Id, editingVehicle);
            CloseEditForm();
            await LoadVehicleData();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving vehicle: {ex.Message}");
        }
    }

    private void CloseEditForm()
    {
        showEditForm = false;
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

