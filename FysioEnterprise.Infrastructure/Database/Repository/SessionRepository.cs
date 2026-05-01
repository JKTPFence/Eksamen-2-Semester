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

        public async Task<Session> GetSessionAsync(Guid sessionId)
        {
            var session = await _context.Sessions
                .Include(s => s.SessionPromotion)
                .FirstOrDefaultAsync(s => s.SessionID == sessionId);

            if (session == null)
                throw new KeyNotFoundException($"Session with ID {sessionId} was not found.");

            return session;
        }

        public async Task<List<Session>> GetSessionsByClientIdAsync(Guid clientId)
        {
            var sessions = await _context.Sessions
                .AsNoTracking()
                .Include(s => s.SessionPromotion)
                .Where(s => s.SessionClientID == clientId)
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
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return session;
        }
    }
}
