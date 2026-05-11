
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using Xunit;

namespace FysioEnterprise.Testing.Domain.EntityTests
{
    public class PromotionTests
    {
        private static Promotion BuildPromotion(
        string name = "Summer Sale",
        decimal discount = 10m,
        DateTime? start = null,
        DateTime? end = null)
        {
            return Promotion.Create(
                name, discount,
                start ?? DateTime.Now.AddDays(-1),
                end ?? DateTime.Now.AddDays(7));
        }

        [Fact]
        public void Create_ValidInputs_ReturnsPromotion()
        {
            var promo = BuildPromotion();

            Assert.NotEqual(Guid.Empty, promo.PromotionID);
            Assert.Equal("Summer Sale", promo.PromotionName);
            Assert.Equal(10m, promo.PromotionDiscountPercent);
        }

        [Fact]
        public void Create_EmptyName_ThrowsDomainException() =>
            Assert.Throws<DomainException>(() => BuildPromotion(name: ""));

        [Fact]
        public void Create_ZeroDiscount_ThrowsDomainException() =>
            Assert.Throws<DomainException>(() => BuildPromotion(discount: 0));

        [Fact]
        public void IsActive_WithinDateRange_ReturnsTrue()
        {
            var promo = BuildPromotion(
                start: DateTime.Now.AddDays(-1),
                end: DateTime.Now.AddDays(1));

            Assert.True(promo.IsActive);
        }

        [Fact]
        public void IsActive_OutsideDateRange_ReturnsFalse()
        {
            var promo = BuildPromotion(
                start: DateTime.Now.AddDays(-10),
                end: DateTime.Now.AddDays(-1));

            Assert.False(promo.IsActive);
        }

        [Fact]
        public void UpdatePromotion_UpdatesAllFields()
        {
            var promo = BuildPromotion();
            var newStart = DateTime.Now.AddDays(1);
            var newEnd = DateTime.Now.AddDays(5);

            promo.UpdatePromotion("Winter Sale", 20m, newStart, newEnd);

            Assert.Equal("Winter Sale", promo.PromotionName);
            Assert.Equal(20m, promo.PromotionDiscountPercent);
            Assert.Equal(newStart, promo.PromotionStartTime);
            Assert.Equal(newEnd, promo.PromotionEndTime);
        }
    }
}
