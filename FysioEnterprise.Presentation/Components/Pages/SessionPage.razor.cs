using Microsoft.AspNetCore.Components;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.UseCase.SessionUseCase;
using FysioEnterprise.Presentation.Service;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;
using Radzen;
using System.Diagnostics.Eventing.Reader;

namespace FysioEnterprise.Presentation.Components.Pages
{
    public partial class SessionPage : ComponentBase
    {
        [Inject] private ICreateSessionUseCase CreateSession { get; set; } = default!;
        [Inject] private ISessionQueries SessionQueries { get; set; } = default!;
        [Inject] private ISimpleQueries SimpleQueries { get; set; } = default!;
        [Inject] private IClientQueries ClientQueries { get; set; } = default!;
        [Inject] private IPromotionQueries PromotionQueries { get; set; } = default!;
        [Inject] private IUpdateSessionUseCase UpdateSession { get; set; } = default!;
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
        private List<StaffDTO> _filteredStaff = [];
        private List<PromotionDTO> _allPromotions = [];

        //Selected værdier
        private Guid _selectedClientId;
        private Guid _selectedClinicId;
        private Guid _selectedStaffId;
        private Guid _selectedSessionTypeId;
        private Guid _selectedRoomId;
        private Guid _selectedPromotionId;
        private DateTime? _startTime;
        private DateTime? _endTime;

        private bool _isEditMode = false;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;

        private bool _editEndTime = false;

        private bool CanSubmit =>
            _selectedClientId != Guid.Empty &&
            _selectedClinicId != Guid.Empty &&
            _selectedStaffId != Guid.Empty &&
            _selectedSessionTypeId != Guid.Empty &&
            _selectedRoomId != Guid.Empty &&
            _startTime.HasValue &&
            _endTime.HasValue &&
            _startTime < _endTime;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            _clients = (await ClientQueries.GetAllClientsAsync())
                .OrderBy(c => c.ClientFirstName).ToList();

            _sessionTypes = (await SimpleQueries.GetAllSessionTypesAsync())
                .OrderBy(s => s.SessionTypeName).ToList();

            _allPromotions = (await PromotionQueries.GetAllPromotionsAsync())
                .OrderBy(p => p.PromotionName).ToList();

            _promotions = _allPromotions
                .Where(p => p.IsActive)
                .ToList();

            _selectedClinicId = Context.ClinicId;
            await LoadStaffAndRooms();

            await ApplyUrlParameters();
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

        private async Task ApplyUrlParameters()
        {
            if (DateTime.TryParse(Date, out var date) && Hour.HasValue)
            {
                _startTime = date.AddHours(Hour.Value);

                _promotions = _allPromotions
                    .Where(p => p.PromotionStartTime <= _startTime && p.PromotionEndTime >= _startTime)
                    .ToList();
            }

            if (SessionTypeId.HasValue && SessionTypeId != Guid.Empty)
            {
                _selectedSessionTypeId = SessionTypeId.Value;

                var sessionType = _sessionTypes.FirstOrDefault(s => s.SessionTypeID == SessionTypeId.Value);

                if (sessionType != null)
                {
                    _filteredStaff = _staffInClinic
                        .Where(s => sessionType.AllowedAuthorisationNumbers.Contains(s.StaffAuthorisationNumber))
                        .ToList();

                    if (_startTime.HasValue)
                        _endTime = _startTime.Value.Add(sessionType.SessionTypeTimeSpan.ToTimeSpan());
                }

            }

            //Til Redigering af bookings
            if (SessionId.HasValue && SessionId != Guid.Empty)
            {
                _isEditMode = true;

                var session = await SessionQueries.GetSessionByIdAsync(SessionId.Value);
                if (session != null)
                {
                    _selectedClientId = session.ClientID;
                    _selectedStaffId = session.StaffID;
                    _selectedSessionTypeId = session.SessionTypeID;
                    _selectedRoomId = session.RoomID;
                    _startTime = session.timeSlot.From;
                    _endTime = session.timeSlot.To;

                    var sessionType = _sessionTypes.FirstOrDefault(s => s.SessionTypeID == session.SessionTypeID);

                    if (sessionType != null)
                    {
                        _filteredStaff = _staffInClinic
                            .Where(s => sessionType.AllowedAuthorisationNumbers.Contains(s.StaffAuthorisationNumber))
                            .ToList();
                    }

                    _promotions = _allPromotions
                    .Where(p => p.PromotionStartTime <= _startTime && p.PromotionEndTime >= _startTime)
                    .ToList();
                }
            }
        }

        private void OnClientChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id))
                _selectedClientId = id;
        }

        private void OnStaffChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id))
                _selectedStaffId = id;
        }

        private void OnSessionTypeChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id))
            {
                _selectedSessionTypeId = id;
                var sessionType = _sessionTypes.FirstOrDefault(s => s.SessionTypeID == id);
                
                if (sessionType != null)
                {
                    _filteredStaff = _staffInClinic
                           .Where(s => sessionType.AllowedAuthorisationNumbers.Contains(s.StaffAuthorisationNumber))
                           .ToList();

                    if (_startTime.HasValue)
                    {
                        _endTime = _startTime.Value.Add(sessionType.SessionTypeTimeSpan.ToTimeSpan());
                    }
                    StateHasChanged();
                }

            }
        }

        private int GetSessionTypeInMinutes(SessionTypeDTO sessionType)
        {
            int minutes = 0;

            if (sessionType.SessionTypeTimeSpan.Hour > 0)
            {
                minutes = sessionType.SessionTypeTimeSpan.Hour * 60;
            }

            minutes += sessionType.SessionTypeTimeSpan.Minute;
            return minutes;
        }

        private void OnRoomChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id))
                _selectedRoomId = id;
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
            {
                _startTime = startDt;

                _promotions = _allPromotions
                    .Where(p => p.PromotionStartTime <= _startTime && p.PromotionEndTime >= _startTime)
                    .ToList();

                var sessionType = _sessionTypes.FirstOrDefault(s => s.SessionTypeID == _selectedSessionTypeId);

                if (sessionType != null)
                {
                    _endTime = _startTime.Value.Add(sessionType.SessionTypeTimeSpan.ToTimeSpan());
                }
                StateHasChanged();
            }
        }

        private void OnEndTimeChanged(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value?.ToString(), out var endDt))
                _endTime = endDt;
        }

        private void ToggleEndTimeEdit()
        {
            _editEndTime = !_editEndTime;
            StateHasChanged();
        }

        private async Task Submit()
        {
            _errorMessage = string.Empty;
            _successMessage = string.Empty;

            if (_isEditMode)
            {
                var updateRequest = new UpdateSessionRequest(
                    SessionId!.Value,
                    _selectedClientId,
                    _selectedStaffId,
                    _selectedClinicId,
                    _selectedRoomId,
                    _startTime!.Value,
                    _endTime!.Value);

                var result = await UpdateSession.UpdateSessionAsync(updateRequest);

                if (result.IsSuccess)
                {
                    _successMessage = "Booking opdateret!";
                    await Task.Delay(1500);
                    Nav.NavigateTo("/calendar");
                }
                else
                {
                    _errorMessage = result.Errors.FirstOrDefault()?.Message ?? "Opdatering af session mislykkedes";
                }
            }
            else
            {
            var request = new CreateSessionRequest(
                _selectedClientId,
                _selectedStaffId,
                _selectedPromotionId,
                _selectedClinicId,
                _selectedRoomId,
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

        }

        private void Cancel()
            => Nav.NavigateTo("/calendar");
    }

}
