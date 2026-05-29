using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

namespace FysioEnterprise.Presentation.Components.Pages
{
    public partial class HomePage : ComponentBase
    {
        [Inject] private ISimpleQueries Queries { get; set; } = default!;
        [Inject] private LogInContext Context { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private List<ClinicDTO> _clinics = [];
        private List<StaffDTO> _staff = [];
        private List<StaffDTO> _receptionistsInClinic = [];

        private string _selectedStaffValue = "";
        private Guid _selectedClinicId;
        private Guid _selectedStaffId = Guid.Empty;

        private bool CanProceed =>
            _selectedClinicId != Guid.Empty &&
            _selectedStaffId != Guid.Empty;

        protected override async Task OnInitializedAsync()
        {
            var fetchedClinics = await Queries.GetAllClinicsAsync();

            _clinics = fetchedClinics.OrderBy(c => c.ClinicAddress).ToList();
        }

        private async Task OnClinicChanged(ChangeEventArgs e)  // When the clinic selection changes, we need to fetch the staff for that clinic and filter out the receptionists
        {
            var value = e.Value?.ToString();
            try
            {
                if (!string.IsNullOrEmpty(value) && Guid.TryParse(value, out var clinicId))
                {

                    _selectedClinicId = clinicId;

                    var staffResult = await Queries.GetAllStaffByClinicAsync(_selectedClinicId);
                    _staff = staffResult.OrderBy(s => s.StaffFirstName).ToList();

                    if (_staff.IsNullOrEmpty())
                        throw new ArgumentNullException();

                    _receptionistsInClinic = _staff
                        .Where(s => s.StaffAuthorisationNumber == 22222 || s.StaffAuthorisationType == "Receptionist")
                        .OrderBy(s => s.StaffFirstName)
                        .ToList();

                    if (_receptionistsInClinic.Count() == 0 || _receptionistsInClinic == null)
                        throw new ArgumentNullException();

                    // Workaround to Reset staff selection when clinic changes
                    _selectedStaffValue = "";
                    _selectedStaffId = Guid.Empty;

                    StateHasChanged();
                }
            }
            catch (ArgumentNullException ex)
            {
                System.Diagnostics.Debug.WriteLine($"No receptionist found for clinic {ex}");
            }
        }

        private void OnStaffChanged(ChangeEventArgs e)
        {
            var value = e.Value?.ToString();
            if (string.IsNullOrEmpty(value) || value == "— Vælg receptionist —")
            {
                _selectedStaffId = Guid.Empty;
                _selectedStaffValue = "— Vælg receptionist —";
                StateHasChanged();
                return;
            }
            if (!string.IsNullOrEmpty(value) && Guid.TryParse(value, out var staffId))
            {
                _selectedStaffId = staffId;
                _selectedStaffValue = value;
            }
            StateHasChanged();
        }

        private async Task Proceed()
        {
            if (!CanProceed) return;

            var clinic = _clinics.First(c => c.ClinicID == _selectedClinicId);
            var staff = _staff.First(s => s.StaffID == _selectedStaffId);

            await Context.SetSessionAsync(
                clinic.ClinicID,
                clinic.ClinicAddress,
                staff.StaffID,
                $"{staff.StaffFirstName} {staff.StaffLastName}");

            Nav.NavigateTo("/calendar");
        }
    }
}
