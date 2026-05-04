using System;
using System.Collections.Generic;
using System.Text;
using FysioEnterprise.Application.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.UseCase.Repository.Interfaces;
using FysioEnterprise.Port.Driving.Commands.ClientCommands;

namespace FysioEnterprise.UseCase.Commands
{
    public class ClientCommandHandler : ICreateClientCommand, IDeleteClientCommand, IUpdateClientCommand
    {
        private readonly IClientRepository _clientRepository;
        private readonly IStaffRepository _staffRepository;

        public ClientCommandHandler(IClientRepository clientRepository, IStaffRepository staffRepository)
        {
            _clientRepository = clientRepository;
            _staffRepository = staffRepository;
        }
        public async Task CreateClientAsync(ICreateClientCommand.CreateClientCommand command)
        {
            try
            {
                if (command == null)
                    throw new ArgumentNullException(nameof(command));
                if (command.ClientFirstName == null)
                    throw new ArgumentNullException(nameof(command.ClientFirstName));
                if (command.ClientLastName == null)
                    throw new ArgumentNullException(nameof(command.ClientLastName));
                if (command.ClientEmail == null)
                    throw new ArgumentNullException(nameof(command.ClientEmail));
                if (command.ClientPhoneNumber == null)
                    throw new ArgumentNullException(nameof(command.ClientPhoneNumber));
                if (command.ClientBirthDate == null)
                    throw new ArgumentNullException(nameof(command.ClientBirthDate));
                if (command.ClientAddress == null)
                    throw new ArgumentNullException(nameof(command.ClientAddress));

                // Load preferred staff
                var preferredStaff = await _staffRepository.GetStaffAsync(command.ClientPrefferedStaffID);
                if (preferredStaff == null)
                    throw new ArgumentException("Preferred staff not found.");

                // Do
                var client = new Client(
                    command.ClientFirstName,
                    command.ClientLastName,
                    command.ClientEmail,
                    command.ClientPhoneNumber,
                    DateOnly.FromDateTime(command.ClientBirthDate),
                    command.ClientAddress,
                    command.ClientNote,
                    preferredStaff,
                    command.ClientLoyaltyLevel);

                // Save
                await _clientRepository.AddClientAsync(client);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the client.", ex);
            }
        }
        public async Task DeleteClientAsync(IDeleteClientCommand.DeleteClientCommand command)
        {
            try
            {
                if (command.ClientID == Guid.Empty)
                    throw new ArgumentException("Client ID cannot be empty.");
                // Load
                var client = await _clientRepository.GetClientAsync(command.ClientID);
                // Do
                client.DeleteClient();
                // Save
                await _clientRepository.UpdateClientAsync(client);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the client.", ex);
            }
        }
        public async Task UpdateClientAsync(IUpdateClientCommand.UpdateClientCommand command)
        {
            try
            {
                if (command.ClientID == Guid.Empty)
                    throw new ArgumentException("Client ID cannot be empty.");
                // Load
                var client = await _clientRepository.GetClientAsync(command.ClientID);
                // Do
                client.UpdateClient(
                    command.ClientFirstName,
                    command.ClientLastName,
                    command.ClientEmail,
                    command.ClientPhoneNumber,
                    command.ClientBirthDate,
                    command.ClientAddress,
                    command.ClientNote);
                // Save
                await _clientRepository.UpdateClientAsync(client);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the client.", ex);
            }
        }
    }
}
