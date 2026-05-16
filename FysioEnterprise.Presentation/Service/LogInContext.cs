namespace FysioEnterprise.Presentation.Service
{
    public class LogInContext
    {
        public Guid ClinicId { get; private set; }
        public string ClinicName { get; private set; } = string.Empty;
        public Guid StaffId { get; private set; }
        public string StaffName { get; private set; } = string.Empty;
        public bool IsLoggedIn => ClinicId != Guid.Empty && StaffId != Guid.Empty;

        public event Action? OnChange;

        public void SetSession(Guid clinicId, string clinicName, Guid staffId, string staffName)
        {
            ClinicId = clinicId;
            ClinicName = clinicName;
            StaffId = staffId;
            StaffName = staffName;
            OnChange?.Invoke();
        }

        public void Clear()
        {
            ClinicId = Guid.Empty;
            ClinicName = string.Empty;
            StaffId = Guid.Empty;
            StaffName = string.Empty;
            OnChange?.Invoke();
        }
    }
}

