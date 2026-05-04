using FysioEnterprise.Facade.DTOs;

namespace FysioEnterprise.Port.Driving.Queries
{
    public interface ISessionQuery
    {
        /// <summary>
        ///     Som receptionist vil jeg kunne se alle sessioner for en given kunde, så jeg kan få et overblik over kundens bookinger.
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        List<SessionDTO> GetAllBySessionId(Guid ClientId);
    }
}
