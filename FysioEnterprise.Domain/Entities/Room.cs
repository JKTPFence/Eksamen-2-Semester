namespace FysioEnterprise.Domain.Entities
{
    public class Room : Entity
    {
        public Guid ClinicID { get; init; }
        public int? RoomNumber { get; private set; }

        public Room() // Empty constructor for EF Core
        {
            
        }
        public Room(Clinic clinic, int? roomNumber)
        {
            Id = Guid.NewGuid();
            ClinicID = clinic.Id;
            RoomNumber = roomNumber;
        }
    }
}
