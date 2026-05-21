using System.Globalization;
using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.UseCase.ClientUseCase;
using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;

namespace FysioEnterprise.Presentation.Components.Pages
{
    public partial class ClientPage : ComponentBase
    {
        [Inject] private IClientQueries ClientQueries { get; set; } = default!;
        [Inject] private ISimpleQueries SimpleQueries { get; set; } = default!;
        [Inject] private ICreateClientUseCase CreateClientUseCase { get; set; } = default!;
        [Inject] private IUpdateClientUseCase UpdateClientUseCase { get; set; } = default!;
        [Inject] private IDeleteClientUseCase DeleteClientUseCase { get; set; } = default!;
        [Inject] private LogInContext Context { get; set; } = default!;

        private Dictionary<Guid, string> staffLookup = new();

        public static readonly CultureInfo DanishCulture = new("da-DK");

        private class ClientEditModel
        {
            public Guid ClientID { get; set; }
            public string ClientFirstName { get; set; } = "";
            public string ClientLastName { get; set; } = "";
            public string ClientEmail { get; set; } = "";
            public string ClientPhoneNumber { get; set; } = "";
            public DateOnly ClientBirthDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
            public string ClientAddress { get; set; } = "";
            public string? ClientNote { get; set; }
            public Guid ClientPrefferedStaffID { get; set; } = Guid.Empty;
        }

        private List<ClientDTO> clients = new();
        private List<ClientDTO> filteredClients = new();
        private ClientEditModel currentClient = new();
        private ClientDTO clientToDelete = null;
        private List<StaffDTO> _staff = new();

        private string searchTerm = "";
        private bool showModal = false;
        private bool showDeleteConfirm = false;
        private bool isEditMode = false;
        private string errorMessage = "";
        private bool showError = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadStaff();
            await LoadClients();
        }

        private async Task LoadClients()
        {
            clients = await ClientQueries.GetAllClientsAsync();
            FilterClients();
        }

        private async Task LoadStaff()
        {
            _staff = await SimpleQueries.GetAllStaffByClinicAsync(Context.ClinicId);

            staffLookup = _staff.ToDictionary(s => s.StaffID, s => $"{s.StaffFirstName} {s.StaffLastName}");

        }

        private string GetStaffNameById(Guid staffId)
        {
            return staffId == Guid.Empty
                ? "Ikke valgt"
                : (staffLookup.TryGetValue(staffId, out var name) ? name : "Ukendt personale");
        }

        private void FilterClients()
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredClients = clients;
            }
            else
            {
                var search = searchTerm.ToLower();
                filteredClients = clients
                    .Where(c => c.ClientFirstName.ToLower().Contains(search) ||
                               c.ClientLastName.ToLower().Contains(search) ||
                               c.ClientEmail.ToLower().Contains(search) ||
                               (c.ClientPhoneNumber != null && c.ClientPhoneNumber.ToLower().Contains(search)))
                    .ToList();
            }
        }

        private void OnSearchChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? "";
            FilterClients();
        }

        private void OpenCreateModal()
        {
            currentClient = new ClientEditModel();
            isEditMode = false;
            showModal = true;
            errorMessage = "";
            showError = false;
        }

        private void OpenEditModal(ClientDTO client)
        {
            currentClient = new ClientEditModel
            {
                ClientID = client.ClientID,
                ClientFirstName = client.ClientFirstName,
                ClientLastName = client.ClientLastName,
                ClientEmail = client.ClientEmail,
                ClientPhoneNumber = client.ClientPhoneNumber,
                ClientBirthDate = client.ClientBirthDate,
                ClientAddress = client.ClientAddress,
                ClientPrefferedStaffID = client.ClientPrefferedStaffID,
                ClientNote = client.ClientNote
            };
            isEditMode = true;
            showModal = true;
            errorMessage = "";
            showError = false;
        }

        private async Task SaveClient()
        {
            try
            {
                if (isEditMode)
                {
                    var updateRequest = new UpdateClientRequest(
                        currentClient.ClientID,
                        currentClient.ClientFirstName,
                        currentClient.ClientLastName,
                        currentClient.ClientEmail,
                        currentClient.ClientPhoneNumber,
                        currentClient.ClientBirthDate,
                        currentClient.ClientAddress,
                        currentClient.ClientNote ?? "",
                        currentClient.ClientPrefferedStaffID);

                    await UpdateClientUseCase.UpdateClientAsync(updateRequest);
                }
                else
                {
                    var createRequest = new CreateClientRequest(
                        Guid.NewGuid(),
                        currentClient.ClientFirstName,
                        currentClient.ClientLastName,
                        currentClient.ClientEmail,
                        currentClient.ClientPhoneNumber,
                        currentClient.ClientBirthDate,
                        currentClient.ClientAddress,
                        currentClient.ClientNote ?? "",
                        LoyaltyLevel.None,
                        currentClient.ClientPrefferedStaffID);

                    await CreateClientUseCase.CreateClientAsync(createRequest);
                }

                await LoadStaff();
                await LoadClients();
                CloseModal();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                showError = true;
                Console.WriteLine($"Error saving client: {ex}");
            }
        }

        private void CloseModal()
        {
            showModal = false;
            currentClient = new();
            errorMessage = "";
            showError = false;
        }

        private void OpenDeleteConfirm(ClientDTO client)
        {
            clientToDelete = client;
            showDeleteConfirm = true;
        }

        private async Task DeleteClient()
        {
            if (clientToDelete != null)
            {
                try
                {
                    var deleteRequest = new DeleteClientRequest(clientToDelete.ClientID);
                    await DeleteClientUseCase.DeleteClientAsync(deleteRequest);
                    await LoadClients();
                    CloseDeleteConfirm();
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    showError = true;
                    Console.WriteLine($"Error deleting client: {ex}");
                }
            }
        }

        private void CloseDeleteConfirm()
        {
            showDeleteConfirm = false;
            clientToDelete = null;
        }
    }
}
