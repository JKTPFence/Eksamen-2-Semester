using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Entities
{
    public class Client
    {
        public Guid ClientID { get; private set; }
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
        public Client(string clientFirstName, string? clientLastName, string clientEmail, string clientPhoneNumber, DateOnly clientBirthDate, string clientAddress, string? clientNote, Staff clientPrefferedStaff, LoyaltyLevel clientLoyaltyLevel)
        {
            if (string.IsNullOrWhiteSpace(clientFirstName)) 
                throw new ArgumentNullException("First name cannot be empty.", nameof(clientFirstName));
            if (string.IsNullOrWhiteSpace(clientEmail)) 
                throw new ArgumentNullException("Email cannot be empty.", nameof(clientEmail));
            if (string.IsNullOrWhiteSpace(clientPhoneNumber)) 
                throw new ArgumentNullException("Phone number cannot be empty.", nameof(clientPhoneNumber));
            if (string.IsNullOrWhiteSpace(clientAddress)) 
                throw new ArgumentNullException("Address cannot be empty.", nameof(clientAddress));

            ClientID = Guid.NewGuid();
            ClientPrefferedStaffID = clientPrefferedStaff.StaffID;
            ClientFirstName = clientFirstName;
            ClientLastName = clientLastName;
            ClientEmail = clientEmail;
            ClientPhoneNumber = clientPhoneNumber;
            ClientBirthDate = clientBirthDate;
            ClientAddress = clientAddress;
            ClientNote = clientNote;
            ClientLoyaltyLevel = clientLoyaltyLevel;
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
                throw new ArgumentException("First name cannot be empty.", nameof(clientFirstName));
            if (string.IsNullOrWhiteSpace(clientEmail))
                throw new ArgumentException("Email cannot be empty.", nameof(clientEmail));
            if (string.IsNullOrWhiteSpace(clientPhoneNumber))
                throw new ArgumentException("Phone number cannot be empty.", nameof(clientPhoneNumber));
            if (string.IsNullOrWhiteSpace(clientAddress))
                throw new ArgumentException("Address cannot be empty.", nameof(clientAddress));

            ClientFirstName = clientFirstName;
            ClientLastName = clientLastName;
            ClientEmail = clientEmail;
            ClientPhoneNumber = clientPhoneNumber;
            ClientBirthDate = clientBirthDate;
            ClientAddress = clientAddress;
        }

        public void UpdateStaff(Staff clientpreferredStaff)
        {
            if (clientpreferredStaff == null)
                throw new ArgumentNullException(nameof(clientpreferredStaff));

            ClientPrefferedStaffID = clientpreferredStaff.StaffID;
        }

        public void UpdateClientNote(string? clientNote)
        {
            ClientNote = clientNote;
        }
    }
}
