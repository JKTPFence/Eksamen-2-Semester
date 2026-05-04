using FysioEnterprise.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Commands.ClientCommands
{
    public interface IUpdateClientCommand
    {
        Task UpdateClientAsync(UpdateClientCommand command);
        public record UpdateClientCommand(
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
