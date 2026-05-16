using System.Globalization;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FysioEnterprise.Presentation.Components.Pages;

public partial class SessionBookingPage : ComponentBase
{
    [Inject] private ISessionQueries SessionQueries { get; set; } = default!;
    [Inject] private ISimpleQueries SimpleQueries { get; set; } = default!;
    [Inject] private IClientQueries ClientQueries { get; set; } = default!;
    [Inject] private LogInContext Context { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    public static readonly CultureInfo DanishCulture = new("da-DK");
    private string View { get; set; } = "week";
    private DateTime CurrentDate { get; set; } = DateTime.Today;
    private int? SelectedRow { get; set; }
    private DateTime? SelectedCell { get; set; }
    private Guid SelectedSessionTypeId { get; set; }
    private enum CalendarViewMode { Staff, Client }
    private CalendarViewMode CurrentViewMode { get; set; } = CalendarViewMode.Staff;
    private string SearchQuery { get; set; } = string.Empty;

    private List<SessionDTO> _sessions = [];
    private List<ClientDTO> _clients = [];
    private List<StaffDTO> _staff = [];
    private List<SessionTypeDTO> _sessionTypes = [];
    private ClinicDTO? _clinic;

    private static readonly string[] StaffColors =
        ["teal", "coral", "purple", "blue", "amber", "rose", "lime"];

    private static readonly int[] Hours = Enumerable.Range(0, 24).ToArray();

    private List<DateTime> VisibleDays => View switch
    {
        "day" => new List<DateTime> { CurrentDate },
        "week" => Enumerable.Range(0, 7)
                    .Select(i => GetMonday(CurrentDate).AddDays(i))
                    .ToList(),
        _ => new List<DateTime>()
    };

    private List<DateTime> MonthDays
    {
        get
        {
            var first = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            var startDay = first.DayOfWeek == DayOfWeek.Sunday ? first.AddDays(-6) : first.AddDays(-(int)first.DayOfWeek + 1);
            return Enumerable.Range(0, 42).Select(i => startDay.AddDays(i)).ToList();
        }
    }
    private List<SessionDTO> FilteredSessions
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return _sessions;

            return _sessions.Where(s =>
            {
                var fullName = CurrentViewMode == CalendarViewMode.Staff
                    ? $"{s.StaffFirstName} {s.StaffLastname}"
                    : $"{s.ClientFirstName} {s.ClientLastName}";

                return fullName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
            }).ToList();
        }
    }

    private void ToggleViewMode()
    {
        CurrentViewMode = CurrentViewMode == CalendarViewMode.Staff
            ? CalendarViewMode.Client
            : CalendarViewMode.Staff;

        SearchQuery = string.Empty;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Context.LoadFromStorageAsync();

            if (!Context.IsLoggedIn)
            {
                Nav.NavigateTo("/");
                return;
            }

            await LoadData();
            StateHasChanged();
        }
        await JS.InvokeVoidAsync("scrollToHour", 7);
    }

    protected override Task OnInitializedAsync() => Task.CompletedTask;

    private async Task LoadData()
    {
        var sessionsTask = await SessionQueries.GetAllActiveSessionsByClincIdAsync(Context.ClinicId);
        var clientsTask = await ClientQueries.GetAllClientsAsync();
        var staffTask = await SimpleQueries.GetAllStaffByClinicAsync(Context.ClinicId);
        var typesTask = await SimpleQueries.GetAllSessionTypesAsync();
        var clinicTask = await SimpleQueries.GetClinicByIdAsync(Context.ClinicId);

        _sessions = sessionsTask?? [];
        _clients = clientsTask?? [];
        _staff = staffTask?? [];
        _sessionTypes = typesTask ?? [];
        _clinic = clinicTask;
    }
    private List<SessionDTO> GetSessionsForSlot(DateTime day, int hour)
        => _sessions.Where(s =>
            s.timeSlot.From.Date == day.Date &&
            s.timeSlot.From.Hour == hour)
        .ToList();

    private List<SessionDTO> GetSessionsForDay(DateTime day)
     => _sessions.Where(s => s.timeSlot.From.Date == day.Date).ToList();

    private string GetClientName(string clientName)
    {
        var client = _clients.FirstOrDefault(c => c.ClientFirstName + " " + c.ClientLastName == clientName);
        if (client is null)
            throw new ArgumentNullException("Client does not exist in system: ", nameof(client));
        return $"{client.ClientFirstName}  {client.ClientLastName}";
    }

    private string GetStaffAuthorisationType(string staffName)
    {
        var staffType = _staff.Where(s => s.StaffFirstName + " " + s.StaffLastName == staffName).Select(s => s.StaffAuthorisationType).FirstOrDefault();
        if (staffType is null)
            throw new ArgumentNullException("Staff member has no authorisationType: ", nameof(staffType));
        return staffType;
    }

    private string GetSessionTypeName(string sessionType)
    {
        if (string.IsNullOrEmpty(sessionType))
            throw new ArgumentNullException($"Session skal have et navn");
        var sessiontypeName = _sessionTypes.FirstOrDefault(t => t.SessionTypeName == sessionType)?.ToString();
        if (string.IsNullOrEmpty(sessiontypeName))
        {
            return "Andet";
        }
        return sessiontypeName;
    }

    private int GetStaffColorIndex(string staffName)
    {
        var idx = _staff.FindIndex(s => s.StaffFirstName + " " + s.StaffLastName == staffName);
        return idx < 0 ? 0 : idx % StaffColors.Length;
    }

    private string GetDynamicColorClass(SessionDTO session)
    {
        string stableIdentifier = $"{session.StaffFirstName} {session.StaffLastname}";

        if (string.IsNullOrEmpty(stableIdentifier.Trim())) return "staff-color-0";

        int colorIndex = Math.Abs(stableIdentifier.GetHashCode()) % 7;
        return $"staff-color-{colorIndex}";
    }

    private bool IsOutsideOpeningHours(DateTime day, int hour)
    {
        if (_clinic is null) return false;
        var oh = _clinic.ClinicOpeningHours?
            .FirstOrDefault(o => o.Day == day.DayOfWeek);
        if (oh is null || oh.IsClosed) return true;
        return hour < oh.From.Hour || hour >= oh.To.Hour;
    }
    private void SelectRow(int hour)
    {
        SelectedRow = SelectedRow == hour ? null : hour;
        SelectedCell = null;
        StateHasChanged();
    }

    private void OnCellClick(DateTime day, int hour)
    {
        SelectedRow = hour;
        if (SelectedCell.HasValue &&
            SelectedCell.Value.Date == day.Date &&
            SelectedCell.Value.Hour == hour)
        {
            SelectedCell = null;
        }
        StateHasChanged();
    }

    private void OnCellDoubleClick(DateTime day, int hour)
    {
        SelectedCell = new DateTime(day.Year, day.Month, day.Day, hour, 0, 0);
        SelectedSessionTypeId = Guid.Empty;
        StateHasChanged();
    }

    private void OnSessionTypeSelected(ChangeEventArgs e)
    {
        if (Guid.TryParse(e.Value?.ToString(), out var id))
            SelectedSessionTypeId = id;
    }

    private void OnSessionClick(SessionDTO session)
        => Nav.NavigateTo($"/sessions/{session.SessionID}");

    private void GoToCreateSession()
        => Nav.NavigateTo("/createsession");

    private void GoToCreateSessionWithSlot()
    {
        if (SelectedCell is null || SelectedSessionTypeId == Guid.Empty) return;
        var slot = SelectedCell.Value;
        Nav.NavigateTo($"/createsession?date={slot:yyyy-MM-dd}&hour={slot.Hour}&sessionTypeId={SelectedSessionTypeId}&clinicId={Context.ClinicId}&staffId={Context.StaffId}");
    }

    private void DrillToDay(DateTime day)
    {
        CurrentDate = day;
        View = "day";
    }

    private void SetView(string view)
    {
        View = view;
        SelectedRow = null;
        SelectedCell = null;
        StateHasChanged();
    }

    private void NavigateBack() => CurrentDate = View switch
    {
        "month" => CurrentDate.AddMonths(-1),
        "day" => CurrentDate.AddDays(-1),
        _ => CurrentDate.AddDays(-7)
    };

    private void NavigateForward() => CurrentDate = View switch
    {
        "month" => CurrentDate.AddMonths(1),
        "day" => CurrentDate.AddDays(1),
        _ => CurrentDate.AddDays(7)
    };

    private void GoToToday() => CurrentDate = DateTime.Today;

    private string GetRangeLabel() => View switch
    {
        "month" => CurrentDate.ToString("MMMM yyyy", DanishCulture),
        "day" => CurrentDate.ToString("dddd d. MMMM yyyy", DanishCulture),
        _ => $"{GetMonday(CurrentDate):d/M} – {GetMonday(CurrentDate).AddDays(6):d/M, yyyy}"
    };

    private static bool IsToday(DateTime day) => day.Date == DateTime.Today;

    private static DateTime GetMonday(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }




}