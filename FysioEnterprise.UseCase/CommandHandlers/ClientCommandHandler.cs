using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Facade.UseCase.ClientUseCase;
using FysioEnterprise.UseCase.IRepositories;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

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
                return Result.Fail("Fejl, der er ikke sendt nogen request");

            var preferredStaff = await _staffRepository.GetStaffAsync(request.StaffID);
            if (preferredStaff.IsFailed)
                return Result.Fail("Kunne ikke finde medarbejder" + preferredStaff.Errors);

            try
            { 
            var client = Client.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.DateOfBirth,
                request.Address,
                request.Note,
                preferredStaff.Value.Id,
                request.LoyaltyLevel);

            return await _clientRepository.CreateClientAsync(client);
            }
            catch (DomainException ex)
            {
                return ex switch
                {
                    NotFoundException => Result.Fail("Der var nogle oplysninger der ikke kunne blive fundet. " + ex.Message),
                    UserInvalidInputException => Result.Fail("Et input var ikke korrekt. " + ex.Message),
                    ValidationException => Result.Fail("Der er sket en valideringsfejl. " + ex.Message),
                    _ => Result.Fail("Der er sket en uforventet fejl. " + ex.Message) // Fallback catch-all for base DomainException
                };
            }
            catch (Exception ex) // Catch-all for any other unexpected exceptions (typically SQL or Infrastructure exceptions)
            {
                System.Diagnostics.Debug.WriteLine($"Infrastructure Failure: {ex.Message}");
                return Result.Fail("An unexpected system error occurred." + ex.Message);
            }
        }

        public async Task<Result> DeleteClientAsync(DeleteClientRequest request)
        {
            if (request.ClientID == Guid.Empty)
                return Result.Fail("Klient ikke fundet");

            return await _clientRepository.DeleteClientAsync(request.ClientID);
        }
        public async Task<Result> UpdateClientAsync(UpdateClientRequest request)
        {
            if (request.ClientID == Guid.Empty)
                return Result.Fail("Klient ikke fundet");

            var clientResult = await _clientRepository.GetClientAsync(request.ClientID);
            if (clientResult.IsFailed)
                return Result.Fail($"Klient med {request.ClientID} blev ikke fundet");

            var client = clientResult.Value;

            try
            {
                client.UpdateClient(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber,
                    request.DateOfBirth,
                    request.Address);

                var updateResult = await _clientRepository.UpdateClientAsync(client);
            }
            catch (DomainException ex)
            {
                return ex switch
                {
                    UserInvalidInputException => Result.Fail("Et input var ikke korrekt" + ex.Message),
                    _ => Result.Fail("Der er sket en uforventet fejl " + ex.Message) // Fallback catch-all for base DomainException
                };
            }
            catch (Exception ex) // Catch-all for any other unexpected exceptions (typically SQL or Infrastructure exceptions)
            {
                System.Diagnostics.Debug.WriteLine($"Infrastructure Failure: {ex.Message}");
                return Result.Fail("An unexpected system error occurred." + ex.Message);
            }
            return Result.Ok();
        }
        public async Task<Result> UpdateClientPrefferedStaffAsync(UpdateClientStaffRequest request)
        {
            if (request.ClientID == Guid.Empty)
                return Result.Fail("Klient ikke fundet");
            
            var clientResult = await _clientRepository.GetClientAsync(request.ClientID);
            if (clientResult.IsFailed)
                return Result.Fail($"Klient blev ikke fundet");

            var client = clientResult.Value;

            var staffResult = await _staffRepository.GetStaffAsync(request.StaffID);
            if (staffResult.IsFailed)
                return Result.Fail($"Medarbejder blev ikke fundet");

            var staff = staffResult.Value.Id;

            try
            {
                client.UpdateStaff(staff);
            }
            catch (DomainException ex)
            {
                return ex switch
                {
                    NotFoundException => Result.Fail("Der var nogle oplysninger der ikke kunne blive fundet " + ex.Message),
                    _ => Result.Fail("Der er sket en uforventet fejl " + ex.Message) // Fallback catch-all for base DomainException
                };
            }
            catch (Exception ex) // Catch-all for any other unexpected exceptions (typically SQL or Infrastructure exceptions)
            {
                System.Diagnostics.Debug.WriteLine($"Infrastructure Failure: {ex.Message}");
                return Result.Fail("An unexpected system error occurred." + ex.Message);
            }
            return Result.Ok();
        }
        public async Task<Result> UpdateClientNoteAsync(UpdateClientNoteRequest request)
        {
            if (request.ClientID == Guid.Empty)
                return Result.Fail("Klient blev ikke fundet");
            
            var clientResult = await _clientRepository.GetClientAsync(request.ClientID);
            if (clientResult.IsFailed)
                return Result.Fail($"Klient blev ikke fundet");
            
            var client = clientResult.Value;
            try
            {
                client.UpdateClientNote(request.Note);
                var updateResult = await _clientRepository.UpdateClientAsync(client);

                if (updateResult.IsFailed)
                {
                    return Result.Fail("An unexpected system error occurred.");
                }
            }
            catch (Exception ex) // Catch-all for any other unexpected exceptions (typically SQL or Infrastructure exceptions)
            {
                return Result.Fail("An unexpected system error occurred." + ex.Message);
            }

            return Result.Ok();
        }
    }
}
