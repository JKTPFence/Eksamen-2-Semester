using System.Globalization;
using DocumentFormat.OpenXml.InkML;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Facade.Queries;
using FysioEnterprise.Facade.UseCase.PromotionUseCase;
using Microsoft.AspNetCore.Components;
using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.Presentation.Components.Pages
{
    public partial class PromotionPage : ComponentBase
    {
        [Inject] private IPromotionQueries PromotionQueries { get; set; } = default!;
        [Inject] private ICreatePromotionUseCase CreatePromotionUseCase { get; set; } = default!;
        [Inject] private IUpdatePromotionUseCase UpdatePromotionUseCase { get; set; } = default!;
        [Inject] private IDeletePromotionUseCase DeletePromotionUseCase { get; set; } = default!;

        public static readonly CultureInfo DanishCulture = new("da-DK");

        private class PromotionEditModel
        {
            public Guid PromotionID { get; set; }
            public string Name { get; set; } = "";
            public decimal DiscountPercentage { get; set; }
            public DateTime StartDate { get; set; } = DateTime.Today;
            public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);
        }

        private List<PromotionDTO> promotions = new();
        private List<PromotionDTO> filteredPromotions = new();
        private PromotionEditModel currentPromotion = new();
        private PromotionDTO? promotionToDelete;

        private string searchTerm = "";
        private bool showModal = false;
        private bool showDeleteConfirm = false;
        private bool isEditMode = false;
        private string errorMessage = "";
        private bool showError = false;
        private bool _loading = false;
        private string _loadingMessage = "Henter data...";
        private const int MaxRetries = 3;

        protected override async Task OnInitializedAsync()
        {
            await LoadPromotions();
        }

        private async Task LoadPromotions(int attempt = 1)
        {
            if (_loading && attempt == 1) return;

            _loading = true;
            _loadingMessage = attempt > 1 ? "Synkroniserer med databasen..." : "Henter data...";
            StateHasChanged();
            try
            {
                promotions = await PromotionQueries.GetAllPromotionsAsync();
                FilterPromotions();
            }
            catch (Exception ex) when (ex.Message.Contains("second operation was started"))
            {
                System.Diagnostics.Debug.WriteLine($"[DB Collision Caught] Attempt {attempt} failed. Retrying...");

                if (attempt < MaxRetries)
                {
                    await Task.Delay(500);

                    await LoadPromotions(attempt + 1);
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

        private void FilterPromotions()
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredPromotions = promotions;
            }
            else
            {
                var search = searchTerm.ToLower();
                filteredPromotions = promotions
                    .Where(p => p.PromotionName.ToLower().Contains(search))
                    .ToList();
            }
        }

        private void OnSearchChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? "";
            FilterPromotions();
        }

        private void OpenCreateModal()
        {
            currentPromotion = new PromotionEditModel
            {
                PromotionID = Guid.Empty
            };
            isEditMode = false;
            showModal = true;
            errorMessage = "";
            showError = false;
        }

        private void OpenEditModal(PromotionDTO promotion)
        {
            currentPromotion = new PromotionEditModel
            {
                PromotionID = promotion.PromotionID,
                Name = promotion.PromotionName,
                DiscountPercentage = promotion.PromotionDiscountPercent,
                StartDate = promotion.PromotionStartTime,
                EndDate = promotion.PromotionEndTime
            };
            isEditMode = true;
            showModal = true;
            errorMessage = "";
            showError = false;
        }

        private async Task SavePromotion()
        {
            try
            {
                if (isEditMode && currentPromotion.PromotionID != Guid.Empty)
                {
                    var updateRequest = new UpdatePromotionRequest(
                        currentPromotion.PromotionID,
                        currentPromotion.Name,
                        currentPromotion.DiscountPercentage,
                        currentPromotion.StartDate,
                        currentPromotion.EndDate);

                    var result = await UpdatePromotionUseCase.UpdatePromotionAsync(updateRequest);
                    if (result.IsFailed)
                    {
                        errorMessage = string.Join(", ", result.Errors.Select(e => e.Message));
                        showError = true;
                        return;
                    }
                }
                else
                {
                    var createRequest = new CreatePromotionRequest(
                        Guid.NewGuid(),
                        currentPromotion.Name,
                        currentPromotion.DiscountPercentage,
                        currentPromotion.StartDate,
                        currentPromotion.EndDate);

                    var result = await CreatePromotionUseCase.CreatePromotionAsync(createRequest);
                    if (result.IsFailed)
                    {
                        errorMessage = string.Join(", ", result.Errors.Select(e => e.Message));
                        showError = true;
                        return;
                    }
                }

                await Task.Delay(50);
                await LoadPromotions();
                CloseModal();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                showError = true;
                Console.WriteLine($"Error saving promotion: {ex}");
            }
        }

        private void CloseModal()
        {
            showModal = false;
            currentPromotion = new();
            errorMessage = "";
            showError = false;
        }

        private void OpenDeleteConfirm(PromotionDTO promotion)
        {
            promotionToDelete = promotion;
            showDeleteConfirm = true;
        }

        private async Task DeletePromotion()
        {
            if (promotionToDelete != null)
            {
                try
                {
                    var deleteRequest = new DeletePromotionRequest(promotionToDelete.PromotionID);
                    var result = await DeletePromotionUseCase.DeletePromotionAsync(deleteRequest);
                    if (result.IsFailed)
                    {
                        errorMessage = string.Join(", ", result.Errors.Select(e => e.Message));
                        showError = true;
                        return;
                    }
                    await LoadPromotions();
                    CloseDeleteConfirm();
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    showError = true;
                    Console.WriteLine($"Error deleting promotion: {ex}");
                }
            }
        }

        private void CloseDeleteConfirm()
        {
            showDeleteConfirm = false;
            promotionToDelete = null;
        }
    }
}
