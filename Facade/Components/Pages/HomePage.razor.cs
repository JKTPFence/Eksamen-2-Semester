using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using Microsoft.AspNetCore.Components;

namespace FysioEnterprise.Presentation.Components.Pages
{
    public partial class HomePage : ComponentBase
    {
        [Inject] private ISimpleQueries Queries { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private List<ClinicDTO> _clinics = [];
        private List<StaffDTO> _staff = [];

        private Guid _selectedClinicId;
        private Guid _selectedStaffId;

        private bool CanProceed =>
            _selectedClinicId != Guid.Empty &&
            _selectedStaffId != Guid.Empty;

        protected override async Task OnInitializedAsync()
        {
            _clinics = await Queries.GetAllClinicsAsync();
            _staff = await Queries.GetAllStaffAsync();
        }

        private async Task OnClinicChanged(ChangeEventArgs e)
        {
            _selectedClinicId = Guid.Parse(e.Value!.ToString()!);
            _selectedStaffId = Guid.Empty;
            _staff = await Queries.GetAllStaffByClinicAsync(_selectedClinicId);
        }

        private void Proceed()
        {
            if (!CanProceed) return;
            Nav.NavigateTo($"/bookings?clinicId={_selectedClinicId}&staffId={_selectedStaffId}");
        }
    }
}
