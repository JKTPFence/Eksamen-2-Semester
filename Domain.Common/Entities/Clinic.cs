using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Entities
{
    public class Clinic
    {
        public Guid ClinicID { get; private set; }
        public string ClinicAddress { get; private set; }
        public DateTime ClinicOpeningHours { get; private set; }
        public List<Room> ClinicRooms { get; private set; }
        public Clinic() // Empty constructor for EF Core
        {
            
        }

        public Clinic(string clinicAddress, DateTime clinicOpeningHours, List<Room> clinicRooms)
        {
            ClinicID = Guid.NewGuid();
            ClinicAddress = clinicAddress;
            ClinicOpeningHours = clinicOpeningHours;
            ClinicRooms = clinicRooms;
        }
    }
}
