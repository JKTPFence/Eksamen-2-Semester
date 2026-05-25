using System.Globalization;
using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.UseCase.ClientUseCase;
using FysioEnterprise.Presentation.Service;
using Microsoft.AspNetCore.Components;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

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
            public LoyaltyLevel ClientLoyaltyLevel { get; set; } = LoyaltyLevel.None;
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
        private bool _loading = false;
        private string _loadingMessage = "Henter data...";
        private const int MaxRetries = 3;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Context.LoadFromStorageAsync();
                await LoadData();
                StateHasChanged();
            }
        }

        protected override Task OnInitializedAsync() => Task.CompletedTask;

        private async Task LoadData(int attempt = 1)
        {
            if (_loading && attempt == 1) return;

            _loading = true;
            _loadingMessage = attempt > 1 ? "Synkroniserer med databasen..." : "Henter data...";
            StateHasChanged();
            try
            {
                clients = await ClientQueries.GetAllClientsAsync();
                FilterClients();
                _staff = await SimpleQueries.GetAllStaffByClinicAsync(Context.ClinicId);
                _loading = false;
            }
            catch (Exception ex) when (ex.Message.Contains("second operation was started"))
            {
                System.Diagnostics.Debug.WriteLine($"[DB Collision Caught] Attempt {attempt} failed. Retrying...");

                if (attempt < MaxRetries)
                {
                    await Task.Delay(500);

                    await LoadData(attempt + 1);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[DB Collision] Maximum retries reached. Context is permanently locked.");
                }
            }
            finally
            {
                if (attempt == 1 || !_loading)
                {
                    _loading = false;
                    StateHasChanged();
                }
            }
        }

        private async Task UpdateData()
        {
            clients = await ClientQueries.GetAllClientsAsync();
            FilterClients();
            _staff = await SimpleQueries.GetAllStaffByClinicAsync(Context.ClinicId);
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
                ClientNote = client.ClientNote,
                ClientLoyaltyLevel = client.ClientLoyaltyLevel
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

                await Task.Delay(50);
                await UpdateData();
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
                    await UpdateData();
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
