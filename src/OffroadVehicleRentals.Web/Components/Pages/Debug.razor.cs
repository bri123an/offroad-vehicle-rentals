using Microsoft.AspNetCore.Components;
using OffroadVehicleRentals.Web.Services;

namespace OffroadVehicleRentals.Web.Components.Pages;

public partial class Debug
{
    [Inject] private ApiService ApiService { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;

    private bool isLoading;
    private string? errorMessage;
    private int vehicleCount;
    private string? responseData;

    protected override async Task OnInitializedAsync()
    {
        await TestApi();
    }

    private async Task TestApi()
    {
        isLoading = true;
        errorMessage = null;
        responseData = null;

        try
        {
            var vehicles = await ApiService.GetVehiclesAsync();
            vehicleCount = vehicles?.Count ?? 0;
            responseData = System.Text.Json.JsonSerializer.Serialize(vehicles, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            errorMessage = $"{ex.GetType().Name}: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nInner Exception:\n{ex.InnerException.GetType().Name}: {ex.InnerException.Message}";
            }
        }
        finally
        {
            isLoading = false;
        }
    }
}

