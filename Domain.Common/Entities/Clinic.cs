using System;
using System.Collections.Generic;
using System.Text;
using FluentResults;

namespace FysioEnterprise.Domain.Entities
{
    public class Clinic : Aggregateroot
    {
        public string ClinicAddress { get; private set; }
        public DateTime ClinicOpeningHours { get; private set; }
        
        private List<Room> _clinicRooms = new();
        public IReadOnlyList<Room> ClinicRooms => _clinicRooms.AsReadOnly();

        public Clinic() // Empty constructor for EF Core
        {
            
        }

        public Clinic(string clinicAddress, DateTime clinicOpeningHours, List<Room> clinicRooms)
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
                ? Result.Fail<Room>("Room not found.")
                : Result.Ok(room);
        }
    }
}
