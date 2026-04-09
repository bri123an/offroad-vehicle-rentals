using Microsoft.AspNetCore.Components;
using OffroadVehicleRentals.Shared.Models;
using OffroadVehicleRentals.Web.Services;

namespace OffroadVehicleRentals.Web.Components.Pages;

public partial class Schedule
{
    [Inject] private ApiService ApiService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private List<Rental>? rentals;
    private List<Vehicle>? vehicles;
    private bool isLoading = true;
    private string activeView = "calendar";
    private DateTime currentDate = DateTime.Today;
    private DateTime timelineStart;
    private DateTime timelineEnd;
    private List<CalendarDay> calendarDays = new();
    private Rental? selectedRental;

    protected override async Task OnInitializedAsync()
    {
        SetTimelineWeek(DateTime.Today);
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        try
        {
            rentals = await ApiService.GetRentalsAsync();
            vehicles = await ApiService.GetVehiclesAsync();
            BuildCalendarDays();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading schedule data: {ex.Message}");
            rentals = new List<Rental>();
            vehicles = new List<Vehicle>();
        }
        finally
        {
            isLoading = false;
        }
    }

    // --- Calendar View Logic ---

    private void BuildCalendarDays()
    {
        calendarDays.Clear();

        var firstOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        var lastOfMonth = firstOfMonth.AddMonths(1).AddDays(-1);

        var calStart = firstOfMonth.AddDays(-(int)firstOfMonth.DayOfWeek);
        var calEnd = lastOfMonth.AddDays(6 - (int)lastOfMonth.DayOfWeek);

        for (var d = calStart; d <= calEnd; d = d.AddDays(1))
        {
            var day = new CalendarDay
            {
                Date = d,
                IsCurrentMonth = d.Month == currentDate.Month && d.Year == currentDate.Year,
                IsToday = d.Date == DateTime.Today,
                Rentals = GetRentalsForDay(d)
            };
            calendarDays.Add(day);
        }
    }

    private List<Rental> GetRentalsForDay(DateTime date)
    {
        if (rentals == null) return new List<Rental>();
        return rentals.Where(r =>
            r.StartDate.ToLocalTime().Date <= date.Date &&
            r.EndDate.ToLocalTime().Date >= date.Date
        ).OrderBy(r => r.StartDate).ToList();
    }

    private bool IsRentalStart(Rental rental, DateTime date)
    {
        var start = rental.StartDate.ToLocalTime().Date;
        if (start == date.Date) return true;
        if (start < calendarDays.First().Date && date == calendarDays.First().Date) return true;
        if (date.DayOfWeek == DayOfWeek.Sunday && start < date.Date) return true;
        return false;
    }

    private string GetEventPositionClass(Rental rental, DateTime date)
    {
        var startLocal = rental.StartDate.ToLocalTime().Date;
        var endLocal = rental.EndDate.ToLocalTime().Date;

        if (startLocal == date.Date && endLocal == date.Date)
            return "event-single";
        if (startLocal == date.Date)
            return "event-start";
        if (endLocal == date.Date || date.DayOfWeek == DayOfWeek.Saturday)
            return "event-end";
        return "event-middle";
    }

    private void PreviousMonth()
    {
        currentDate = currentDate.AddMonths(-1);
        BuildCalendarDays();
    }

    private void NextMonth()
    {
        currentDate = currentDate.AddMonths(1);
        BuildCalendarDays();
    }

    private void GoToToday()
    {
        currentDate = DateTime.Today;
        BuildCalendarDays();
    }

    // --- Timeline View Logic ---

    private void SetTimelineWeek(DateTime referenceDate)
    {
        var dayOfWeek = (int)referenceDate.DayOfWeek;
        var monday = referenceDate.AddDays(-(dayOfWeek == 0 ? 6 : dayOfWeek - 1));
        timelineStart = monday;
        timelineEnd = monday.AddDays(13);
    }

    private void PreviousWeek()
    {
        timelineStart = timelineStart.AddDays(-7);
        timelineEnd = timelineEnd.AddDays(-7);
    }

    private void NextWeek()
    {
        timelineStart = timelineStart.AddDays(7);
        timelineEnd = timelineEnd.AddDays(7);
    }

    private void GoToThisWeek()
    {
        SetTimelineWeek(DateTime.Today);
    }

    private List<Rental> GetVehicleRentalsInRange(int vehicleId, DateTime start, DateTime end)
    {
        if (rentals == null) return new List<Rental>();
        return rentals.Where(r =>
            r.VehicleId == vehicleId &&
            r.StartDate.ToLocalTime().Date <= end.Date &&
            r.EndDate.ToLocalTime().Date >= start.Date
        ).OrderBy(r => r.StartDate).ToList();
    }

    private (double LeftPercent, double WidthPercent) GetTimelinePosition(Rental rental, DateTime rangeStart, DateTime rangeEnd)
    {
        var totalDays = (rangeEnd - rangeStart).TotalDays + 1;
        var rentalStart = rental.StartDate.ToLocalTime().Date;
        var rentalEnd = rental.EndDate.ToLocalTime().Date;

        var visibleStart = rentalStart < rangeStart ? rangeStart : rentalStart;
        var visibleEnd = rentalEnd > rangeEnd ? rangeEnd : rentalEnd;

        var leftDays = (visibleStart - rangeStart).TotalDays;
        var widthDays = (visibleEnd - visibleStart).TotalDays + 1;

        var leftPct = (leftDays / totalDays) * 100;
        var widthPct = (widthDays / totalDays) * 100;

        return (leftPct, widthPct);
    }

    // --- Shared Logic ---

    private string GetRentalStatusClass(Rental rental)
    {
        var now = DateTime.UtcNow;
        if (rental.EndDate < now)
            return "rental-completed";
        if (rental.StartDate <= now && rental.EndDate >= now)
            return "rental-active";
        return "rental-upcoming";
    }

    private string GetDurationText(Rental rental)
    {
        var duration = rental.EndDate - rental.StartDate;
        if (duration.TotalDays >= 1)
        {
            var days = (int)Math.Ceiling(duration.TotalDays);
            return $"{days} day{(days != 1 ? "s" : "")}";
        }
        return $"{(int)duration.TotalHours}h {duration.Minutes}m";
    }

    private void ViewRental(int id)
    {
        selectedRental = rentals?.FirstOrDefault(r => r.Id == id);
    }

    private void CloseRentalDetail()
    {
        selectedRental = null;
    }

    private void NavigateToRental(int id)
    {
        Navigation.NavigateTo($"/rentals/{id}");
    }

    private class CalendarDay
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public List<Rental> Rentals { get; set; } = new();
    }
}

