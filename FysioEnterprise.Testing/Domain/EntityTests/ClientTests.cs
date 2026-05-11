using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.ValueObjects;
using Xunit;

namespace FysioEnterprise.Testing.Domain.EntityTests
{
    public class ClientTests
    {
        private static Guid BuildStaff()
        {
            var staff = Guid.NewGuid();
            return staff;
        }

        //Create client
        private static Client BuildClient(
            string firstName = "Johanne",
            string lastName = "Testing",
            string email = "johanne@example.com",
            string phone = "71362851",
            string address = "Valløesgade 37, 2. th, 7100 Vejle",
            Guid? staff = null)    
        {
            return Client.Create(
                firstName, lastName, email, phone,
                new DateOnly(1990, 6, 15), address,
                clientNote: null,
                clientPrefferedStaffID: BuildStaff(),
                clientLoyaltyLevel: LoyaltyLevel.Bronze);
        }

        [Fact]
        public void Create_ValidInputs_ReturnsClient()
        {
            var staff = BuildStaff();
            var client = BuildClient(staff: Guid.NewGuid());

            Assert.NotEqual(Guid.Empty, client.Id);
            Assert.Equal("Jane", client.ClientFirstName);
            Assert.Equal(staff, client.ClientPrefferedStaffID);
        }

        [Theory]
        [InlineData("firstName", "", "johanne@example.com", "71362851", "Valløesgade 37, 2. th, 7100 Vejle")]
        [InlineData("email", "Johanne", "", "71362851", "Valløesgade 37, 2. th, 7100 Vejle")]
        [InlineData("phone", "Johanne", "johanne@example.com", "", "Valløesgade 37, 2. th, 7100 Vejle")]
        [InlineData("address", "Johanne", "johanne@example.com", "71362851", "")]
        public void Create_EmptyRequiredField_ThrowsDomainException(
            string _, string firstName, string email, string phone, string address)
        {
            Assert.Throws<DomainException>(() =>
                Client.Create(firstName, null, email, phone,
                    new DateOnly(1990, 1, 1), address,
                    null, BuildStaff(), LoyaltyLevel.Bronze));
        }

        //Update preffered staff
        [Fact]
        public void UpdatePrefferedStaff_ValidStaff_UpdatesPreferredStaffId()
        {
            var client = BuildClient();
            var newStaff = BuildStaff();

            client.UpdateStaff(newStaff);

            Assert.Equal(newStaff, client.ClientPrefferedStaffID);
        }

        [Fact]
        public void UpdateStaff_NullStaff_ThrowsDomainException()
        {
            var client = BuildClient();

            Assert.Throws<DomainException>(() => client.UpdateStaff(Guid.NewGuid()));
        }

        //Note test
        [Fact]
        public void UpdateClientNote_SetsNote()
        {
            var client = BuildClient();

            client.UpdateClientNote("Prefers morning slots");

            Assert.Equal("Prefers morning slots", client.ClientNote);
        }


        //Birthday month and discount check
        [Fact]
        public void IsBirthdayMonth_SameMonth_ReturnsTrue()
        {
            var client = BuildClient(); // birth month = June

            Assert.True(client.IsBirthdayMonth(new DateOnly(2025, 6, 1)));
        }

        [Fact]
        public void IsBirthdayMonth_DifferentMonth_ReturnsFalse()
        {
            var client = BuildClient();

            Assert.False(client.IsBirthdayMonth(new DateOnly(2025, 3, 1)));
        }


        [Fact]
        public void MarkBirthdayDiscountUsed_InBirthdayMonth_SetsDiscount()
        {
            var client = BuildClient(); // birth month = June
            var birthdayDate = new DateOnly(DateTime.Now.Year, 6, 1);

            client.MarkBirthdayDiscountUsed(birthdayDate);

            Assert.True(client.HasUsedBirthdayDiscountThisYear);
        }

        [Fact]
        public void MarkBirthdayDiscountUsed_OutsideBirthdayMonth_ThrowsDomainException()
        {
            var client = BuildClient();

            Assert.Throws<DomainException>(() =>
                client.MarkBirthdayDiscountUsed(new DateOnly(DateTime.Now.Year, 3, 1)));
        }

        [Fact]
        public void MarkBirthdayDiscountUsed_AlreadyUsedThisYear_ThrowsDomainException()
        {
            var client = BuildClient();
            var birthdayDate = new DateOnly(DateTime.Now.Year, 6, 1);
            client.MarkBirthdayDiscountUsed(birthdayDate);

            Assert.Throws<DomainException>(() =>
                client.MarkBirthdayDiscountUsed(birthdayDate));
        }
    }
}
