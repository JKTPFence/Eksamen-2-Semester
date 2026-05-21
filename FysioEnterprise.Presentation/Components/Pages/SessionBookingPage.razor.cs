using System.Globalization;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.UseCase.SessionUseCase;
using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Presentation.Components.Pages;

public partial class SessionBookingPage : ComponentBase
{
    [Inject] private ISessionQueries SessionQueries { get; set; } = default!;
    [Inject] private ISimpleQueries SimpleQueries { get; set; } = default!;
    [Inject] private IClientQueries ClientQueries { get; set; } = default!;
    [Inject] private LogInContext Context { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IEndSessionUseCase EndSessionUseCase { get; set; } = default!;
    [Inject] private ICancelSessionUseCase CancelSessionUseCase { get; set; } = default!;
    [Inject] private IMarkSessionAsNoShowUseCase MarkSessionAsNoShowUseCase { get; set; } = default!;

    public static readonly CultureInfo DanishCulture = new("da-DK");
    private string View { get; set; } = "week";
    private DateTime CurrentDate { get; set; } = DateTime.Today;
    private int? SelectedRow { get; set; }
    private DateTime? SelectedCell { get; set; }
    private Guid SelectedSessionTypeId { get; set; }
    private enum CalendarViewMode { Staff, Client }
    private CalendarViewMode CurrentViewMode { get; set; } = CalendarViewMode.Staff;
    private bool HideWeekends { get; set; } = false;

    private List<SessionDTO> _sessions = [];
    private List<ClientDTO> _clients = [];
    private List<StaffDTO> _staff = [];
    private List<SessionTypeDTO> _sessionTypes = [];
    private ClinicDTO? _clinic;
    private const int SlotHeight = 30;
    private int SessionTimeInMinutes;
    private string clinicName = string.Empty;
    private bool HideCancelled { get; set; } = false;
    private record SessionLayout(double Top, double Height, double Left, double Width);

    private double TimeToPixels(DateTime time)
    => (time.Hour * 60 + time.Minute) * SlotHeight / 15.0;

    private static readonly string[] StaffColors =
        ["teal", "coral", "purple", "blue", "amber", "rose", "lime"];

    private static readonly int[] Hours = Enumerable.Range(0, 24).ToArray();

    private string _searchQuery = string.Empty;
    private string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            StateHasChanged();
        }
    }

    private List<DateTime> VisibleDays => View switch
    {
        "day" => new List<DateTime> { CurrentDate },
        "week" => Enumerable.Range(0, 7)
                    .Select(i => GetMonday(CurrentDate).AddDays(i))
                    .Where(d => !HideWeekends || (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday))
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
    private void ToggleCancelledSessions()
    {
        HideCancelled = !HideCancelled;
    }
    private List<SessionDTO> FilteredSessions => string.IsNullOrWhiteSpace(SearchQuery)
    ? _sessions
    : CurrentViewMode == CalendarViewMode.Staff
        ? _sessions.Where(s =>
            $"{s.StaffFirstName} {s.StaffLastname}"
                .Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
            .ToList()
        : _sessions.Where(s =>
            $"{s.ClientFirstName} {s.ClientLastName}"
                .Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();

    private void ToggleViewMode()
    {
        CurrentViewMode = CurrentViewMode == CalendarViewMode.Staff
            ? CalendarViewMode.Client
            : CalendarViewMode.Staff;

        SearchQuery = string.Empty;
    }

    private int GetSessionTypeInMinutes(SessionTypeDTO sessionType)
    {
        if (sessionType.SessionTypeTimeSpan.Hour > 0)
        {
            var hourCount = sessionType.SessionTypeTimeSpan.Hour;
            SessionTimeInMinutes = hourCount * 60;
        }
        var totalMinuteCount = sessionType.SessionTypeTimeSpan.Minute + SessionTimeInMinutes;
        SessionTimeInMinutes = 0;
        return totalMinuteCount;
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

        _sessions = sessionsTask.OrderBy(s => s.StaffFirstName).ToList();
        _clients = clientsTask.OrderBy(c => c.ClientFirstName).ToList();
        _staff = staffTask.OrderBy(s => s.StaffFirstName).ToList();
        _sessionTypes = typesTask.OrderBy(s => s.SessionTypeName).ToList();
        _clinic = clinicTask;

        if (!string.IsNullOrEmpty(_clinic?.ClinicAddress))
        {
            var clinicsplit = _clinic.ClinicAddress.Split(',');

            if (clinicsplit.Length >= 2)
            {
                clinicName = $"{clinicsplit[0].Trim()}, {clinicsplit[1].Trim()}";
            }
            else
            {
                clinicName = $"{clinicsplit[0].Trim()}";
            }
        }
        else
        {
            clinicName = "Klinik";
        }
    }

    private List<SessionDTO> GetSessionsForDay(DateTime day)
     => FilteredSessions.Where(s => s.timeSlot.From.Date == day.Date).ToList();

    private string GetStaffAuthorisationType(Guid staffID)
    {
        var staffType = _staff.Where(s => s.StaffID == staffID).Select(s => s.StaffAuthorisationType).FirstOrDefault();
        if (staffType is null)
            throw new ArgumentNullException("Staff member has no authorisationType: ", nameof(staffType));
        return staffType;
    }

    private string GetDynamicColorClass(SessionDTO session)
    {
        string stableIdentifier = $"{session.StaffFirstName} {session.StaffLastname}";

        if (string.IsNullOrWhiteSpace(stableIdentifier)) return "staff-color-0";

        int hash = 5381;
        foreach (char c in stableIdentifier)
        {
            hash = ((hash << 5) + hash) + c;
        }

        int colorIndex = Math.Abs(hash) % 7;
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

    private void OnSessionTypeSelected(ChangeEventArgs e)
    {
        if (Guid.TryParse(e.Value?.ToString(), out var id))
            SelectedSessionTypeId = id;
    }

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
        "week" => GetWeekRangeLabel(),
        _ => $"{GetMonday(CurrentDate):d/M} – {GetMonday(CurrentDate).AddDays(6):d/M, yyyy}"
    };

    private string GetWeekRangeLabel()
    {
        var monday = GetMonday(CurrentDate);
        var sunday = monday.AddDays(6);
        string monthName = monday.ToString("MMMM", DanishCulture);

        if (!string.IsNullOrEmpty(monthName))
        {
            monthName = char.ToUpper(monthName[0]) + monthName.Substring(1);
        }

        if (monday.Month != sunday.Month)
        {
            string endMonthName = sunday.ToString("MMMM", DanishCulture);
            if (!string.IsNullOrEmpty(endMonthName))
            {
                endMonthName = char.ToUpper(endMonthName[0]) + endMonthName.Substring(1);
            }

            return $"{monthName} {monday.Day} – {endMonthName} {sunday.Day}, {sunday.Year}";
        }
        return $"{monthName} , {monday.Day} - {sunday.Day} , {monday.Year}";
    }

    private static bool IsToday(DateTime day) => day.Date == DateTime.Today;

    private static DateTime GetMonday(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }

    private List<(SessionDTO Session, int Col, int TotalCols)> CalculateColumns(
    List<SessionDTO> sessions)
    {
        var result = new List<(SessionDTO Session, int Col, int TotalCols)>();
        if (!sessions.Any()) return result;

        var sorted = sessions.OrderBy(s => s.timeSlot.From).ToList();
        var columns = new List<List<SessionDTO>>();

        foreach (var session in sorted)
        {
            var placed = false;
            for (var col = 0; col < columns.Count; col++)
            {
                var overlaps = columns[col].Any(existing =>
                    session.timeSlot.From < existing.timeSlot.To &&
                    existing.timeSlot.From < session.timeSlot.To);

                if (!overlaps)
                {
                    columns[col].Add(session);
                    result.Add((session, col, 0));
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                columns.Add(new List<SessionDTO> { session });
                result.Add((session, columns.Count - 1, 0));
            }
        }
        var final = new List<(SessionDTO Session, int Col, int TotalCols)>();
        foreach (var (session, col, _) in result)
        {
            var overlapGroup = result
                .Where(r =>
                    session.timeSlot.From < r.Session.timeSlot.To &&
                    r.Session.timeSlot.From < session.timeSlot.To)
                .ToList();

            var totalCols = overlapGroup.Max(r => r.Col) + 1;
            final.Add((session, col, totalCols));
        }

        foreach (var (s, col, total) in final)
            Console.WriteLine($"{s.timeSlot.From:HH:mm}-{s.timeSlot.To:HH:mm} → col {col}/{total}");

        return final;
    }

    private SessionLayout GetSessionLayout(
    SessionDTO session,
    List<(SessionDTO Session, int Col, int TotalCols)> columns)
    {
        var match = columns.FirstOrDefault(c => c.Session == session);
        var top = TimeToPixels(session.timeSlot.From);
        var bottom = TimeToPixels(session.timeSlot.To);

        var height = Math.Max(bottom - top, SlotHeight);

        var totalCols = match.TotalCols > 0 ? match.TotalCols : 1;
        var col = match.Col;
        var colWidth = 100.0 / totalCols;
        var left = col * colWidth;

        return new SessionLayout(top, height - 2, left + 0.3, colWidth - 0.8);
    }

    private void OnColumnDoubleClick(DateTime day, int hour, int minutes)
    {
        SelectedCell = new DateTime(day.Year, day.Month, day.Day, hour, minutes, 0);
        SelectedSessionTypeId = Guid.Empty;
        StateHasChanged();
    }

    private void ToggleWeekends()
    {
        HideWeekends = !HideWeekends;
    }

    private SessionDTO? SelectedViewSession { get; set; }

    private void OnSessionClick(SessionDTO session)
    {
        SelectedViewSession = session;
    }

    private void CloseSessionDetails()
    {
        SelectedViewSession = null;

    }

    private async void HandleUpdate()
    {
       if (SelectedViewSession == null) return;
        Nav.NavigateTo($"/createsession?sessionId={SelectedViewSession.SessionID}");
    }

    private async Task HandleComplete()
    {
        if (SelectedViewSession == null) return;
        var result = await EndSessionUseCase.EndSessionAsync(new EndSessionRequest(SelectedViewSession.SessionID));
        if (result.Errors == null || result.Errors.Count == 0)
        {
            var masterSessionItem = _sessions.FirstOrDefault(s => s.SessionID == SelectedViewSession.SessionID);

            if (masterSessionItem != null)
            {
                var index = _sessions.IndexOf(masterSessionItem);

                _sessions[index] = masterSessionItem with { SessionStatus = "Completed" };
            }

            var dayofsession = SelectedViewSession.timeSlot.From.Date;
            var sessionsfortheday = GetSessionsForDay(dayofsession);
            CalculateColumns(sessionsfortheday);
            CloseSessionDetails();
            StateHasChanged();
        }
        else
        {
           
        }
        StateHasChanged();
    }

    private async Task HandleNoShow()
    {
        if (SelectedViewSession == null) return;
        var result = await MarkSessionAsNoShowUseCase.MarkSessionAsNoShowAsync(new MarkNoShowSessionRequest(SelectedViewSession.SessionID));
        if (result.Errors == null || result.Errors.Count == 0)
        {
            var masterSessionItem = _sessions.FirstOrDefault(s => s.SessionID == SelectedViewSession.SessionID);

            if (masterSessionItem != null)
            {
                var index = _sessions.IndexOf(masterSessionItem);

                _sessions[index] = masterSessionItem with { SessionStatus = "NoShow" };
            }

            var dayofsession = SelectedViewSession.timeSlot.From.Date;
            var sessionsfortheday = GetSessionsForDay(dayofsession);
            CalculateColumns(sessionsfortheday);
            CloseSessionDetails();
            StateHasChanged();
        }
        else
        {

        }
        StateHasChanged();
    }

    private async Task HandleCancel()
    {
        if (SelectedViewSession == null) return;
        var result = await CancelSessionUseCase.CancelSessionAsync(new CancelSessionRequest(SelectedViewSession.SessionID));
        if (result.Errors == null || result.Errors.Count == 0)
        {
            var masterSessionItem = _sessions.FirstOrDefault(s => s.SessionID == SelectedViewSession.SessionID);

            if (masterSessionItem != null)
            {
                var index = _sessions.IndexOf(masterSessionItem);

                _sessions[index] = masterSessionItem with { SessionStatus = "Cancelled" };
            }

            var dayofsession = SelectedViewSession.timeSlot.From.Date;
            var sessionsfortheday = GetSessionsForDay(dayofsession);
            CalculateColumns(sessionsfortheday);
            CloseSessionDetails();
            StateHasChanged();
        }
        else
        {

        }
        StateHasChanged();
    }
    private void ClearPopupSelection()
    {
        SelectedCell = null;
        SelectedSessionTypeId = Guid.Empty;
    }

}