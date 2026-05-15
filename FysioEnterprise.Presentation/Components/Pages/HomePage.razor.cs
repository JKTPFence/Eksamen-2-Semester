using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Radzen.Blazor.Markdown;

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

        private Guid _selectedClinicId;
        private Guid _selectedStaffId;

        private bool CanProceed =>
            _selectedClinicId != Guid.Empty &&
            _selectedStaffId != Guid.Empty;

        protected override async Task OnInitializedAsync()
        {
            _clinics = await Queries.GetAllClinicsAsync();
        }

        private async void OnClinicChanged(ChangeEventArgs e)
        {
            var value = e.Value?.ToString();
            try
            {
                if (!string.IsNullOrEmpty(value) && Guid.TryParse(value, out var clinicId))
                {

                        _selectedClinicId = clinicId;
                        _selectedStaffId = Guid.Empty;

                        var staffResult = await Queries.GetAllStaffByClinicAsync(_selectedClinicId);
                        _staff = staffResult ?? new List<StaffDTO>();

                        if (_staff.IsNullOrEmpty() || _staff.Count() == 0)
                            throw new ArgumentNullException();

                        _receptionistsInClinic = _staff
                            .Where(s => s.StaffAuthorisationNumber == 22222 || s.StaffAuthorisationType == "Receptionist")
                            .ToList();

                        if (_receptionistsInClinic.Count() == 0)
                            throw new ArgumentNullException();

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
            if (!string.IsNullOrEmpty(value) && Guid.TryParse(value, out var staffId))
            {
                _selectedStaffId = staffId;
            }
            StateHasChanged();
        }

        private void Proceed()
        {
            if (!CanProceed) return;

            var clinic = _clinics.First(c => c.ClinicID == _selectedClinicId);
            var staff = _staff.First(s => s.StaffID == _selectedStaffId);

            Context.SetSession(
                clinic.ClinicID,
                clinic.ClinicAddress,
                staff.StaffID,
                $"{staff.StaffFirstName} {staff.StaffLastName}");

            Nav.NavigateTo("/calendar");
        }
    }
}
