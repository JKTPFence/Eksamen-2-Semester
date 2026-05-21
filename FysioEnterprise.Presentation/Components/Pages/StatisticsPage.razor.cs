using System.Drawing;
using System.Globalization;
using ClosedXML.Excel;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FysioEnterprise.Presentation.Components.Pages;

public partial class StatisticsPage : ComponentBase
{
    [Inject] private IEarningsReportQuery ReportQuery { get; set; } = default!;
    [Inject] private ISessionQueries SessionQueries { get; set; } = default!;
    [Inject] private LogInContext Context { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    public static readonly CultureInfo DanishCulture = new("da-DK");

    private string View { get; set; } = "week";
    private DateTime CurrentDate { get; set; } = DateTime.Today;
    private bool _loading = true;
    private bool _exporting = false;

    private DateTime ExportFrom { get; set; } = DateTime.Today.AddMonths(-1);
    private DateTime ExportTo { get; set; } = DateTime.Today;

    private double _totalRevenue;
    private int _totalSessions;
    private double _avgRevenue;
    private int _completedCount;
    private int _cancelledCount;
    private int _noShowCount;
    private int _activeCount;

    private record RevenuePoint(string Label, double Value);
    private record StatusPoint(string Label, int Completed, int Cancelled, int NoShow, int Active);

    private List<RevenuePoint> _revenueData = [];
    private List<StatusPoint> _statusData = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Context.LoadFromStorageAsync();
            if (!Context.IsLoggedIn) { Nav.NavigateTo("/"); return; }
            await LoadData();
            StateHasChanged();
        }
    }

    private async Task LoadData()
    {
        _loading = true;
        StateHasChanged();

        try
        {
            var (from, to) = GetCurrentRange();

            var sessions = await SessionQueries.GetAllActiveSessionsByClincIdAsync(Context.ClinicId);
            var inRange = sessions.Where(s =>
                s.timeSlot.From.Date >= from &&
                s.timeSlot.From.Date <= to).ToList();

            _totalSessions = inRange.Count;
            _totalRevenue = inRange.Sum(s => s.SessionTotalPrice.Value);
            _avgRevenue = _totalSessions > 0 ? _totalRevenue / _totalSessions : 0;
            _completedCount = inRange.Count(s => s.SessionStatus == "Completed");
            _cancelledCount = inRange.Count(s => s.SessionStatus == "Cancelled");
            _noShowCount = inRange.Count(s => s.SessionStatus == "NoShow");
            _activeCount = inRange.Count(s => s.SessionStatus == "Active");

            BuildChartData(inRange, from, to);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Statistics load error: {ex.Message}");
        }
        finally
        {
            _loading = false;
        }
    }

    private void BuildChartData(List<SessionDTO> sessions, DateTime from, DateTime to)
    {
        _revenueData = [];
        _statusData = [];

        switch (View)
        {
            case "week":
                for (var d = from; d <= to; d = d.AddDays(1))
                {
                    var day = d;
                    var daySessions = sessions.Where(s => s.timeSlot.From.Date == day.Date).ToList();
                    var label = day.ToString("ddd d/M", DanishCulture);
                    _revenueData.Add(new(label, daySessions.Sum(s => s.SessionTotalPrice.Value)));
                    _statusData.Add(new(label,
                        daySessions.Count(s => s.SessionStatus == "Completed"),
                        daySessions.Count(s => s.SessionStatus == "Cancelled"),
                        daySessions.Count(s => s.SessionStatus == "NoShow"),
                        daySessions.Count(s => s.SessionStatus == "Active")));
                }
                break;

            case "month":
                var weekStart = from;
                var weekNum = 1;
                while (weekStart <= to)
                {
                    var ws = weekStart;
                    var we = ws.AddDays(6) > to ? to : ws.AddDays(6);
                    var weekSessions = sessions.Where(s =>
                        s.timeSlot.From.Date >= ws.Date &&
                        s.timeSlot.From.Date <= we.Date).ToList();
                    var label = $"Uge {weekNum}";
                    _revenueData.Add(new(label, weekSessions.Sum(s => s.SessionTotalPrice.Value)));
                    _statusData.Add(new(label,
                        weekSessions.Count(s => s.SessionStatus == "Completed"),
                        weekSessions.Count(s => s.SessionStatus == "Cancelled"),
                        weekSessions.Count(s => s.SessionStatus == "NoShow"),
                        weekSessions.Count(s => s.SessionStatus == "Active")));
                    weekStart = weekStart.AddDays(7);
                    weekNum++;
                }
                break;

            case "year":
                for (var m = 1; m <= 12; m++)
                {
                    var month = m;
                    var monthSessions = sessions.Where(s =>
                        s.timeSlot.From.Year == CurrentDate.Year &&
                        s.timeSlot.From.Month == month).ToList();
                    var label = new DateTime(CurrentDate.Year, month, 1)
                        .ToString("MMM", DanishCulture);
                    _revenueData.Add(new(label, monthSessions.Sum(s => s.SessionTotalPrice.Value)));
                    _statusData.Add(new(label,
                        monthSessions.Count(s => s.SessionStatus == "Completed"),
                        monthSessions.Count(s => s.SessionStatus == "Cancelled"),
                        monthSessions.Count(s => s.SessionStatus == "NoShow"),
                        monthSessions.Count(s => s.SessionStatus == "Active")));
                }
                break;
        }
    }

    private async Task SetView(string view)
    {
        View = view;
        CurrentDate = DateTime.Today;
        await LoadData();
        StateHasChanged();
    }

    private async Task NavigateBack()
    {
        CurrentDate = View switch
        {
            "year" => CurrentDate.AddYears(-1),
            "month" => CurrentDate.AddMonths(-1),
            _ => CurrentDate.AddDays(-7)
        };
        await LoadData();
        StateHasChanged();
    }

    private async Task NavigateForward()
    {
        CurrentDate = View switch
        {
            "year" => CurrentDate.AddYears(1),
            "month" => CurrentDate.AddMonths(1),
            _ => CurrentDate.AddDays(7)
        };
        await LoadData();
        StateHasChanged();
    }

    private async Task GoToThisYear()
    {
        CurrentDate = DateTime.Today;
        View = "year";
        await LoadData();
        StateHasChanged();
    }
    private (DateTime From, DateTime To) GetCurrentRange() => View switch
    {
        "year" => (new DateTime(CurrentDate.Year, 1, 1), new DateTime(CurrentDate.Year, 12, 31)),
        "month" => (new DateTime(CurrentDate.Year, CurrentDate.Month, 1),
                    new DateTime(CurrentDate.Year, CurrentDate.Month,
                        DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month))),
        _ => (GetMonday(CurrentDate), GetMonday(CurrentDate).AddDays(6))
    };

    private string GetRangeLabel() => View switch
    {
        "year" => CurrentDate.Year.ToString(),
        "month" => CurrentDate.ToString("MMMM yyyy", DanishCulture),
        _ => $"{GetMonday(CurrentDate):d/M} – {GetMonday(CurrentDate).AddDays(6):d/M}, {CurrentDate.Year}"
    };

    private string GetChartSubtitle() => View switch
    {
        "year" => "pr. måned",
        "month" => "pr. uge",
        _ => "pr. dag"
    };

    private static DateTime GetMonday(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }

    private async Task ExportToExcel()
    {
        _exporting = true;
        StateHasChanged();

        try
        {
            var sessions = await SessionQueries.GetAllActiveSessionsByClincIdAsync(Context.ClinicId);
            var inRange = sessions.Where(s =>
                s.timeSlot.From.Date >= ExportFrom.Date &&
                s.timeSlot.From.Date <= ExportTo.Date).ToList();

            using var wb = new XLWorkbook();
            var revenueSheet = wb.Worksheets.Add("Omsætning");
            var statusSheet = wb.Worksheets.Add("Session Status");

            revenueSheet.Cell("A1").Value = "FysioFunc — Omsætningsrapport";
            revenueSheet.Cell("A1").Style.Font.Bold = true;
            revenueSheet.Cell("A1").Style.Font.FontSize = 14;

            revenueSheet.Cell("A2").Value = $"Periode: {ExportFrom:dd-MM-yyyy} til {ExportTo:dd-MM-yyyy}";
            revenueSheet.Cell("A2").Style.Font.Italic = true;

            revenueSheet.Cell("A4").Value = "Dato";
            revenueSheet.Cell("B4").Value = "Klient";
            revenueSheet.Cell("C4").Value = "Behandler";
            revenueSheet.Cell("D4").Value = "Sessiontype";
            revenueSheet.Cell("E4").Value = "Status";
            revenueSheet.Cell("F4").Value = "Beløb (kr.)";

            var headerRange = revenueSheet.Range("A4:F4");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#2d6a4f");
            headerRange.Style.Font.FontColor = XLColor.White;

            var row = 5;
            foreach (var s in inRange.OrderBy(s => s.timeSlot.From))
            {
                revenueSheet.Cell(row, 1).Value = s.timeSlot.From.ToString("dd-MM-yyyy HH:mm");
                revenueSheet.Cell(row, 2).Value = $"{s.ClientFirstName} {s.ClientLastName}";
                revenueSheet.Cell(row, 3).Value = $"{s.StaffFirstName} {s.StaffLastname}";
                revenueSheet.Cell(row, 4).Value = s.SessionTypeName;
                revenueSheet.Cell(row, 5).Value = s.SessionStatus;
                revenueSheet.Cell(row, 6).Value = s.SessionTotalPrice.Value;
                revenueSheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

                var rowColor = s.SessionStatus switch
                {
                    "Completed" => XLColor.FromHtml("#e8f5e9"),
                    "Cancelled" => XLColor.FromHtml("#fce4ec"),
                    "NoShow" => XLColor.FromHtml("#fff3e0"),
                    _ => XLColor.White
                };
                revenueSheet.Range(row, 1, row, 6).Style.Fill.BackgroundColor = rowColor;
                row++;
            }

            revenueSheet.Cell(row, 5).Value = "Total";
            revenueSheet.Cell(row, 5).Style.Font.Bold = true;
            revenueSheet.Cell(row, 6).FormulaA1 = $"=SUM(F5:F{row - 1})";
            revenueSheet.Cell(row, 6).Style.Font.Bold = true;
            revenueSheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

            revenueSheet.Columns().AdjustToContents();

            statusSheet.Cell("A1").Value = "FysioFunc — Session Status Rapport";
            statusSheet.Cell("A1").Style.Font.Bold = true;
            statusSheet.Cell("A1").Style.Font.FontSize = 14;

            statusSheet.Cell("A2").Value = $"Periode: {ExportFrom:dd-MM-yyyy} til {ExportTo:dd-MM-yyyy}";
            statusSheet.Cell("A2").Style.Font.Italic = true;

            statusSheet.Cell("A4").Value = "Status";
            statusSheet.Cell("B4").Value = "Antal";
            statusSheet.Cell("C4").Value = "Andel (%)";

            var statusHeader = statusSheet.Range("A4:C4");
            statusHeader.Style.Font.Bold = true;
            statusHeader.Style.Fill.BackgroundColor = XLColor.FromHtml("#2d6a4f");
            statusHeader.Style.Font.FontColor = XLColor.White;

            var statuses = new[] { "Completed", "Cancelled", "NoShow", "Active" };
            var labels = new[] { "Fuldført", "Annulleret", "No-show", "Aktiv" };
            var sRow = 5;
            foreach (var (status, label) in statuses.Zip(labels))
            {
                var count = inRange.Count(s => s.SessionStatus == status);
                statusSheet.Cell(sRow, 1).Value = label;
                statusSheet.Cell(sRow, 2).Value = count;
                statusSheet.Cell(sRow, 3).FormulaA1 = inRange.Count > 0
                    ? $"=B{sRow}/{inRange.Count}"
                    : "=0";
                statusSheet.Cell(sRow, 3).Style.NumberFormat.Format = "0.0%";
                sRow++;
            }

            statusSheet.Cell(sRow, 1).Value = "Total";
            statusSheet.Cell(sRow, 1).Style.Font.Bold = true;
            statusSheet.Cell(sRow, 2).FormulaA1 = $"=SUM(B5:B{sRow - 1})";
            statusSheet.Cell(sRow, 2).Style.Font.Bold = true;
            statusSheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var bytes = stream.ToArray();
            var base64 = Convert.ToBase64String(bytes);
            var filename = $"FysioFunc_Rapport_{ExportFrom:yyyyMMdd}_{ExportTo:yyyyMMdd}.xlsx";

            await JS.InvokeVoidAsync("downloadBase64File", filename, base64);
        }
        finally
        {
            _exporting = false;
            StateHasChanged();
        }
    }
}
