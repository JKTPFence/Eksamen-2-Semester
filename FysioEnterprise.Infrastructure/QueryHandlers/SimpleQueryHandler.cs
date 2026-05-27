using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Facade.Queries;
using Microsoft.EntityFrameworkCore;

namespace FysioEnterprise.Infrastructure.QueryHandlers
{
    public class SimpleQueriesImpl : ISimpleQueries
    {
        private readonly AppDBContext _context;

        public SimpleQueriesImpl(AppDBContext context) => _context = context;


        public async Task<StaffDTO?> GetStaffByIdAsync(Guid staffId)
        {
            return await _context.Staff
                .AsNoTracking()
                .Where(st => st.Id == staffId)
                .Select(st => new StaffDTO(
                    st.Id,
                    st.StaffFirstName,
                    st.StaffLastName,
                    st.StaffContactInformation,
                    st.StaffAuthorisationType,
                    st.StaffAuthorisationNumber,
                    _context.Clinics
                    .Where(cl => st.ClinicAssignments.Select(a => a.ClinicId).Contains(cl.Id))
                    .Select(cl => cl.ClinicAddress)
                    .ToList()))
                .FirstOrDefaultAsync();


        }

        public async Task<List<StaffDTO>> GetAllStaffAsync()
        {
            return await _context.Staff
                .AsNoTracking()
                .Select(st => new StaffDTO(
                    st.Id,
                    st.StaffFirstName,
                    st.StaffLastName,
                    st.StaffContactInformation,
                    st.StaffAuthorisationType,
                    st.StaffAuthorisationNumber,
                    _context.Clinics
                    .Where(cl => st.ClinicAssignments
                        .Select(a => a.ClinicId)
                        .Contains(cl.Id))
                    .Select(cl => cl.ClinicAddress)
                    .ToList()))
                .ToListAsync();
        }

        public async Task<List<StaffDTO>> GetAllStaffByClinicAsync(Guid clinicId)
        {
            var staffList = await _context.Staff
                .AsNoTracking()
                .Include(st => st.ClinicAssignments)
                .Where(st => st.ClinicAssignments.Any(a => a.ClinicId == clinicId))
                .Select(st => new StaffDTO(
                    st.Id,
                    st.StaffFirstName,
                    st.StaffLastName,
                    st.StaffContactInformation,
                    st.StaffAuthorisationType,
                    st.StaffAuthorisationNumber,
                    _context.Clinics
                    .Where(cl => st.ClinicAssignments.Select(a => a.ClinicId).Contains(cl.Id))
                    .Select(cl => cl.ClinicAddress)
                    .ToList()))
                .ToListAsync();

            return staffList;
        }


        public async Task<ClinicDTO?> GetClinicByIdAsync(Guid clinicId)
        {
            return await _context.Clinics
                .AsNoTracking()
                .Include(cl => cl.ClinicRooms)
                .Where(cl => cl.Id == clinicId)
                .Select(cl => new ClinicDTO(
                    cl.Id,
                    cl.ClinicAddress,
                    cl.ClinicOpeningHours,
                    cl.ClinicRooms
                    .Select(r => r.RoomNumber)
                    .ToList()))
                .FirstOrDefaultAsync();
        }

        public async Task<List<ClinicDTO>> GetAllClinicsAsync()
        {
            return await _context.Clinics
                .AsNoTracking()
                .Include(cl => cl.ClinicRooms)
                .Select(cl => new ClinicDTO(
                    cl.Id,
                    cl.ClinicAddress,
                    cl.ClinicOpeningHours,
                    cl.ClinicRooms
                    .Select(r => r.RoomNumber)
                    .ToList()))
                .ToListAsync();
        }

        public async Task<List<RoomDTO>> GetRoomsByClinicIdAsync(Guid clinicId)
        {
            return await _context.Clinics
                .AsNoTracking()
                .Include(cl => cl.ClinicRooms)
                .Where(cl => cl.Id == clinicId)
                .SelectMany(cl => cl.ClinicRooms
                    .Select(r => new RoomDTO(
                        r.Id,
                        cl.ClinicAddress,
                        r.RoomNumber)))
                .ToListAsync();
        }

        public async Task<List<SessionTypeDTO>> GetAllSessionTypesAsync()
        {
            return await _context.SessionTypes
                .AsNoTracking()
                .Select(styp => new SessionTypeDTO(
                    styp.Id,
                    styp.SessionTypeName,
                    styp.SessionTypePrice.Value,
                    styp.SessionTypeMaxAmount,
                    styp.SessionTypeTimeSpan,
                    styp.AllowedAuthorisationNumbers))
                .ToListAsync();
        }
    }
}
