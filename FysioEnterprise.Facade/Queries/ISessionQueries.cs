using FysioEnterprise.Facade.DTOs;

namespace FysioEnterprise.Facade.Queries
{
    public interface ISessionQueries
    {
        /// <summary>
        ///     Som receptionist vil jeg kunne se alle sessioner for en given kunde, så jeg kan få et overblik over kundens bookinger.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        //List<SessionDTO> GetAllBySessionId(Guid ClientId);

        /// <summary>
        ///     Som receptionist vil jeg have et bookingsystem som gøt det nemmere for mig at håndtere bookinger i alle vores klinikker.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task <SessionDTO?> GetSessionByIdAsync(Guid sessionId);

        /// <summary>
        ///     Som receptionist vil jeg kunne se alle sessioner for en given kunde, så jeg kan få et overblik over kundens bookinger.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<List<SessionDTO>> GetAllSessionsByClientIdAsync(Guid clientId);

        /// <summary>
        ///     Som receptionist vil jeg kunne se alle aktive sessioner for en given kunde, så jeg kan se kundens kommende bookinger.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<List<SessionDTO>> GetAllActiveSessionsByClientIdAsync(Guid clientId);

        /// <summary>
        ///     Som receptionist vil jeg kunne se alle aktive sessioner for en given behandler så jeg kan se behandlerens kalender.
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        Task<List<SessionDTO>> GetAllActiveSessionsByStaffIdAsync(Guid staffId);

        /// <summary>
        ///     Som receptionist vil jeg kunne se alle aktive sessioner for en given klinik så jeg kan se klinikkens kalender.
        /// </summary>
        /// <param name="clinicId"></param>
        /// <returns></returns>
        Task<List<SessionDTO>> GetAllActiveSessionsByClincIdAsync(Guid clinicId);

    }
}