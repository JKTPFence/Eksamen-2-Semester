using System;
using System.Collections.Generic;
using System.Text;
using FysioEnterprise.Facade.DTOs;

namespace FysioEnterprise.Facade.Queries
{
    public interface ISessionQueries
    {
        Task<SessionDTO> GetSessionAsync(Guid id);
        Task<IReadOnlyList<SessionDTO>> GetAllSessionsAsync();
    }
    public interface ISessionTypesQueries
    {
        Task<IReadOnlyList<SessionTypeDTO>> GetAllSessionTypesAsync();
    }
    public interface IClientQueries
    {
        Task<IReadOnlyList<ClientDTO>> GetAllClientsAsync();
    }
    public interface IStaffQueries
    {
        Task<IReadOnlyList<StaffDTO>> GetAllStaffAsync();
    }
}
