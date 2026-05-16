using System.Globalization;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;

namespace FysioEnterprise.Presentation.Components.Pages;

public partial class SessionBookingPage : ComponentBase
{
    [Inject] private ISessionQueries SessionQueries { get; set; } = default!;
    [Inject] private ISimpleQueries SimpleQueries { get; set; } = default!;
    [Inject] private LogInContext Context { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    public static readonly CultureInfo DanishCulture = new("da-DK");

    private string View { get; set; } = "week";
    private DateTime CurrentWeekStart { get; set; } = GetMonday(DateTime.Today);

    private List<SessionDTO> _sessions = [];
    private List<ClientDTO> _clients = [];
    private List<SessionTypeDTO> _sessionTypes = [];

    private List<DateTime> WeekDays => Enumerable.Range(0, 7)
        .Select(i => CurrentWeekStart.AddDays(i))
        .ToList();

    private static readonly int[] Hours = Enumerable.Range(7, 13).ToArray(); // 07:00 - 19:00

    protected override async Task OnInitializedAsync()
    {
        if (Context.IsLoggedIn)
        {
            Nav.NavigateTo("/");
            return;
        }

        await LoadData();
    }

    private async Task LoadData()
    {
        _sessions = await SessionQueries.GetAllActiveSessionsByClincIdAsync(Context.ClinicId);
        var staffList = await SimpleQueries.GetAllStaffByClinicAsync(Context.ClinicId);
        _sessionTypes = await SimpleQueries.GetAllSessionTypesAsync();
    }

    private List<SessionDTO> GetSessionsForSlot(DateTime day, int hour)
        => _sessions.Where(s =>
            s.timeSlot.From.Date == day.Date &&
            s.timeSlot.From.Hour == hour)
        .ToList();

    private string GetSessionColor(SessionDTO session)
    {
        var colors = new[] { "teal", "purple", "coral", "blue", "amber" };
        return colors[Math.Abs(session.StaffFirstName.GetHashCode()) % colors.Length];
    }

    private string GetRangeLabel()
    {
        var end = CurrentWeekStart.AddDays(6);
        return $"{CurrentWeekStart.Day}/{CurrentWeekStart.Month} – {end.Day}/{end.Month}, {end.Year}";
    }

    private void PreviousWeek() => CurrentWeekStart = CurrentWeekStart.AddDays(-7);
    private void NextWeek() => CurrentWeekStart = CurrentWeekStart.AddDays(7);
    private void GoToToday() => CurrentWeekStart = GetMonday(DateTime.Today);
    private bool IsToday(DateTime day) => day.Date == DateTime.Today;

    private static DateTime GetMonday(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }

    private void NewBooking() => Nav.NavigateTo("/bookings/new");
    private void OnCellClick(DateTime day, int hour) { /* open create modal */ }
    private void OnSessionClick(SessionDTO session) { /* open detail modal */ }
}