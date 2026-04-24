namespace FysioEnterprise.Domain.Entities
{
    public class Room
    {
        public Guid RoomID { get; private set; }
        public Guid ClinicID { get; private set; }
        public int? RoomNumber { get; private set; }

        public Room() // Empty constructor for EF Core
        {
            
        }
        public Room(Clinic clinic, int? roomNumber)
        {
            RoomID = Guid.NewGuid();
            ClinicID = clinic.ClinicID;
            RoomNumber = roomNumber;
        }
    }
}
