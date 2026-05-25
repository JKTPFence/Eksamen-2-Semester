using System.Collections;
using System.Diagnostics.Eventing.Reader;
using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.UseCase.SessionUseCase;
using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;
using Radzen;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

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
        [SupplyParameterFromQuery] private int? Minute { get; set; }
        [SupplyParameterFromQuery] private Guid? SessionTypeId { get; set; }
        [SupplyParameterFromQuery] private Guid? SessionId { get; set; }

        //Data lister
        private List<ClientDTO> _clients = [];
        private List<StaffDTO> _staffInClinic = [];
        private List<SessionTypeDTO> _sessionTypes = [];
        private List<RoomDTO> _rooms = [];
        private List<PromotionDTO> _promotions = [];
        private List<StaffDTO> _filteredStaff = [];
        private List<PromotionDTO> _allPromotions = [];
        private bool _isComboMode { get; set; } = false;
        private int _comboStep { get; set; } = 1;

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
        private bool _editEndTime = false;
        private bool _isLoading = false;

        private bool CanSubmit =>
            _selectedClientId != Guid.Empty &&
            _selectedClinicId != Guid.Empty &&
            _selectedStaffId != Guid.Empty &&
            _selectedSessionTypeId != Guid.Empty &&
            _selectedRoomId != Guid.Empty &&
            _startTime.HasValue &&
            _endTime.HasValue &&
            _startTime < _endTime;

        private record PendingSession(
        Guid ClientId,
        Guid SessionTypeId,
        Guid StaffId,
        Guid ClinicId,
        Guid RoomId,
        Guid PromotionId,
        DateTime StartTime,
        DateTime EndTime);

        private bool IsStepTwoLocked => _isComboMode && _comboStep == 2;

        private PendingSession? _firstSession = null;

        private static readonly Dictionary<string, List<string>> AllowedComboRules = new()
        {
            { "Fysioterapi",    new() { "Akupunktur", "Sportsmassage" } },
            { "Genoptræning",   new() { "Fysioterapi", "Sportsmassage", "Kostvejledning førstegang", "Kostvejledning opfølgning" } },
            { "Sportsmassage",  new() { "Akupunktur", "Genoptræning" } },
            { "Akupunktur",    new() { "Fysioterapi", "Sportsmassage" } },
            { "Kostvejledning førstegang",  new() { "Genoptræning" } },
            { "Kostvejledning opfølgning",  new() { "Genoptræning" } }
        };


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
                if (Minute.HasValue)
                    _startTime = date.AddHours(Hour.Value).AddMinutes(Minute.Value);
                else
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
            if (IsStepTwoLocked) return;

            if (Guid.TryParse(e.Value?.ToString(), out var id))
                _selectedClientId = id;
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

        private void OnPromotionChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var id))
                _selectedPromotionId = id;
            else
                _selectedPromotionId = Guid.Empty;
        }

        private void OnStartTimeChanged(ChangeEventArgs e)
        {
            if (IsStepTwoLocked) return;

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

        private void SelectSessionType(Guid id)
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

        private void SelectStaff(Guid id)
        {
            _selectedStaffId = id;
            StateHasChanged();
        }

        private void SelectRoom(Guid id)
        {
            _selectedRoomId = id;
            StateHasChanged();
        }

        private async Task Submit()
        {
            if (_isLoading) return;

            if (_isComboMode && _comboStep == 1)
            {
                _firstSession = new PendingSession(
                _selectedClientId,
                _selectedSessionTypeId,
                _selectedStaffId,
                _selectedClinicId,
                _selectedRoomId,
                _selectedPromotionId,
                _startTime!.Value,
                _endTime!.Value);

               
                var nextStart = _endTime!.Value;
                var firstSessionType = _sessionTypes.FirstOrDefault(s => s.SessionTypeID == _selectedSessionTypeId);
                string firstTypeName = firstSessionType?.SessionTypeName ?? "";
                ResetForm();
                _selectedClientId = _firstSession.ClientId;
                _selectedClinicId = _firstSession.ClinicId;
                _startTime = nextStart;
                _comboStep = 2;
                _sessionTypes = _sessionTypes.Where(type =>
                {
                    bool allowedAsSecond = AllowedComboRules.TryGetValue(firstTypeName, out var list1) && list1.Contains(type.SessionTypeName);
                    bool allowedAsFirst = AllowedComboRules.TryGetValue(type.SessionTypeName, out var list2) && list2.Contains(firstTypeName);

                    return allowedAsSecond || allowedAsFirst;
                }).ToList();

                StateHasChanged();
                return; 
            }

            _isLoading = true;
            StateHasChanged();

            try
            {
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

                    var result1 = await UpdateSession.UpdateSessionAsync(updateRequest);

                    if (result1.IsSuccess)
                    {
                        Notification.ShowSuccess("Booking opdateret!");
                        await Task.Delay(1500);
                        Nav.NavigateTo("/calendar");
                    }
                    else
                    {
                        Notification.ShowError(result1.Errors.FirstOrDefault()?.Message ?? "Opdatering af session mislykkedes");
                    }
                }
                if (_isComboMode && _comboStep == 2 && _firstSession is not null)
                {

                    var request1 = new CreateSessionRequest(
                        _firstSession.ClientId,
                        _firstSession.StaffId,
                        _firstSession.PromotionId,
                        _firstSession.ClinicId,
                        _firstSession.RoomId,
                        _firstSession.SessionTypeId,
                        0,
                        _firstSession.StartTime,
                        _firstSession.EndTime);

                    var request2 = new CreateSessionRequest(
                        _selectedClientId,
                        _selectedStaffId,
                        _selectedPromotionId,
                        _selectedClinicId,
                        _selectedRoomId,
                        _selectedSessionTypeId,
                        0,
                        _startTime!.Value,
                        _endTime!.Value);

                        var result1 = await CreateSession.CreateSessionAsync(request1);
                        if (result1.IsSuccess)
                        {
                            var result2 = await CreateSession.CreateSessionAsync(request2);

                            if (result2.IsSuccess)
                            {
                                Notification.ShowSuccess("Combo Booking oprettet!");
                                await Task.Delay(1500);
                                Nav.NavigateTo("/calendar");
                            }
                            else
                            {
                                Notification.ShowError(result2.Errors.FirstOrDefault()?.Message ?? "Oprettelse af session 2 mislykkedes");
                            }
                        }
                        else
                        {
                            Notification.ShowError(result1.Errors.FirstOrDefault()?.Message ?? "Oprettelse af session 1 mislykkedes");
                        }
                    return;
                }

                    var singleRequest = new CreateSessionRequest(
                            _selectedClientId,
                            _selectedStaffId,
                            _selectedPromotionId,
                            _selectedClinicId,
                            _selectedRoomId,
                            _selectedSessionTypeId,
                            0,
                            _startTime!.Value,
                            _endTime!.Value);

                    var result = await CreateSession.CreateSessionAsync(singleRequest);

                    if (result.IsSuccess)
                    {
                        Notification.ShowSuccess("Booking opdateret!");
                        await Task.Delay(1500);
                        Nav.NavigateTo("/calendar");
                    }
                    else
                    {
                        Notification.ShowError(result.Errors.FirstOrDefault()?.Message ?? "Opdatering af session mislykkedes");
                        StateHasChanged();

                    }
                
              }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            
            }
        }

        private void Cancel()
            => Nav.NavigateTo("/calendar");

        private Result<string> GetStaffNameWithId(Guid staffID)
        {
            var clinicStaff = _staffInClinic.FirstOrDefault(s => s.StaffID == staffID);
            if (clinicStaff == null)
            {
                return Result.Fail<string>("No staff found with the given ID in this clinic");
            }

            var staffResult = _filteredStaff.FirstOrDefault(s => s.StaffID == staffID);
            if (staffResult != null)
            {
                string fullName = $"{staffResult.StaffFirstName} {staffResult.StaffLastName}";
                return Result.Ok(fullName);
            }

            return Result.Fail<string>("No staff found with the given ID");
        }

        private async Task ToggleComboMode()
        {
            _isComboMode = !_isComboMode;
            _comboStep = 1;
            _firstSession = null;

            if (!_isComboMode)
            {
                ResetForm();
                _sessionTypes = (await SimpleQueries.GetAllSessionTypesAsync())
                .OrderBy(s => s.SessionTypeName).ToList();
            }
            StateHasChanged();
        }

        private void ResetForm()
        {
            _selectedClientId = Guid.Empty;
            _selectedSessionTypeId = Guid.Empty;
            _selectedStaffId = Guid.Empty;
            _selectedRoomId = Guid.Empty;
            _selectedPromotionId = Guid.Empty;
            _startTime = null;
            _endTime = null;
            _editEndTime = false;
        }
    }

}
