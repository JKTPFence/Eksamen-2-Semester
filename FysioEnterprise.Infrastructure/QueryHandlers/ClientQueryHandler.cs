using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Facade.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Infrastructure.QueryHandlers
{
    public class ClientQueriesImpl : IClientQueries
    {
        private readonly AppDBContext _context;

        public ClientQueriesImpl(AppDBContext context) => _context = context;


        public async Task<ClientDTO?> GetClientByIdAsync(Guid clientId)
        {
            return await _context.Clients
                .AsNoTracking()
                .Where(c => c.Id  == clientId)
                .Select(c => new ClientDTO(
                    c.Id,
                    c.ClientFirstName,
                    c.ClientLastName,
                    c.ClientEmail,
                    c.ClientPhoneNumber,
                    c.ClientBirthDate,
                    c.ClientAddress,
                    c.ClientNote,
                    c.ClientPrefferedStaffID,
                    _context.Staff
                    .Where(st => st.Id == c.ClientPrefferedStaffID)
                    .Select(st => $"{st.StaffFirstName} {st.StaffLastName}").FirstOrDefault() ?? "",
                    c.ClientLoyaltyLevel,
                    c.HasUsedBirthdayDiscountThisYear))
                .FirstOrDefaultAsync();
        }

        public async Task<List<ClientDTO>> GetAllClientsAsync()
        {
            return await _context.Clients
                .AsNoTracking()
                .Select(c => new ClientDTO(
                    c.Id,
                    c.ClientFirstName,
                    c.ClientLastName,
                    c.ClientEmail,
                    c.ClientPhoneNumber,
                    c.ClientBirthDate,
                    c.ClientAddress,
                    c.ClientNote,
                    c.ClientPrefferedStaffID,
                    _context.Staff
                    .Where(st => st.Id == c.ClientPrefferedStaffID)
                    .Select(st => $"{st.StaffFirstName} {st.StaffLastName}").FirstOrDefault() ?? "",
                    c.ClientLoyaltyLevel,
                    c.HasUsedBirthdayDiscountThisYear))
                .ToListAsync();
        }
    }
}