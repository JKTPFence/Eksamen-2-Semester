using FysioEnterprise.Facade.DTOs;

namespace FysioEnterprise.Facade.Queries
{
    public interface IClientQueries
    {
        /// <summary>
        ///     Som receptionist vil jeg gerne kunne søge på klienter, så det gør processen hurtigere.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<ClientDTO?> GetClientByIdAsync(Guid clientId);

        /// <summary>
        ///     Som receptionist vil jeg gerne kunne søge på klienter, så det gør processen hurtigere.  
        /// </summary>
        /// <returns></returns>
        Task<List<ClientDTO>> GetAllClientsAsync();
    }
}
