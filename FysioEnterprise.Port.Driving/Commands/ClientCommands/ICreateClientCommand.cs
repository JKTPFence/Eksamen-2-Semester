using System;
using System.Collections.Generic;
using System.Text;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Port.Driving.Commands.ClientCommands
{
    public interface ICreateClientCommand
    {
        Task CreateClientAsync(CreateClientCommand command);

        public record CreateClientCommand(
            Guid ClientID,
            Guid ClientPrefferedStaffID,
            string ClientFirstName,
            string ClientLastName,
            string ClientEmail,
            string ClientPhoneNumber,
            DateTime ClientBirthDate,
            string ClientAddress,
            string ClientNote,
            LoyaltyLevel ClientLoyaltyLevel);
    }
}
