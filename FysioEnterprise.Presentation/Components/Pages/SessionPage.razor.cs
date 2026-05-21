using Microsoft.AspNetCore.Components;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.UseCase.SessionUseCase;
using FysioEnterprise.Presentation.Service;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;
using Radzen;

namespace FysioEnterprise.Presentation.Components.Pages
{
    public partial class SessionPage : ComponentBase
    {
        [Inject] private ICreateSessionUseCase CreateSession { get; set; } = default!;
        [Inject] private ISessionQueries SessionQueries { get; set; } = default!;
        [Inject] private ISimpleQueries SimpleQueries { get; set; } = default!;
        [Inject] private IClientQueries ClientQueries { get; set; } = default!;
        [Inject] private IPromotionQueries PromotionQueries { get; set; } = default!;
        [Inject] private LogInContext Context { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        //URL parametre fra kalender
        [SupplyParameterFromQuery] private string? Date {  get; set; }
        [SupplyParameterFromQuery] private int? Hour { get; set; }
        [SupplyParameterFromQuery] private Guid? SessionTypeId { get; set; }
        [SupplyParameterFromQuery] private Guid? ClinicId { get; set; }
        [SupplyParameterFromQuery] private Guid? SessionId { get; set; }

        //Data lister
        private List<ClientDTO> _clients = [];
        private List<StaffDTO> _staffInClinic = [];
        private List<SessionTypeDTO> _sessionTypes = [];
        private List<RoomDTO> _rooms = [];
        private List<PromotionDTO> _promotions = [];

        //Selected værdier
        private Guid _selectedClientId;
        private Guid _selectedClinicId;
        private Guid _SelectedStaffId;
        private Guid _selectedSessionTypeId;
        private Guid _selectedRoomid;
        private Guid _selectedPromotionId;
        private DateTime? _startTime;
        private DateTime? _endTime;

        private bool _isEditMode = false;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;

        private int SessionTimeInMinutes;

        private bool CanSubmit =>
            _selectedClientId != Guid.Empty &&
            _selectedClinicId != Guid.Empty &&
            _SelectedStaffId != Guid.Empty &&
            _selectedSessionTypeId != Guid.Empty &&
            _selectedRoomid != Guid.Empty &&
            _startTime.HasValue &&
            _endTime.HasValue &&
            _startTime < _endTime;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
                ApplyUrlParameters();
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            _clients = (await ClientQueries.GetAllClientsAsync())
                .OrderBy(c => c.ClientFirstName).ToList();

            _sessionTypes = (await SimpleQueries.GetAllSessionTypesAsync())
                .OrderBy(s => s.SessionTypeName).ToList();

            _promotions = (await PromotionQueries.GetAllActivePromotionsAsync())
                .OrderBy(p => p.PromotionName).ToList();

            _selectedClinicId = Context.ClinicId;
            await LoadStaffAndRooms();
        }

        private async Task LoadStaffAndRooms()
        {
            if (_selectedClinicId == Guid.Empty) return;

            var staffResult = await SimpleQueries.GetAllStaffByClinicAsync(_selectedClinicId);
            _staffInClinic = staffResult
                .Where(s => s.StaffAuthorisationType != "Receptionist")
                .OrderBy(s => s.StaffFirstName).ToList();

            _rooms = (await SimpleQueries.GetRoomsByClinicIdAsync(_selectedClinicId))
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        private void ApplyUrlParameters()
        {
            if (DateTime.TryParse(Date, out var date) && Hour.HasValue)
            {
                _startTime = date.AddHours(Hour.Value);
            }

            if (SessionTypeId.HasValue && SessionTypeId != Guid.Empty)
                _selectedSessionTypeId = SessionTypeId.Value;

            if (SessionId.HasValue && SessionId != Guid.Empty)
                _isEditMode = true;
        }

        private void OnClientChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id))
                _selectedClientId = id;
        }

        private void OnStaffChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id))
                _SelectedStaffId = id;
        }

        private void OnSessionTypeChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id))
            {
                _selectedSessionTypeId = id;
                var sessionType = _sessionTypes.FirstOrDefault(s => s.SessionTypeID == id);
                if (sessionType != null && _startTime.HasValue)
                {
                    _endTime = _startTime.Value.Add(sessionType.SessionTypeTimeSpan.ToTimeSpan());
                    StateHasChanged();
                }
            }
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

        private void OnRoomChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id))
                _selectedRoomid = id;
        }

        private void OnPromotionChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id))
                _selectedPromotionId = id;
            else
                _selectedPromotionId = Guid.Empty;
        }

        private void OnStartTimeChanged(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value?.ToString(), out var startDt))
                _startTime = startDt;
        }

        private void OnEndTimeChanged(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value?.ToString(), out var endDt))
                _endTime = endDt;
        }

        private async Task Submit()
        {
            _errorMessage = string.Empty;
            _successMessage = string.Empty;

            var request = new CreateSessionRequest(
                _selectedClientId,
                _SelectedStaffId,
                _selectedPromotionId,
                _selectedClinicId,
                _selectedRoomid,
                _selectedSessionTypeId,
                0,
                _startTime.Value,
                _endTime.Value);

            var result = await CreateSession.CreateSessionAsync(request);

            if (result.IsSuccess)
            {
                _successMessage = "Booking oprettet!";
                await Task.Delay(1500);
                Nav.NavigateTo("/calendar");
            }
            else
            {
                _errorMessage = result.Errors.FirstOrDefault()?.Message ?? "Oprettelse mislykkes";
            }
        }

        private void Cancel()
            => Nav.NavigateTo("/calendar");
    }

}
