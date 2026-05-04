using FysioEnterprise.Application.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.UseCase.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Infrastructure.Database.Repository
{
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDBContext _context;
        public StaffRepository(AppDBContext context) => _context = context;

        public async Task<Staff?> GetStaffAsync(Guid staffId)
            => await _context.Staff.FindAsync(staffId);
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly AppDBContext _context;
        public RoomRepository(AppDBContext context) => _context = context;

        public async Task<Room?> GetRoomAsync(Guid roomId)
            => await _context.Rooms.FindAsync(roomId);
    }

    public class ClinicRepository : IClinicRepository
    {
        private readonly AppDBContext _context;
        public ClinicRepository(AppDBContext context) => _context = context;

        public async Task<Clinic?> GetClinicAsync(Guid clinicId)
            => await _context.Clinics.FindAsync(clinicId);
    }
}
