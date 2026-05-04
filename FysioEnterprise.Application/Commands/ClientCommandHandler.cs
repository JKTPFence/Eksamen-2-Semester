using System;
using System.Collections.Generic;
using System.Text;
using FluentResults;
using FysioEnterprise.Application.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Port.Driving.Commands.ClientCommands;
using FysioEnterprise.UseCase.Repository.Interfaces;
using static FysioEnterprise.Port.Driving.Commands.ClientCommands.ICreateClientCommand;
using static FysioEnterprise.Port.Driving.Commands.ClientCommands.IDeleteClientCommand;
using static FysioEnterprise.Port.Driving.Commands.ClientCommands.IUpdateClientCommand;

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
        public async Task<Result> CreateClientAsync(CreateClientCommand command)
        {
            if (command == null)
                return Result.Fail("Command cannot be null.");
            if (string.IsNullOrWhiteSpace(command.ClientFirstName))
                return Result.Fail("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(command.ClientEmail))
                return Result.Fail("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(command.ClientPhoneNumber))
                return Result.Fail("Phone number cannot be empty.");
            if (string.IsNullOrWhiteSpace(command.ClientAddress))
                return Result.Fail("Address cannot be empty.");

            var preferredStaff = await _staffRepository.GetStaffAsync(command.ClientPrefferedStaffID);
            if (preferredStaff.IsFailed)
                return Result.Fail("Preferred staff not found.");

            var client = new Client(
                command.ClientFirstName,
                command.ClientLastName,
                command.ClientEmail,
                command.ClientPhoneNumber,
                DateOnly.FromDateTime(command.ClientBirthDate),
                command.ClientAddress,
                command.ClientNote,
                preferredStaff.Value,
                command.ClientLoyaltyLevel);

            return await _clientRepository.CreateClientAsync(client);
        }

        public async Task<Result> DeleteClientAsync(DeleteClientCommand command)
        {
            if (command.ClientID == Guid.Empty)
                return Result.Fail("Client ID cannot be empty.");

            return await _clientRepository.DeleteClientAsync(command.ClientID);
        }
        public async Task<Result> UpdateClientAsync(UpdateClientCommand command)
        {
            if (command.ClientID == Guid.Empty)
                return Result.Fail("Client ID cannot be empty.");

            var clientResult = await _clientRepository.GetClientAsync(command.ClientID);
            if (clientResult.IsFailed)
                return Result.Fail($"Client with ID {command.ClientID} was not found.");

            var client = clientResult.Value;

            client.UpdateClient(
                command.ClientFirstName,
                command.ClientLastName,
                command.ClientEmail,
                command.ClientPhoneNumber,
                command.ClientBirthDate,
                command.ClientAddress);

            var updateResult = await _clientRepository.UpdateClientAsync(client);
            if (updateResult.IsFailed)
                return Result.Fail("An error occurred while updating the client.");

            return Result.Ok();
        }
    }
}
