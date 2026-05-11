using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Facade.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
                .Where(st => st.StaffID == staffId)
                .Select(st => new StaffDTO(
                    st.StaffID,
                    st.StaffFirstName,
                    st.StaffLastName,
                    st.StaffContactInformation,
                    st.StaffAuthorisationType,
                    st.StaffAuthorisationNumber,
                    _context.Clinics
                    .Where(cl => st.ClinicIDs.Contains(cl.ClinicID))
                    .Select(cl => cl.ClinicAddress)
                    .ToList()))
                .FirstOrDefaultAsync();


        }

        public async Task<List<StaffDTO>> GetAllStaffAsync()
        {
            return await _context.Staff
                .AsNoTracking()
                .Select(st => new StaffDTO(
                    st.StaffID,
                    st.StaffFirstName,
                    st.StaffLastName,
                    st.StaffContactInformation,
                    st.StaffAuthorisationType,
                    st.StaffAuthorisationNumber,
                    _context.Clinics
                    .Where(cl => st.ClinicIDs.Contains(cl.ClinicID))
                    .Select(cl => cl.ClinicAddress)
                    .ToList()))
                .ToListAsync();
        }

        public async Task<List<StaffDTO>> GetAllStaffByClinicAsync(Guid clinicId)
        {
            return await _context.Staff
                .AsNoTracking()
                .Where(st => st.ClinicIDs.Contains(clinicId))
                .Select(st => new StaffDTO(
                    st.StaffID,
                    st.StaffFirstName,
                    st.StaffLastName,
                    st.StaffContactInformation,
                    st.StaffAuthorisationType,
                    st.StaffAuthorisationNumber,
                    _context.Clinics
                    .Where(cl => st.ClinicIDs.Contains(cl.ClinicID))
                    .Select(cl => cl.ClinicAddress)
                    .ToList()))
                .ToListAsync();
        }

        public async Task<RoomDTO?> GetRoomsByIdAsync(Guid roomId)
        {
            return await _context.Rooms
                .AsNoTracking()
                .Where(r => r.RoomID == roomId)
                .Select(r => new RoomDTO(
                    r.RoomID,
                    _context.Clinics
                    .Where(cl => cl.ClinicID == r.ClinicID)
                    .Select(cl => cl.ClinicAddress)
                    .FirstOrDefault() ?? "",
                    r.RoomNumber))
                .FirstOrDefaultAsync();
        }

        public async Task<List<RoomDTO>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .AsNoTracking()
                .Select(r => new RoomDTO(
                    r.RoomID,
                    _context.Clinics
                    .Where(cl => cl.ClinicID == r.ClinicID)
                    .Select(cl => cl.ClinicAddress)
                    .FirstOrDefault() ?? "",
                    r.RoomNumber))
                .ToListAsync();
        }

        public async Task<ClinicDTO?> GetClinicByIdAsync(Guid clinicId)
        {
            return await _context.Clinics
                .AsNoTracking()
                .Where(cl => cl.ClinicID == clinicId)
                .Select(cl => new ClinicDTO(
                    cl.ClinicID,
                    cl.ClinicAddress,
                    cl.ClinicOpeningHours,
                    _context.Rooms
                    .Where(r => r.ClinicID == cl.ClinicID)
                    .Select(r => r.RoomNumber)
                    .ToList()))
                .FirstOrDefaultAsync();
        }

        public async Task<List<ClinicDTO>> GetAllClinicsAsync()
        {
            return await _context.Clinics
                .AsNoTracking()
                .Select(cl => new ClinicDTO(
                    cl.ClinicID,
                    cl.ClinicAddress,
                    cl.ClinicOpeningHours,
                    _context.Rooms
                    .Where(r => r.ClinicID == cl.ClinicID)
                    .Select(r => r.RoomNumber)
                    .ToList()))
                .ToListAsync();
        }

        public async Task<List<SessionTypeDTO>> GetAllSessionTypesAsync()
        {
            return await _context.SessionTypes
                .AsNoTracking()
                .Select(styp => new SessionTypeDTO(
                    styp.SessionTypeName,
                    styp.SessionTypePrice,
                    styp.SessionTypeMaxAmount,
                    styp.SessionTypeTimeSpan))
                .ToListAsync();
        }
    }
}
