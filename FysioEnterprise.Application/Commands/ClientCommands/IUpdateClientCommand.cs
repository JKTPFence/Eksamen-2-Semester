using FluentResults;
using FysioEnterprise.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Commands.ClientCommands
{
    public interface IUpdateClientCommand
    {
        Task<Result> UpdateClientAsync(UpdateClientCommand command);
        public record UpdateClientCommand(
            Guid ClientID,
            Guid ClientPrefferedStaffID,
            string ClientFirstName,
            string ClientLastName,
            string ClientEmail,
            string ClientPhoneNumber,
            DateOnly ClientBirthDate,
            string ClientAddress,
            string ClientNote,
            LoyaltyLevel ClientLoyaltyLevel);
    }
    public interface IUpdateStaffCommand
    {
        Task<Result> UpdateStaffAsync(UpdateStaffCommand command);
        public record UpdateStaffCommand(Guid ClientID, Guid ClientPrefferedStaffID);
    }
    public interface IUpdateNoteCommand
    {
        Task<Result> UpdateNoteAsync(UpdateNoteCommand command);
        public record UpdateNoteCommand(Guid ClientID, string ClientNote);
    }
}
