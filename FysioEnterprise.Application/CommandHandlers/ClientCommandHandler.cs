using FluentResults;
using FysioEnterprise.UseCase.IRepositories;
using FysioEnterprise.Domain.Entities;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;
using FysioEnterprise.Facade.UseCase.ClientUseCase;

namespace FysioEnterprise.UseCase.CommandHandler.ClientCommands
{
    public class ClientCommandHandler : ICreateClientUseCase, IDeleteClientUseCase, IUpdateClientUseCase, IUpdateClientPrefferedStaffUseCase, IUpdateClientNoteUseCase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IStaffRepository _staffRepository;

        public ClientCommandHandler(IClientRepository clientRepository, IStaffRepository staffRepository)
        {
            _clientRepository = clientRepository;
            _staffRepository = staffRepository;
        }
        public async Task<Result> CreateClientAsync(CreateClientRequest request)
        {
            if (request == null)
                return Result.Fail("Request cannot be null.");
            if (string.IsNullOrWhiteSpace(request.FirstName))
                return Result.Fail("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Fail("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                return Result.Fail("Phone number cannot be empty.");
            if (string.IsNullOrWhiteSpace(request.Address))
                return Result.Fail("Address cannot be empty.");

            var preferredStaff = await _staffRepository.GetStaffAsync(request.StaffID);
            if (preferredStaff.IsFailed)
                return Result.Fail("Preferred staff not found.");

            var client = new Client(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.DateOfBirth,
                request.Address,
                request.Note,
                preferredStaff.Value,
                request.LoyaltyLevel);

            return await _clientRepository.CreateClientAsync(client);
        }

        public async Task<Result> DeleteClientAsync(DeleteClientRequest request)
        {
            if (request.ClientID == Guid.Empty)
                return Result.Fail("Client ID cannot be empty.");

            return await _clientRepository.DeleteClientAsync(request.ClientID);
        }
        public async Task<Result> UpdateClientAsync(UpdateClientRequest request)
        {
            if (request.ClientID == Guid.Empty)
                return Result.Fail("Client ID cannot be empty.");

            var clientResult = await _clientRepository.GetClientAsync(request.ClientID);
            if (clientResult.IsFailed)
                return Result.Fail($"Client with ID {request.ClientID} was not found.");

            var client = clientResult.Value;

            client.UpdateClient(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.DateOfBirth,
                request.Address);

            var updateResult = await _clientRepository.UpdateClientAsync(client);
            if (updateResult.IsFailed)
                return Result.Fail("An error occurred while updating the client.");

            return Result.Ok();
        }
        public async Task<Result> UpdateClientPrefferedStaffAsync(UpdateClientStaffRequest request)
        {
            if (request.ClientID == Guid.Empty)
                return Result.Fail("Client ID cannot be empty.");
            
            var clientResult = await _clientRepository.GetClientAsync(request.ClientID);
            if (clientResult.IsFailed)
                return Result.Fail($"Client with ID {request.ClientID} was not found.");
            
            var client = clientResult.Value;

            var staffResult = await _staffRepository.GetStaffAsync(request.StaffID);
            if (staffResult.IsFailed)
                return Result.Fail($"Staff with ID {request.StaffID} was not found.");

            var staff = staffResult.Value;
            
            client.UpdateStaff(staff);

            return Result.Ok();
        }
        public async Task<Result> UpdateClientNoteAsync(UpdateClientNoteRequest request)
        {
            if (request.ClientID == Guid.Empty)
                return Result.Fail("Client ID cannot be empty.");
            
            var clientResult = await _clientRepository.GetClientAsync(request.ClientID);
            if (clientResult.IsFailed)
                return Result.Fail($"Client with ID {request.ClientID} was not found.");
            
            var client = clientResult.Value;
            client.UpdateClientNote(request.Note);
            
            var updateResult = await _clientRepository.UpdateClientAsync(client);
            if (updateResult.IsFailed)
                return Result.Fail("An error occurred while updating the client's note.");

            return Result.Ok();
        }
    }
}
