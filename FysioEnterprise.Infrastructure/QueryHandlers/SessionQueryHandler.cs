using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Facade.Queries;
using Microsoft.EntityFrameworkCore;

namespace FysioEnterprise.Infrastructure.QueryHandlers
{
    public class SessionQueriesImpl : ISessionQueries
    {
        private readonly AppDBContext _context;

        public SessionQueriesImpl(AppDBContext context) => _context = context;

        public async Task<SessionDTO?> GetSessionByIdAsync(Guid sessionId)
        {
            return await _context.Sessions
                .AsNoTracking()
                .Where(s => s.Id == sessionId)
                .Select(s => new SessionDTO(
                    s.Id,
                    s.SessionClientID,
                    s.SessionStaffID,
                    s.SessionInstanceTypeID,
                    s.SessionRoomID,
                    s.SessionPromotion,
                    _context.Clients
                    .Where(c => c.Id == s.SessionClientID).Select(c => c.ClientFirstName).FirstOrDefault() ?? "",
                    _context.Clients
                    .Where(c => c.Id == s.SessionClientID).Select(c => c.ClientLastName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.Id == s.SessionStaffID).Select(st => st.StaffFirstName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.Id == s.SessionStaffID).Select(st => st.StaffLastName).FirstOrDefault() ?? "",
                    _context.Clinics
                    .SelectMany(cl => cl.ClinicRooms)
                    .Where(r => r.Id == s.SessionRoomID)
                    .Select(r => r.RoomNumber)
                    .FirstOrDefault(),
                    _context.SessionTypes
                    .Where(styp => styp.Id == s.SessionInstanceTypeID).Select(styp => styp.SessionTypeName).FirstOrDefault() ?? "",
                    _context.Promotions
                    .Where(p => p.Id == s.SessionPromotion).Select(p => p.PromotionName).FirstOrDefault() ?? "",
                    s.SessionTimeSlot,
                    s.priceTotal,
                    s.SessionStatus.ToString()))
                    .FirstOrDefaultAsync();
        }

        public async Task<List<SessionDTO>> GetAllSessionsByClientIdAsync(Guid clientId)
        {
            return await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionClientID == clientId)
                .Select(s => new SessionDTO(
                    s.Id,
                    s.SessionClientID,
                    s.SessionStaffID,
                    s.SessionInstanceTypeID,
                    s.SessionRoomID,
                    s.SessionPromotion,
                    _context.Clients
                    .Where(c => c.Id == s.SessionClientID).Select(c => c.ClientFirstName).FirstOrDefault() ?? "",
                    _context.Clients
                    .Where(c => c.Id == s.SessionClientID).Select(c => c.ClientLastName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.Id == s.SessionStaffID).Select(st => st.StaffFirstName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.Id == s.SessionStaffID).Select(st => st.StaffLastName).FirstOrDefault() ?? "",
                    _context.Clinics
                    .SelectMany(cl => cl.ClinicRooms)
                    .Where(r => r.Id == s.SessionRoomID)
                    .Select(r => r.RoomNumber)
                    .FirstOrDefault(),
                    _context.SessionTypes
                    .Where(styp => styp.Id == s.SessionInstanceTypeID).Select(styp => styp.SessionTypeName).FirstOrDefault() ?? "",
                    _context.Promotions
                    .Where(p => p.Id == s.SessionPromotion).Select(p => p.PromotionName).FirstOrDefault() ?? "",
                    s.SessionTimeSlot,
                    s.priceTotal,
                    s.SessionStatus.ToString()))
                    .ToListAsync();
        }

        public async Task<List<SessionDTO>> GetAllActiveSessionsByClientIdAsync(Guid clientId)
        {
            return await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionClientID == clientId && s.SessionStatus == SessionStatusEnum.Active)
                .Select(s => new SessionDTO(
                    s.Id,
                    s.SessionClientID,
                    s.SessionStaffID,
                    s.SessionInstanceTypeID,
                    s.SessionRoomID,
                    s.SessionPromotion,
                    _context.Clients
                    .Where(c => c.Id == s.SessionClientID).Select(c => c.ClientFirstName).FirstOrDefault() ?? "",
                    _context.Clients
                    .Where(c => c.Id == s.SessionClientID).Select(c => c.ClientLastName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.Id == s.SessionStaffID).Select(st => st.StaffFirstName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.Id == s.SessionStaffID).Select(st => st.StaffLastName).FirstOrDefault() ?? "",
                    _context.Clinics
                    .SelectMany(cl => cl.ClinicRooms)
                    .Where(r => r.Id == s.SessionRoomID)
                    .Select(r => r.RoomNumber)
                    .FirstOrDefault(),
                    _context.SessionTypes
                    .Where(styp => styp.Id == s.SessionInstanceTypeID).Select(styp => styp.SessionTypeName).FirstOrDefault() ?? "",
                    _context.Promotions
                    .Where(p => p.Id == s.SessionPromotion).Select(p => p.PromotionName).FirstOrDefault() ?? "",
                    s.SessionTimeSlot,
                    s.priceTotal,
                    s.SessionStatus.ToString()))
                    .ToListAsync();
        }

        public async Task<List<SessionDTO>> GetAllActiveSessionsByStaffIdAsync(Guid staffId)
        {
            return await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionStaffID == staffId && s.SessionStatus == SessionStatusEnum.Active)
                .Select(s => new SessionDTO(
                    s.Id,
                    s.SessionClientID,
                    s.SessionStaffID,
                    s.SessionInstanceTypeID,
                    s.SessionRoomID,
                    s.SessionPromotion,
                    _context.Clients
                    .Where(c => c.Id == s.SessionClientID).Select(c => c.ClientFirstName).FirstOrDefault() ?? "",
                    _context.Clients
                    .Where(c => c.Id == s.SessionClientID).Select(c => c.ClientLastName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.Id == s.SessionStaffID).Select(st => st.StaffFirstName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.Id == s.SessionStaffID).Select(st => st.StaffLastName).FirstOrDefault() ?? "",
                    _context.Clinics
                    .SelectMany(cl => cl.ClinicRooms)
                    .Where(r => r.Id == s.SessionRoomID)
                    .Select(r => r.RoomNumber)
                    .FirstOrDefault(),
                    _context.SessionTypes
                    .Where(styp => styp.Id == s.SessionInstanceTypeID).Select(styp => styp.SessionTypeName).FirstOrDefault() ?? "",
                    _context.Promotions
                    .Where(p => p.Id == s.SessionPromotion).Select(p => p.PromotionName).FirstOrDefault() ?? "",
                    s.SessionTimeSlot,
                    s.priceTotal,
                    s.SessionStatus.ToString()))
                    .ToListAsync();
        }

        public async Task<List<SessionDTO>> GetAllActiveSessionsByClincIdAsync(Guid clinicId)
        {
            return await _context.Sessions
                .AsNoTracking()
                .Where(s => _context.Clinics
                    .Where(c => c.Id == clinicId)
                    .SelectMany(c => c.ClinicRooms)
                    .Any(r => r.Id == s.SessionRoomID))
                .Select(s => new SessionDTO(
                    s.Id,
                    s.SessionClientID,
                    s.SessionStaffID,
                    s.SessionInstanceTypeID,
                    s.SessionRoomID,
                    s.SessionPromotion,
                    _context.Clients
                        .Where(c => c.Id == s.SessionClientID)
                        .Select(c => c.ClientFirstName).FirstOrDefault() ?? "",
                    _context.Clients
                        .Where(c => c.Id == s.SessionClientID)
                        .Select(c => c.ClientLastName).FirstOrDefault() ?? "",
                    _context.Staff
                        .Where(st => st.Id == s.SessionStaffID)
                        .Select(st => st.StaffFirstName).FirstOrDefault() ?? "",
                    _context.Staff
                        .Where(st => st.Id == s.SessionStaffID)
                        .Select(st => st.StaffLastName).FirstOrDefault() ?? "",
                    _context.Clinics
                        .SelectMany(cl => cl.ClinicRooms)
                        .Where(r => r.Id == s.SessionRoomID)
                        .Select(r => r.RoomNumber)
                        .FirstOrDefault(),
                    _context.SessionTypes
                        .Where(styp => styp.Id == s.SessionInstanceTypeID)
                        .Select(styp => styp.SessionTypeName).FirstOrDefault() ?? "",
                    _context.Promotions
                        .Where(p => p.Id == s.SessionPromotion)
                        .Select(p => p.PromotionName).FirstOrDefault() ?? "",
                    s.SessionTimeSlot,
                    s.priceTotal,
                    s.SessionStatus.ToString()
                ))
                .ToListAsync();
        }


    }
}
