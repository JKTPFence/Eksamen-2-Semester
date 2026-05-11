using FysioEnterprise.Domain.Entities;
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
                .Where(s => s.SessionID == sessionId)
                .Select(s => new SessionDTO(
                    s.SessionID,
                    _context.Clients
                    .Where(c => c.ClientID == s.SessionClientID).Select(c => c.ClientFirstName).FirstOrDefault() ?? "",
                    _context.Clients
                    .Where(c => c.ClientID == s.SessionClientID).Select(c => c.ClientLastName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.StaffID == s.SessionStaffID).Select(st => st.StaffFirstName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.StaffID == s.SessionStaffID).Select(st => st.StaffLastName).FirstOrDefault() ?? "",
                    _context.Rooms
                    .Where(r => r.RoomID == s.SessionRoomID).Select(r => r.RoomNumber ?? null).FirstOrDefault(),
                    _context.SessionTypes
                    .Where(styp => styp.SessionTypeId == s.SessionInstanceTypeID).Select(styp => styp.SessionTypeName).FirstOrDefault() ?? "",
                    _context.Promotions
                    .Where(p => p.PromotionID == s.SessionPromotion).Select(p => p.PromotionName).FirstOrDefault() ?? "",
                    s.SessionStartTime,
                    s.SessionEndTime,
                    s.SessionTotalPrice,
                    s.SessionStatus.ToString()))
                    .FirstOrDefaultAsync();
        }

        public async Task<List<SessionDTO>> GetAllSessionsByClientIdAsync(Guid clientId)
        {
            return await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionClientID == clientId)
                .Select(s => new SessionDTO(
                    s.SessionID,
                    _context.Clients
                    .Where(c => c.ClientID == s.SessionClientID).Select(c => c.ClientFirstName).FirstOrDefault() ?? "",
                    _context.Clients
                    .Where(c => c.ClientID == s.SessionClientID).Select(c => c.ClientLastName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.StaffID == s.SessionStaffID).Select(st => st.StaffFirstName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.StaffID == s.SessionStaffID).Select(st => st.StaffLastName).FirstOrDefault() ?? "",
                    _context.Rooms
                    .Where(r => r.RoomID == s.SessionRoomID).Select(r => r.RoomNumber ?? null).FirstOrDefault(),
                    _context.SessionTypes
                    .Where(styp => styp.SessionTypeId == s.SessionInstanceTypeID).Select(styp => styp.SessionTypeName).FirstOrDefault() ?? "",
                    _context.Promotions
                    .Where(p => p.PromotionID == s.SessionPromotion).Select(p => p.PromotionName).FirstOrDefault() ?? "",
                    s.SessionStartTime,
                    s.SessionEndTime,
                    s.SessionTotalPrice,
                    s.SessionStatus.ToString()))
                    .ToListAsync();
        }

        public async Task<List<SessionDTO>> GetAllActiveSessionsByClientIdAsync(Guid clientId)
        {
            return await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionClientID == clientId && s.SessionStatus == SessionStatusEnum.Active)
                .Select(s => new SessionDTO(
                    s.SessionID,
                    _context.Clients
                    .Where(c => c.ClientID == s.SessionClientID).Select(c => c.ClientFirstName).FirstOrDefault() ?? "",
                    _context.Clients
                    .Where(c => c.ClientID == s.SessionClientID).Select(c => c.ClientLastName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.StaffID == s.SessionStaffID).Select(st => st.StaffFirstName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.StaffID == s.SessionStaffID).Select(st => st.StaffLastName).FirstOrDefault() ?? "",
                    _context.Rooms
                    .Where(r => r.RoomID == s.SessionRoomID).Select(r => r.RoomNumber ?? null).FirstOrDefault(),
                    _context.SessionTypes
                    .Where(styp => styp.SessionTypeId == s.SessionInstanceTypeID).Select(styp => styp.SessionTypeName).FirstOrDefault() ?? "",
                    _context.Promotions
                    .Where(p => p.PromotionID == s.SessionPromotion).Select(p => p.PromotionName).FirstOrDefault() ?? "",
                    s.SessionStartTime,
                    s.SessionEndTime,
                    s.SessionTotalPrice,
                    s.SessionStatus.ToString()))
                    .ToListAsync();
        }

        public async Task<List<SessionDTO>> GetAllActiveSessionsByStaffIdAsync(Guid staffId)
        {
            return await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionStaffID == staffId && s.SessionStatus == SessionStatusEnum.Active)
                .Select(s => new SessionDTO(
                    s.SessionID,
                    _context.Clients
                    .Where(c => c.ClientID == s.SessionClientID).Select(c => c.ClientFirstName).FirstOrDefault() ?? "",
                    _context.Clients
                    .Where(c => c.ClientID == s.SessionClientID).Select(c => c.ClientLastName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.StaffID == s.SessionStaffID).Select(st => st.StaffFirstName).FirstOrDefault() ?? "",
                    _context.Staff
                    .Where(st => st.StaffID == s.SessionStaffID).Select(st => st.StaffLastName).FirstOrDefault() ?? "",
                    _context.Rooms
                    .Where(r => r.RoomID == s.SessionRoomID).Select(r => r.RoomNumber ?? null).FirstOrDefault(),
                    _context.SessionTypes
                    .Where(styp => styp.SessionTypeId == s.SessionInstanceTypeID).Select(styp => styp.SessionTypeName).FirstOrDefault() ?? "",
                    _context.Promotions
                    .Where(p => p.PromotionID == s.SessionPromotion).Select(p => p.PromotionName).FirstOrDefault() ?? "",
                    s.SessionStartTime,
                    s.SessionEndTime,
                    s.SessionTotalPrice,
                    s.SessionStatus.ToString()))
                    .ToListAsync();
        }
    }
}
