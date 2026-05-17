using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FysioEnterprise.Presentation.Service
{
    public class LogInContext
    {
        private readonly ProtectedSessionStorage _storage;

        public LogInContext(ProtectedSessionStorage storage)
        {
            _storage = storage;
        }

        public Guid ClinicId { get; private set; }
        public string ClinicName { get; private set; } = string.Empty;
        public Guid StaffId { get; private set; }
        public string StaffName { get; private set; } = string.Empty;
        public bool IsLoggedIn => ClinicId != Guid.Empty && StaffId != Guid.Empty;

        public event Action? OnChange;

        public async Task SetSessionAsync(Guid clinicId, string clinicName, Guid staffId, string staffName)
        {
            ClinicId = clinicId;
            ClinicName = clinicName;
            StaffId = staffId;
            StaffName = staffName;
            await _storage.SetAsync("clinicId", clinicId.ToString());
            await _storage.SetAsync("clinicName", clinicName);
            await _storage.SetAsync("staffId", staffId.ToString());
            await _storage.SetAsync("staffName", staffName);
            OnChange?.Invoke();
        }

        public async Task LoadFromStorageAsync()
        {
            try
            {
                var clinicId = await _storage.GetAsync<string>("clinicId");
                var clinicName = await _storage.GetAsync<string>("clinicName");
                var staffId = await _storage.GetAsync<string>("staffId");
                var staffName = await _storage.GetAsync<string>("staffName");

                if (clinicId.Success && Guid.TryParse(clinicId.Value, out var cId))
                    ClinicId = cId;
                if (clinicName.Success)
                    ClinicName = clinicName.Value ?? string.Empty;
                if (staffId.Success && Guid.TryParse(staffId.Value, out var sId))
                    StaffId = sId;
                if (staffName.Success)
                    StaffName = staffName.Value ?? string.Empty;
            }
            catch { }

            OnChange?.Invoke();
        }

        public async Task ClearAsync()
        {
            ClinicId = Guid.Empty;
            ClinicName = string.Empty;
            StaffId = Guid.Empty;
            StaffName = string.Empty;
            await _storage.DeleteAsync("clinicId");
            await _storage.DeleteAsync("clinicName");
            await _storage.DeleteAsync("staffId");
            await _storage.DeleteAsync("staffName");
            OnChange?.Invoke();
        }
    }
}

