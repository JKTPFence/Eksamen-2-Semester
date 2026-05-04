using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.UseCase.Repository.Interfaces;
namespace FysioEnterprise.Infrastructure.Database.Repository
{
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDBContext _context;
        public StaffRepository(AppDBContext context) => _context = context;

        public async Task<Result<Staff>> GetStaffAsync(Guid staffId)
        {
           var staffResult = await _context.Staff.FindAsync(staffId);

            if (staffResult == null)
            {
                return Result.Fail<Staff>("Staff member not found.");
            }
            return Result.Ok(staffResult);
        }
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly AppDBContext _context;
        public RoomRepository(AppDBContext context) => _context = context;

        public async Task<Result<Room>> GetRoomAsync(Guid roomId)
        {
            var roomResult = await _context.Rooms.FindAsync(roomId);

            if (roomResult == null)
            {
                return Result.Fail<Room>("Room not found.");
            }
            return Result.Ok(roomResult);
        }
    }

    public class ClinicRepository : IClinicRepository
    {
        private readonly AppDBContext _context;
        public ClinicRepository(AppDBContext context) => _context = context;

        public async Task<Result<Clinic>> GetClinicAsync(Guid clinicId)
        {
            var clinicResult = await _context.Clinics.FindAsync(clinicId);

            if (clinicResult == null)
            {
                return Result.Fail<Clinic>("Clinic not found.");
            }
            return Result.Ok(clinicResult);
        }
    }
}
