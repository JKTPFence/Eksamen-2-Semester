using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Domain.Entities
{
    public class Staff
    {
        public Guid StaffID { get; private set; }
        public string StaffFirstName { get; private set; }
        public string? StaffLastName { get; private set; }
        public string StaffContactInformation { get; private set; }
        public string StaffAuthorisationType { get; private set; }
        public int StaffAuthorisationNumber { get; private set; }
        public List<Guid> ClinicIDs { get; private set; }
        public Staff() // Empty constructor for EF Core
        {
            
        }

        public Staff(string staffFirstName, string? staffLastName, string staffContactInformation, string staffAuthorisationType, int staffAuthorisationNumber, List<Clinic> clinics)
        {
            StaffID = Guid.NewGuid();
            StaffFirstName = staffFirstName;
            StaffLastName = staffLastName;
            StaffContactInformation = staffContactInformation;
            StaffAuthorisationType = staffAuthorisationType;
            StaffAuthorisationNumber = staffAuthorisationNumber;
            ClinicIDs = clinics.Select(c => c.ClinicID).ToList();
        }
    }
}
