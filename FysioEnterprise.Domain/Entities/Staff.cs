using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Entities
{
    public class Staff : Aggregateroot
    {
        public string StaffFirstName { get; private set; }
        public string? StaffLastName { get; private set; }
        public string StaffContactInformation { get; private set; }
        public string StaffAuthorisationType { get; private set; }
        public int StaffAuthorisationNumber { get; private set; }

        private readonly List<StaffClinicAssignment> _clinicAssignments = new();
        public IReadOnlyList<StaffClinicAssignment> ClinicAssignments =>
            _clinicAssignments.AsReadOnly();
        public Staff() // Empty constructor for EF Core
        {
            
        }

        public Staff(string staffFirstName, string? staffLastName, string staffContactInformation, string staffAuthorisationType, int staffAuthorisationNumber, List<Clinic> clinics)
        {
            Id = Guid.NewGuid();
            StaffFirstName = staffFirstName;
            StaffLastName = staffLastName;
            StaffContactInformation = staffContactInformation;
            StaffAuthorisationType = staffAuthorisationType;
            StaffAuthorisationNumber = staffAuthorisationNumber;
        }

        public void AssignToClinic(Guid clinicId)
        {
            var assignment = new StaffClinicAssignment(Id, clinicId);

            if (_clinicAssignments.Contains(assignment))
                return;

            _clinicAssignments.Add(assignment);
        }

        public void RemoveFromClinic(Guid clinicId)
        {
            var assignment = new StaffClinicAssignment(Id, clinicId);
            _clinicAssignments.Remove(assignment);
        }
    }
}
