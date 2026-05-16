using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.UseCase.IRepositories;
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
    public class SessionTypeRepository : ISessionTypeRepository
    {
        private readonly AppDBContext _context;
        public SessionTypeRepository(AppDBContext context) => _context = context;

        public async Task<Result<SessionType>> GetSessionTypeAsync(Guid sessionTypeId)
        {
            var sessionTypeResult = await _context.SessionTypes.FindAsync(sessionTypeId);

            if (sessionTypeResult == null)
            {
                return Result.Fail<SessionType>("Session type not found.");
            }
            return Result.Ok(sessionTypeResult);
        }
    }
}
