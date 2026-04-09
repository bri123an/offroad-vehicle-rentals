using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OffroadVehicleRentals.Shared.Models;
using OffroadVehicleRentals.Web.Services;

namespace OffroadVehicleRentals.Web.Components.Pages;

public partial class Maintenance
{
    [Inject] private ApiService ApiService { get; set; } = default!;
    [Inject] private ExcelExportService ExcelExportService { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private List<MaintenanceRecord>? maintenanceRecords;
    private List<RepairRecord>? repairRecords;
    private List<Vehicle>? vehicles;
    private bool isLoadingMaintenance = true;
    private bool isLoadingRepairs = true;
    private bool showMaintenanceForm;
    private bool showRepairForm;
    private MaintenanceRecord? editingMaintenance;
    private RepairRecord? editingRepair;
    private string activeTab = "maintenance";
    private bool isExporting;

    // Maintenance filter fields
    private string filterMaintVehicle = "";
    private string filterMaintType = "";
    private string filterMaintDescription = "";
    private string filterMaintPerformedBy = "";

    // Repair filter fields
    private string filterRepairVehicle = "";
    private string filterRepairIssue = "";
    private string filterRepairDescription = "";
    private string filterRepairWarranty = "";
    private string filterRepairPerformedBy = "";

    private IEnumerable<MaintenanceRecord> FilteredMaintenanceRecords =>
        (maintenanceRecords ?? new List<MaintenanceRecord>()).Where(r =>
            (string.IsNullOrEmpty(filterMaintVehicle) || (r.Vehicle?.VehicleNumber ?? "").Contains(filterMaintVehicle, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterMaintType) || (r.MaintenanceType ?? "").Contains(filterMaintType, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterMaintDescription) || (r.Description ?? "").Contains(filterMaintDescription, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterMaintPerformedBy) || (r.PerformedBy ?? "").Contains(filterMaintPerformedBy, StringComparison.OrdinalIgnoreCase))
        );

    private IEnumerable<RepairRecord> FilteredRepairRecords =>
        (repairRecords ?? new List<RepairRecord>()).Where(r =>
            (string.IsNullOrEmpty(filterRepairVehicle) || (r.Vehicle?.VehicleNumber ?? "").Contains(filterRepairVehicle, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterRepairIssue) || r.IssueDescription.Contains(filterRepairIssue, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterRepairDescription) || r.RepairDescription.Contains(filterRepairDescription, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filterRepairWarranty) ||
                (filterRepairWarranty == "Yes" && r.IsWarrantyClaim) ||
                (filterRepairWarranty == "No" && !r.IsWarrantyClaim)) &&
            (string.IsNullOrEmpty(filterRepairPerformedBy) || (r.PerformedBy ?? "").Contains(filterRepairPerformedBy, StringComparison.OrdinalIgnoreCase))
        );

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        await LoadMaintenanceRecords();
        await LoadRepairRecords();
        await LoadVehicles();
    }

    private async Task LoadMaintenanceRecords()
    {
        isLoadingMaintenance = true;
        try
        {
            maintenanceRecords = await ApiService.GetMaintenanceRecordsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading maintenance records: {ex.Message}");
        }
        finally
        {
            isLoadingMaintenance = false;
        }
    }

    private async Task LoadRepairRecords()
    {
        isLoadingRepairs = true;
        try
        {
            repairRecords = await ApiService.GetRepairRecordsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading repair records: {ex.Message}");
        }
        finally
        {
            isLoadingRepairs = false;
        }
    }

    private async Task LoadVehicles()
    {
        try
        {
            vehicles = await ApiService.GetVehiclesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading vehicles: {ex.Message}");
        }
    }

    private void SetActiveTab(string tab)
    {
        activeTab = tab;
    }

    private void ShowAddMaintenanceForm()
    {
        editingMaintenance = new MaintenanceRecord
        {
            MaintenanceType = "",
            MaintenanceDate = DateTime.Now,
            HoursAtMaintenance = 0,
            MileageAtMaintenance = 0
        };
        showMaintenanceForm = true;
    }

    private void ShowAddRepairForm()
    {
        editingRepair = new RepairRecord
        {
            IssueDescription = "",
            RepairDescription = "",
            RepairDate = DateTime.Now
        };
        showRepairForm = true;
    }

    private async Task SaveMaintenance()
    {
        if (editingMaintenance == null) return;

        try
        {
            await ApiService.CreateMaintenanceRecordAsync(editingMaintenance);
            await LoadMaintenanceRecords();
            CloseMaintenanceForm();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving maintenance record: {ex.Message}");
        }
    }

    private async Task SaveRepair()
    {
        if (editingRepair == null) return;

        try
        {
            await ApiService.CreateRepairRecordAsync(editingRepair);
            await LoadRepairRecords();
            CloseRepairForm();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving repair record: {ex.Message}");
        }
    }

    private void CloseMaintenanceForm()
    {
        showMaintenanceForm = false;
        editingMaintenance = null;
    }

    private void CloseRepairForm()
    {
        showRepairForm = false;
        editingRepair = null;
    }

    private async Task ExportMaintenanceToExcel()
    {
        if (maintenanceRecords == null || !maintenanceRecords.Any()) return;

        isExporting = true;
        try
        {
            var fileBytes = ExcelExportService.ExportMaintenanceRecords(maintenanceRecords);
            var fileName = $"MaintenanceRecords_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            using var streamRef = new DotNetStreamReference(new MemoryStream(fileBytes));
            await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting maintenance records: {ex.Message}");
        }
        finally
        {
            isExporting = false;
        }
    }

    private async Task ExportRepairsToExcel()
    {
        if (repairRecords == null || !repairRecords.Any()) return;

        isExporting = true;
        try
        {
            var fileBytes = ExcelExportService.ExportRepairRecords(repairRecords);
            var fileName = $"RepairRecords_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            using var streamRef = new DotNetStreamReference(new MemoryStream(fileBytes));
            await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting repair records: {ex.Message}");
        }
        finally
        {
            isExporting = false;
        }
    }
}

