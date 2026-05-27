using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.UseCase.IRepositories;
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
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                return Result.Fail($"Session with ID {sessionId} was not found.");

            return Result.Ok(session);
        }

        public async Task<List<Session>> GetSessionsByClientAsync(Guid clientId)
        {
            var sessions = await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionClientID == clientId)
                .ToListAsync();

            return sessions;
        }

        public async Task<List<Session>> GetSessionsByStaffAsync(Guid staffId)
        {
            var sessions = await _context.Sessions
                .AsNoTracking()
                .Where(s => s.SessionStaffID == staffId)
                .ToListAsync();

            return sessions;
        }

        public async Task<List<Session>> GetSessionsByRoomAsync(Guid clinicId, Guid roomId)
        {
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
                .ToListAsync(); 

            return sessions;
        }

        public async Task UpdateSessionAsync(Session session)
        {
            var sessionCompleted = _context.Sessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task<Session> DeleteSessionAsync(Guid sessionId)
        {
            var session = await GetSessionAsync(sessionId);
            _context.Sessions.Remove(session.Value);
            await _context.SaveChangesAsync();
            return session.Value;
        }

        public async Task<List<Session>> GetSessionsInRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Sessions
                .AsNoTracking()
                .Where(s =>
                s.SessionTimeSlot.To >= from &&
                s.SessionTimeSlot.From <= to)
                .ToListAsync();
        }
    }
}
