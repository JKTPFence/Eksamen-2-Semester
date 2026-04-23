namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IPromotionRepository
    {
            void CreatePromotion(Domain.Entities.Promotion promotion);
            Domain.Entities.Promotion GetPromotion(int ID);
            void UpdatePromotion(Domain.Entities.Promotion promotion);
            Domain.Entities.Promotion DeletePromotion(int ID);
    }
}
