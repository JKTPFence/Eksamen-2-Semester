using FluentResults;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Entities
{
    public class Clinic : Aggregateroot
    {
        public string ClinicAddress { get; private set; }
        public List<OpeningHours> ClinicOpeningHours { get; private set; } = new();

        private List<Room> _clinicRooms = new();
        public IReadOnlyList<Room> ClinicRooms => _clinicRooms.AsReadOnly();

        public Clinic() // Empty constructor for EF Core
        {
            
        }

        public Clinic(string clinicAddress, List<OpeningHours> clinicOpeningHours, List<Room> clinicRooms)
        {
            Id = Guid.NewGuid();
            ClinicAddress = clinicAddress;
            ClinicOpeningHours = clinicOpeningHours;
            _clinicRooms = clinicRooms;
        }

        public Result<Room> GetRoom(Guid roomId)
        {
            var room = _clinicRooms.FirstOrDefault(r => r.Id == roomId);
            return room is null
                ? Result.Fail<Room>("Rummet blev ikke fundet")
                : Result.Ok(room);
        }
    }
}
