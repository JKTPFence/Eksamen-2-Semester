using FluentResults;
using FysioEnterprise.Application.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FysioEnterprise.Infrastructure.Database.Repository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly AppDBContext _context;

        public SessionRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task CreateSessionAsync(Session session)
        {
                await _context.Sessions.AddAsync(session);
                await _context.SaveChangesAsync();
        }

        //Bruger ikke fluent result her, grundet komplikation med Deletemetode og andre metoder der bruger "GetSessionAsync"
        public async Task<Result<Session>> GetSessionAsync(Guid sessionId)
        {
            var session = await _context.Sessions
                .Include(s => s.SessionPromotion)
                .FirstOrDefaultAsync(s => s.SessionID == sessionId);

            if (session == null)
                return Result.Fail($"Session with ID {sessionId} was not found.");

            return Result.Ok(session);
        }

        public async Task<List<Session>> GetSessionsByClientAsync(Guid clientId)
        {
            var sessions = await _context.Sessions
                .AsNoTracking()
                .Include(s => s.SessionPromotion)
                .Where(s => s.SessionClientID == clientId)
                .ToListAsync();

            return sessions;
        }

        public async Task<List<Session>> GetSessionsByStaffAsync(Guid staffId)
        {
            var sessions = await _context.Sessions
                .AsNoTracking()
                .Include(s => s.SessionPromotion)
                .Where(s => s.SessionStaffID == staffId)
                .ToListAsync();

            return sessions;
        }

        public async Task UpdateSessionAsync(Session session)
        {
            _context.Sessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task<Session> DeleteSessionAsync(Guid sessionId)
        {
            var session = await GetSessionAsync(sessionId);
            _context.Sessions.Remove(session.Value);
            await _context.SaveChangesAsync();
            return session.Value;
        }
    }
}
