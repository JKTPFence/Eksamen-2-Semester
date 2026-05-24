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
                throw new UserInvalidInputException($"En klient skal have et fornavn");
            ClientFirstName = clientFirstName;
            ClientLastName = clientLastName;
            if (string.IsNullOrWhiteSpace(clientEmail))
                throw new UserInvalidInputException($"En klient skal have en email");
            ClientEmail = clientEmail;
            if (string.IsNullOrWhiteSpace(clientPhoneNumber))
                throw new UserInvalidInputException($"En klient skal have et telefonnummer");
            ClientPhoneNumber = clientPhoneNumber;
            if (string.IsNullOrWhiteSpace(clientAddress))
                throw new UserInvalidInputException($"En klient skal have en Adresse");
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
                throw new ValidationException("Kan ikke bruge en et fødselsdagstilbud uden for fødselsdagsmåneden.");
            if (HasUsedBirthdayDiscountThisYear)
                throw new UserInvalidInputException("Fødselsdagstilbud er allerede brugt");

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
                throw new UserInvalidInputException($"En klient skal have et fornavn");
            if (string.IsNullOrWhiteSpace(clientEmail))
                throw new UserInvalidInputException($"En klient skal have en email");
            if (string.IsNullOrWhiteSpace(clientPhoneNumber))
                throw new UserInvalidInputException($"En klient skal have et telefonnummer");
            if (string.IsNullOrWhiteSpace(clientAddress))
                throw new UserInvalidInputException($"En klient skal have en Adresse");
            ClientFirstName = clientFirstName;
            ClientLastName = clientLastName;
            ClientEmail = clientEmail;
            ClientPhoneNumber = clientPhoneNumber;
            ClientBirthDate = clientBirthDate;
            ClientAddress = clientAddress;
        }

        public void UpdateStaff(Guid clientpreferredStaffID)
        {
            if (clientpreferredStaffID == Guid.Empty)
                throw new NotFoundException($"No staff member with this ID could be found");

            ClientPrefferedStaffID = clientpreferredStaffID;
        }

        public void UpdateClientNote(string? clientNote)
        {
            ClientNote = clientNote;
        }
    }
}
