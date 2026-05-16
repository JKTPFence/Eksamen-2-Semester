using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Entities
{
    public class Client : Aggregateroot
    {
        public Guid ClientPrefferedStaffID { get; private set; }
        public string ClientFirstName { get; private set; }
        public string? ClientLastName { get; private set; }

        public string ClientEmail { get; private set; }
        public string ClientPhoneNumber { get; private set; }
        public DateOnly ClientBirthDate { get; private set; }
        public string ClientAddress { get; private set; }
        public string? ClientNote { get; private set; }
        public LoyaltyLevel ClientLoyaltyLevel { get; private set; }
        public int? BirthdayDiscountUsedYear { get; private set; }
        public bool HasUsedBirthdayDiscountThisYear =>
            BirthdayDiscountUsedYear == DateTime.Now.Year;
        public Client() // Empty constructor for EF Core
        {

        }

        private Client(
            string clientFirstName,
            string? clientLastName,
            string clientEmail,
            string clientPhoneNumber,
            DateOnly clientBirthDate,
            string clientAddress,
            string? clientNote,
            Guid clientPrefferedStaffID,
            LoyaltyLevel clientLoyaltyLevel)
        {
            Id = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(clientFirstName))
                throw new DomainException($"First name cannot be empty: {clientFirstName}");
            ClientFirstName = clientFirstName;
            ClientLastName = clientLastName;
            if (string.IsNullOrWhiteSpace(clientEmail))
                throw new DomainException($"Email cannot be empty: {clientEmail}");
            ClientEmail = clientEmail;
            if (string.IsNullOrWhiteSpace(clientPhoneNumber))
                throw new DomainException($"Phone number cannot be empty: {clientPhoneNumber}");
            ClientPhoneNumber = clientPhoneNumber;
            if (string.IsNullOrWhiteSpace(clientAddress))
                throw new DomainException($"Address cannot be empty: {clientAddress}");
            ClientAddress = clientAddress;
            ClientNote = clientNote;
            ClientPrefferedStaffID = clientPrefferedStaffID;
            ClientBirthDate = clientBirthDate;
            ClientLoyaltyLevel = clientLoyaltyLevel;
        }

        public static Client Create(
            string clientFirstName, 
            string? clientLastName, 
            string clientEmail, 
            string clientPhoneNumber, 
            DateOnly clientBirthDate, 
            string clientAddress, 
            string? clientNote, 
            Guid clientPrefferedStaffID, 
            LoyaltyLevel clientLoyaltyLevel)
        {

            var client = new Client(clientFirstName, clientLastName, clientEmail, clientPhoneNumber, clientBirthDate, clientAddress, clientNote, clientPrefferedStaffID, clientLoyaltyLevel);

            return client;
        }
        public bool IsBirthdayMonth(DateOnly date)
        {
            return date.Month == ClientBirthDate.Month;
        }

        public void MarkBirthdayDiscountUsed(DateOnly date)
        {
            if (!IsBirthdayMonth(date))
                throw new DomainException("Cannot mark birthday discount outside of birthday month.");
            if (HasUsedBirthdayDiscountThisYear)
                throw new DomainException("Birthday discount already used this year.");

            BirthdayDiscountUsedYear = DateTime.Now.Year;
        }
        public void UpdateClient(
            string clientFirstName,
            string? clientLastName,
            string clientEmail,
            string clientPhoneNumber,
            DateOnly clientBirthDate,
            string clientAddress
            )
            {
            if (string.IsNullOrWhiteSpace(clientFirstName))
                throw new DomainException($"First name cannot be empty: {clientFirstName}");
            if (string.IsNullOrWhiteSpace(clientEmail))
                throw new DomainException($"Email cannot be empty: {clientEmail}");
            if (string.IsNullOrWhiteSpace(clientPhoneNumber))
                throw new DomainException($"Phone number cannot be empty: {clientPhoneNumber}");
            if (string.IsNullOrWhiteSpace(clientAddress))
                throw new DomainException($"Address cannot be empty: {clientAddress}");
            ClientFirstName = clientFirstName;
            ClientLastName = clientLastName;
            ClientEmail = clientEmail;
            ClientPhoneNumber = clientPhoneNumber;
            ClientBirthDate = clientBirthDate;
            ClientAddress = clientAddress;
        }

        public void UpdateStaff(Guid clientpreferredStaffID)
        {
            if (clientpreferredStaffID == null)
                throw new DomainException($"No staff member with these informations could be found");

            ClientPrefferedStaffID = clientpreferredStaffID;
        }

        public void UpdateClientNote(string? clientNote)
        {
            ClientNote = clientNote;
        }
    }
}
