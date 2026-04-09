using Microsoft.AspNetCore.Components;
using OffroadVehicleRentals.Shared.Models;
using OffroadVehicleRentals.Web.Services;

namespace OffroadVehicleRentals.Web.Components.Pages;

public partial class DebugTemplates
{
    [Inject] private ApiService ApiService { get; set; } = default!;

    private List<ChecklistTemplate>? templates;
    private string? rawJson;
    private string? errorMessage;
    private bool isLoading;

    private async Task LoadTemplates()
    {
        isLoading = true;
        errorMessage = null;
        rawJson = null;
        templates = null;
        StateHasChanged();

        try
        {
            templates = await ApiService.GetChecklistTemplatesAsync();
            rawJson = $"Templates loaded: {templates?.Count ?? 0}";
        }
        catch (Exception ex)
        {
            errorMessage = $"Exception: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nInner Exception: {ex.InnerException.Message}";
            }
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
}

