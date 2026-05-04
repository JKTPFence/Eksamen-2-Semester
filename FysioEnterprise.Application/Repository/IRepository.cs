using FysioEnterprise.UseCase.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.UseCase.Repository
{
    public interface IRepository
    {
        Task<Session?> GetSessionAsync(Guid id);
        Task<IReadOnlyList<Session>> GetAllSessionsByClientAsync(Guid ClientID);
        Task<IReadOnlyList<Session>> GetAllSessionsByStaffAsync(Guid StaffID);
        Task AddAsync(Session session);
        Task SaveAsync();
        Task GetClientbookningsByClientIDAsync(object clientID);
        Task GetStaffbookningsByStaffIDAsync(object staffID);
    }

    public interface ISessionTypeRepository
    {
        Task GetByIDAsync(object sessionTypeID);
        Task<SessionType?> GetSessionTypeAsync(Guid id);
    }
    public interface IClientRepository
    {
        Task GetByIDAsync(object clientID);
        Task<Client?> GetClientAsync(Guid id);
    }
    public interface IStaffRepository
    {
        Task GetByIDAsync(object staffID);
        Task<Staff> GetStaffAsync(Guid id);
    }
}
