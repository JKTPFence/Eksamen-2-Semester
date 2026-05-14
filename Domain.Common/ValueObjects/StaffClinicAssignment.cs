
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Domain.ValueObjects
{
    public class StaffClinicAssignment : ValueObject
    {
        public Guid StaffId { get; }
        public Guid ClinicId { get; }

        public StaffClinicAssignment() { } // Empty constructor for EF Core

        public StaffClinicAssignment(Guid staffId, Guid clinicId)
        {
            StaffId = staffId;
            ClinicId = clinicId;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StaffId;
            yield return ClinicId;
        }
    }
}
