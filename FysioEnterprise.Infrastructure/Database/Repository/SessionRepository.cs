using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.UseCase.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace FysioEnterprise.Infrastructure.Database.Repository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly IDbContextFactory<AppDBContext> _contextFactory;

        public SessionRepository(IDbContextFactory<AppDBContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task CreateSessionAsync(Session session)
        {
                await using var _context = _contextFactory.CreateDbContext();
                await _context.Sessions.AddAsync(session);
                await _context.SaveChangesAsync();
        }

        //Bruger ikke fluent result her, grundet komplikation med Deletemetode og andre metoder der bruger "GetSessionAsync"
        public async Task<Result<Session>> GetSessionAsync(Guid sessionId)
        {
            await using var _context = _contextFactory.CreateDbContext();
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                return Result.Fail($"Session with ID {sessionId} was not found.");

            return Result.Ok(session);
        }

        public async Task<List<Session>> GetSessionsByClientAsync(Guid clientId, Guid? excludeSessionId = null)
        {
            await using var _context = _contextFactory.CreateDbContext();
            var sessions = await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionClientID == clientId)
                .Where(s => excludeSessionId == null || s.Id != excludeSessionId.Value)
                .ToListAsync();

            return sessions;
        }

        public async Task<List<Session>> GetSessionsByStaffAsync(Guid staffId, Guid? excludeSessionId = null)
        {
            await using var _context = _contextFactory.CreateDbContext();
            var sessions = await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionStaffID == staffId)
                .Where(s => excludeSessionId == null || s.Id != excludeSessionId.Value)
                .ToListAsync();

            return sessions;
        }

        public async Task<List<Session>> GetSessionsByRoomAsync(Guid clinicId, Guid roomId, Guid? excludeSessionId = null)
        {
            await using var _context = _contextFactory.CreateDbContext();
            var clinic = await _context.Clinics
                .AsNoTracking()
                .Include(c => c.ClinicRooms)
                .FirstOrDefaultAsync(c => c.Id == clinicId);

            if (clinic is null)
                throw new NotFoundException($"Clinic {clinicId} not found");

            var roomExists = clinic.ClinicRooms.Any(r => r.Id == roomId);
            if (!roomExists)
                throw new NotFoundException($"Room {roomId} does not belong to clinic {clinicId}");

            var sessions = await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionRoomID == roomId)
                .Where(s => excludeSessionId == null || s.Id != excludeSessionId.Value)
                .ToListAsync(); 

            return sessions;
        }

        public async Task UpdateSessionAsync(Session session)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.Sessions
                .Where(s => s.Id == session.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.SessionClientID, session.SessionClientID)
                    .SetProperty(x => x.SessionStaffID, session.SessionStaffID)
                    .SetProperty(x => x.SessionInstanceTypeID, session.SessionInstanceTypeID)
                    .SetProperty(x => x.SessionRoomID, session.SessionRoomID)
                    .SetProperty(x => x.SessionPromotion, session.SessionPromotion)
                    .SetProperty(x => x.SessionTimeSlot.From, session.SessionTimeSlot.From)
                    .SetProperty(x => x.SessionTimeSlot.To, session.SessionTimeSlot.To)
                    .SetProperty(x => x.priceTotal.Value, session.priceTotal.Value)
                );
        }
        public async Task SaveChangesAsync()
        {
            await using var _context = _contextFactory.CreateDbContext();
            await _context.SaveChangesAsync();
        }

        public async Task<Session> DeleteSessionAsync(Guid sessionId)
        {
            await using var _context = _contextFactory.CreateDbContext();
            var session = await GetSessionAsync(sessionId);
            _context.Sessions.Remove(session.Value);
            await _context.SaveChangesAsync();
            return session.Value;
        }

        public async Task<List<Session>> GetSessionsInRangeAsync(DateTime from, DateTime to)
        {
            await using var _context = _contextFactory.CreateDbContext();
            return await _context.Sessions
                .AsNoTracking()
                .Where(s =>
                s.SessionTimeSlot.To >= from &&
                s.SessionTimeSlot.From <= to)
                .ToListAsync();
        }
    }
}
