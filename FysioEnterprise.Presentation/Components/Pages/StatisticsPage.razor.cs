using System.Globalization;
using ClosedXML.Excel;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.RequestModels;
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

    private DateTime ExportFrom { get; set; } = DateTime.Today;
    private DateTime ExportTo { get; set; } = DateTime.Today.AddMonths(1);

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
            UpdateExportRangeToCurrentView();
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
        UpdateExportRangeToCurrentView();
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
        UpdateExportRangeToCurrentView();
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
        _ => GetWeekRangeLabel()
    };

    private string GetWeekRangeLabel() //Formats the week range as "1-7 Jan, 2024" or "28 Jan - 3 Feb, 2024" if it spans two months
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

            return $"{monday.Day} , {monthName} – {sunday.Day} , {endMonthName} , {sunday.Year}";
        }
        return $"{monday.Day} - {sunday.Day} , {monthName} , {monday.Year}";
    }
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
            //using the DTO directly from the report query, to ensure we get the same data as shown in the summary, and not just the raw session data which would require us to re-apply all discounts and calculations here
            var report = await ReportQuery.GetEarningsReportAsync(
            new EarningsReportRequestDTO(ExportFrom, ExportTo));

            using var wb = new XLWorkbook();

            //Session box, with calculated revenue
            var revenueSheet = wb.Worksheets.Add("Omsætning");

            revenueSheet.Cell("A1").Value = $"{Context.ClinicName} — Omsætningsrapport";
            revenueSheet.Cell("A1").Style.Font.Bold = true;
            revenueSheet.Cell("A1").Style.Font.FontSize = 14;
            revenueSheet.Cell("A2").Value = $"Periode: {ExportFrom:dd-MM-yyyy} til {ExportTo:dd-MM-yyyy}";
            revenueSheet.Cell("A2").Style.Font.Italic = true;
            revenueSheet.Cell("A3").Value = $"Genereret: {DateTime.Now:dd-MM-yyyy HH:mm}";
            revenueSheet.Cell("A3").Style.Font.Italic = true;

            //Summary box, for easier view of key metrics
            revenueSheet.Cell("A5").Value = "Total omsætning (kr.)";
            revenueSheet.Cell("B5").Value = report.TotalRevenue;

            revenueSheet.Cell("A6").Value = "Antal fuldførte bookinger";
            revenueSheet.Cell("B6").Value = report.SessionCount;

            revenueSheet.Cell("A7").Value = "Gennemsnit pr. booking (kr.)";
            revenueSheet.Cell("B7").Value = report.AverageRevenue;

            revenueSheet.Cell("A8").Value = "Samlet rabat givet (kr.)";
            revenueSheet.Cell("B8").Value = report.Discounts
                .Where(d => d.SessionStatus is SessionStatusEnum.Completed or SessionStatusEnum.Active)
                .Sum(d => d.DiscountAmount);

            revenueSheet.Cell("A9").Value = "Mistede penge (kr.)";
            revenueSheet.Cell("B9").Value = report.Discounts.Sum(d => d.LostRevenue);

            revenueSheet.Cell("A10").Value = "Kommende omsætning (kr.)";
            revenueSheet.Cell("B10").Value = report.Discounts.Sum(d => d.UpcomingPrice);

            var summaryRange = revenueSheet.Range("A5:B10");
            summaryRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#f0f7f4");
            summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            revenueSheet.Range("A5:A10").Style.Font.Bold = true;


            revenueSheet.Cell("A12").Value = "Dato";
            revenueSheet.Cell("B12").Value = "Klient";
            revenueSheet.Cell("C12").Value = "Behandler";
            revenueSheet.Cell("D12").Value = "Bookingtype";
            revenueSheet.Cell("E12").Value = "Status";
            revenueSheet.Cell("F12").Value = "Originalpris (kr.)";
            revenueSheet.Cell("G12").Value = "Rabat (kr.)";
            revenueSheet.Cell("H12").Value = "Aflyste bookinger (kr.)";
            revenueSheet.Cell("I12").Value = "Kommende bookinger (kr.)";
            revenueSheet.Cell("J12").Value = "Nettoresultat (kr.)";

            var revHeader = revenueSheet.Range("A12:J12");
            revHeader.Style.Font.Bold = true;
            revHeader.Style.Fill.BackgroundColor = XLColor.FromHtml("#2d6a4f");
            revHeader.Style.Font.FontColor = XLColor.White;
            revHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var row = 13;
            foreach (var d in report.Discounts.OrderBy(d => d.SessionStart))
            {
                revenueSheet.Cell(row, 1).Value = d.SessionStart.ToString("dd-MM-yyyy HH:mm");
                revenueSheet.Cell(row, 2).Value = d.ClientName;
                revenueSheet.Cell(row, 3).Value = d.StaffName;
                revenueSheet.Cell(row, 4).Value = d.SessiontypeName;
                revenueSheet.Cell(row, 5).Value = d.SessionStatus.ToString();
                revenueSheet.Cell(row, 6).Value = d.OriginalPrice;
                revenueSheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

                if (d.SessionStatus is SessionStatusEnum.Completed or SessionStatusEnum.Active)
                {
                    revenueSheet.Cell(row, 7).Value = d.DiscountAmount;
                    revenueSheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                }
                else
                {
                    revenueSheet.Cell(row, 7).Value = "Ingen Rabat";
                    revenueSheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                if (d.SessionStatus is SessionStatusEnum.Cancelled or SessionStatusEnum.NoShow)
                {
                    revenueSheet.Cell(row, 8).Value = d.LostRevenue;
                    revenueSheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";
                }

                if (d.SessionStatus == SessionStatusEnum.Active)
                {
                    revenueSheet.Cell(row, 9).Value = d.UpcomingPrice;
                    revenueSheet.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";
                }

                if (d.SessionStatus == SessionStatusEnum.Completed)
                {
                    revenueSheet.Cell(row, 10).Value = d.FinalPrice;
                    revenueSheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0.00";
                }
                else
                {
                    revenueSheet.Cell(row, 10).Value = $"{d.SessionStatus}";
                    revenueSheet.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                revenueSheet.Cell(row, 6).Value = d.OriginalPrice;
                revenueSheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

                var rowColor = d.SessionStatus switch
                {
                    SessionStatusEnum.Completed => XLColor.FromHtml("#e8f5e9"),
                    SessionStatusEnum.Cancelled => XLColor.FromHtml("#fce4ec"),
                    SessionStatusEnum.NoShow => XLColor.FromHtml("#fff3e0"),
                    SessionStatusEnum.Active => XLColor.FromHtml("#e3f2fd"),
                    _ => XLColor.White
                };
                revenueSheet.Range(row, 1, row, 10).Style.Fill.BackgroundColor = rowColor;
                row++;
            }

            if (row > 13)
            {
                revenueSheet.Cell(row, 5).Value = "Total";
                revenueSheet.Cell(row, 5).Style.Font.Bold = true;
                revenueSheet.Cell(row, 6).FormulaA1 = $"=SUM(F13:F{row - 1})";
                revenueSheet.Cell(row, 7).FormulaA1 = $"=SUM(G13:G{row - 1})";
                revenueSheet.Cell(row, 8).FormulaA1 = $"=SUM(H13:H{row - 1})";
                revenueSheet.Cell(row, 9).FormulaA1 = $"=SUM(I13:I{row - 1})";
                revenueSheet.Cell(row, 10).FormulaA1 = $"=SUM(J13:J{row - 1})";
                revenueSheet.Range(row, 5, row, 10).Style.Font.Bold = true;
                revenueSheet.Range(row, 6, row, 10).Style.NumberFormat.Format = "#,##0.00";
                revenueSheet.Range(row, 1, row, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#e8ebe4");
            }


            revenueSheet.Columns().AdjustToContents();

            //Session status sheet
            var statusSheet = wb.Worksheets.Add("Booking Status");

            statusSheet.Cell("A1").Value = $"{Context.ClinicName} — Booking Status Rapport";
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
            var colors = new[] { "#e8f5e9", "#fce4ec", "#fff3e0", "#e3f2fd" };

            var sRow = 5;
            foreach (var (status, label, color) in statuses.Zip(labels).Zip(colors,
            (a, c) => (a.First, a.Second, c)))
            {
                var count = report.Discounts.Count(s => s.SessionStatus.ToString() == status);
                statusSheet.Cell(sRow, 1).Value = label;
                statusSheet.Cell(sRow, 2).Value = count;
                statusSheet.Cell(sRow, 3).FormulaA1 = report.Discounts.Count > 0
                    ? $"=B{sRow}/{report.Discounts.Count}"
                    : "=0";
                statusSheet.Cell(sRow, 3).Style.NumberFormat.Format = "0.0%";
                statusSheet.Range(sRow, 1, sRow, 3).Style.Fill.BackgroundColor =
                XLColor.FromHtml(color);
                sRow++;
            }

            statusSheet.Cell(sRow, 1).Value = "Total";
            statusSheet.Cell(sRow, 1).Style.Font.Bold = true;
            statusSheet.Cell(sRow, 2).FormulaA1 = $"=SUM(B5:B{sRow - 1})";
            statusSheet.Cell(sRow, 2).Style.Font.Bold = true;
            statusSheet.Range(sRow, 1, sRow, 3).Style.Fill.BackgroundColor =
            XLColor.FromHtml("#e8ebe4");
            statusSheet.Columns().AdjustToContents();

            //Discount sheet, to analyze the impact of discounts and see which sessions had them applied
            var discountSheet = wb.Worksheets.Add("Rabatter");

            discountSheet.Cell("A1").Value = $"{Context.ClinicName} — Rabatrapport";
            discountSheet.Cell("A1").Style.Font.Bold = true;
            discountSheet.Cell("A1").Style.Font.FontSize = 14;
            discountSheet.Cell("A2").Value = $"Periode: {ExportFrom:dd-MM-yyyy} til {ExportTo:dd-MM-yyyy}";
            discountSheet.Cell("A2").Style.Font.Italic = true;

            discountSheet.Cell("A4").Value = "Bookinger med rabat";
            discountSheet.Cell("B4").Value = report.Discounts.Count(d => d.DiscountAmount > 0);
            discountSheet.Cell("A5").Value = "Bookinger uden rabat";
            discountSheet.Cell("B5").Value = report.Discounts.Count(d => d.DiscountAmount <= 0);
            discountSheet.Cell("A6").Value = "Samlet rabat givet (kr.)";
            discountSheet.Cell("B6").Value = report.Discounts.Sum(d => d.DiscountAmount);
            discountSheet.Cell("B6").Style.NumberFormat.Format = "#,##0.00";

            var discSummary = discountSheet.Range("A4:B6");
            discSummary.Style.Fill.BackgroundColor = XLColor.FromHtml("#f0f7f4");
            discSummary.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            discountSheet.Range("A4:A6").Style.Font.Bold = true;

            discountSheet.Cell("A8").Value = "Bookingtype";
            discountSheet.Cell("B8").Value = "Rabat grundlag";
            discountSheet.Cell("C8").Value = "Originalpris (kr.)";
            discountSheet.Cell("D8").Value = "Rabat (kr.)";
            discountSheet.Cell("E8").Value = "Slutpris (kr.)";

            var discHeader = discountSheet.Range("A8:E8");
            discHeader.Style.Font.Bold = true;
            discHeader.Style.Fill.BackgroundColor = XLColor.FromHtml("#2d6a4f");
            discHeader.Style.Font.FontColor = XLColor.White;
            discHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var dRow = 9;
            foreach (var d in report.Discounts.OrderByDescending(d => d.DiscountAmount))
            {
                discountSheet.Cell(dRow, 1).Value = d.SessiontypeName;
                discountSheet.Cell(dRow, 2).Value = d.StrategyName;
                discountSheet.Cell(dRow, 3).Value = d.OriginalPrice;
                discountSheet.Cell(dRow, 4).Value = d.DiscountAmount;
                discountSheet.Cell(dRow, 5).Value = d.FinalPrice;

                discountSheet.Cell(dRow, 3).Style.NumberFormat.Format = "#,##0.00";
                discountSheet.Cell(dRow, 4).Style.NumberFormat.Format = "#,##0.00";
                discountSheet.Cell(dRow, 5).Style.NumberFormat.Format = "#,##0.00";

                discountSheet.Range(dRow, 1, dRow, 5).Style.Fill.BackgroundColor = d.DiscountAmount > 0
                    ? XLColor.FromHtml("#e8f5e9")
                    : XLColor.White;

                dRow++;
            }

            // Total prices at the bottom of the discount sheet, to see the overall impact of discounts on the revenue
            if (dRow > 9)
            {
                discountSheet.Cell(dRow, 2).Value = "Total";
                discountSheet.Cell(dRow, 2).Style.Font.Bold = true;
                discountSheet.Cell(dRow, 3).FormulaA1 = $"=SUM(C9:C{dRow - 1})";
                discountSheet.Cell(dRow, 4).FormulaA1 = $"=SUM(D9:D{dRow - 1})";
                discountSheet.Cell(dRow, 5).FormulaA1 = $"=SUM(E9:E{dRow - 1})";
                discountSheet.Range(dRow, 2, dRow, 5).Style.Font.Bold = true;
                discountSheet.Range(dRow, 3, dRow, 5).Style.NumberFormat.Format = "#,##0.00";
                discountSheet.Range(dRow, 1, dRow, 5).Style.Fill.BackgroundColor =
                    XLColor.FromHtml("#e8ebe4");
            }

            discountSheet.Columns().AdjustToContents();

            //Stream the Excel file to the client for download
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var bytes = stream.ToArray();
            var base64 = Convert.ToBase64String(bytes);
            var filename = $"FysioFunc_Rapport_{ExportFrom:yyyyMMdd}_{ExportTo:yyyyMMdd}.xlsx";

            await JS.InvokeVoidAsync("downloadBase64File", filename, base64, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        finally
        {
            _exporting = false;
            StateHasChanged();
        }

    }

    private void UpdateExportRangeToCurrentView()
    {
        var range = GetCurrentRange();
        ExportFrom = range.From.Date;

        ExportTo = range.To.Date.AddDays(1).AddTicks(-1);
    }
}
