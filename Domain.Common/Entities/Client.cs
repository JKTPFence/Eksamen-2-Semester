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
        public Client() // Empty constructor for EF Core
        {

        }
        public Client(string clientFirstName, string? clientLastName, string clientEmail, string clientPhoneNumber, DateOnly clientBirthDate, string clientAddress, string? clientNote, Staff clientPrefferedStaffID, LoyaltyLevel clientLoyaltyLevel)
        {
            if (string.IsNullOrWhiteSpace(clientFirstName)) throw new ArgumentNullException(nameof(clientFirstName));
            if (string.IsNullOrWhiteSpace(clientEmail)) throw new ArgumentNullException(nameof(clientEmail));
            if (string.IsNullOrWhiteSpace(clientPhoneNumber)) throw new ArgumentNullException(nameof(clientPhoneNumber));
            if (string.IsNullOrWhiteSpace(clientAddress)) throw new ArgumentNullException(nameof(clientAddress));

            ClientID = Guid.NewGuid();
            ClientPrefferedStaffID = clientPrefferedStaffID.StaffID;
            ClientFirstName = clientFirstName;
            ClientLastName = clientLastName;
            ClientEmail = clientEmail;
            ClientPhoneNumber = clientPhoneNumber;
            ClientBirthDate = clientBirthDate;
            ClientAddress = clientAddress;
            ClientNote = clientNote;
            ClientLoyaltyLevel = clientLoyaltyLevel;
        }

        public Boolean IsBirthdayMonth(Client client)
        {
            try
            {
                if (client == null) throw new ArgumentNullException(nameof(client));

                if (DateOnly.FromDateTime(DateTime.Now).Month == client.ClientBirthDate.Month)
                    return true;
                else return false;

            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public void UpdateClient(string clientFirstName, string clientLastName, string clientEmail, string clientPhoneNumber, DateTime clientBirthDate, string clientAddress, string clientNote)
        {
            throw new NotImplementedException();
        }
        public void DeleteClient()
        {
            // This method can be used to mark the client as deleted in the database, if soft deletion is implemented.
            // For example, you could add a boolean property like "IsDeleted" and set it to true here.
        }
    }
}
