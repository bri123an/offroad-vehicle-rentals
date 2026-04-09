using Microsoft.AspNetCore.Components;
using OffroadVehicleRentals.Shared.Models;
using OffroadVehicleRentals.Web.Services;

namespace OffroadVehicleRentals.Web.Components.Pages;

public partial class Home
{
    [Inject] private ApiService ApiService { get; set; } = default!;

    private List<Vehicle>? vehicles;
    private List<Rental>? upcomingRentals;
    private bool isLoading = true;
    private int availableCount;
    private int inUseCount;
    private int maintenanceCount;
    private int totalCount;

    protected override async Task OnInitializedAsync()
    {
        await LoadDashboardData();
    }

    private async Task LoadDashboardData()
    {
        isLoading = true;
        try
        {
            vehicles = await ApiService.GetVehiclesAsync();
            upcomingRentals = await ApiService.GetUpcomingRentalsAsync();

            if (vehicles != null)
            {
                totalCount = vehicles.Count;
                availableCount = vehicles.Count(v => v.Status == VehicleStatus.Available);
                inUseCount = vehicles.Count(v => v.Status == VehicleStatus.InUse);
                maintenanceCount = vehicles.Count(v => v.Status == VehicleStatus.InMaintenance || v.Status == VehicleStatus.InRepair);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dashboard: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
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

