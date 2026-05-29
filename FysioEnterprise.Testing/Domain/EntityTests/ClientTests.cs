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
            string lastName = "Jensen",
            string email = "johanne@example.com",
            string phone = "71362851",
            string address = "Valløesgade 37, 2. th, 7100 Vejle",
            Guid? staff = null)    
        {
            var expectedStaffId = Guid.NewGuid();
            return Client.Create(
                firstName, lastName, email, phone,
                new DateOnly(1990, 6, 15), address,
                clientNote: null,
                clientPrefferedStaffID: expectedStaffId,
                clientLoyaltyLevel: LoyaltyLevel.Bronze);
        }

        [Fact]
        public void Create_ValidInputs_ReturnsClient()
        {
            var expectedStaffId = Guid.NewGuid();
            var staff = BuildStaff();
            var client = Client.Create(
            "Johanne",
            "Jensen",
            "johanne@example.com",
            "71362851",
            new DateOnly(1995, 5, 15),
            "Valløesgade 37, 2. th, 7100 Vejle",
            clientNote: null,
            clientPrefferedStaffID: expectedStaffId,
            clientLoyaltyLevel: LoyaltyLevel.Gold
        );

            Assert.Equal(expectedStaffId, client.ClientPrefferedStaffID);
            Assert.Equal("Johanne", client.ClientFirstName);
            Assert.Equal("Jensen", client.ClientLastName);
        }

        [Theory]
        [InlineData("firstName", "", "johanne@example.com", "71362851", "Valløesgade 37, 2. th, 7100 Vejle")]
        [InlineData("email", "Johanne", "", "71362851", "Valløesgade 37, 2. th, 7100 Vejle")]
        [InlineData("phone", "Johanne", "johanne@example.com", "", "Valløesgade 37, 2. th, 7100 Vejle")]
        [InlineData("address", "Johanne", "johanne@example.com", "71362851", "")]
        public void Create_EmptyRequiredField_ThrowsDomainException(
            string _, string firstName, string email, string phone, string address)
        {
            Assert.Throws<UserInvalidInputException>(() =>
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
        public void UpdateStaff_EmptyGuid_ThrowsDomainException()
        {
            var client = Client.Create("Johanne", "Jensen", "johanne@example.com", "71362851",
                new DateOnly(1995, 5, 15), "Valløesgade 37, 2. th, 7100 Vejle", null, Guid.NewGuid(), LoyaltyLevel.Gold);

            var ex = Assert.Throws<NotFoundException>(() => client.UpdateStaff(Guid.Empty));

            Assert.Contains("Ingen medarbejder kunne findes", ex.Message);
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

            var ex = Assert.Throws<ValidationException>(() =>
                client.MarkBirthdayDiscountUsed(new DateOnly(2026, 9, 1)));

            Assert.Contains("Kan ikke bruge en et fødselsdagstilbud uden for fødselsdagsmåneden.", ex.Message);
        }

        [Fact]
        public void MarkBirthdayDiscountUsed_AlreadyUsedThisYear_ThrowsDomainException()
        {
            var client = BuildClient();
            var birthdayDate = new DateOnly(DateTime.Now.Year, 6, 1);
            client.MarkBirthdayDiscountUsed(birthdayDate);

            Assert.Throws<UserInvalidInputException>(() =>
                client.MarkBirthdayDiscountUsed(birthdayDate));
        }
    }
}
