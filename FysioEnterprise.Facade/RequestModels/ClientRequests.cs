using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Facade.RequestModels
{
    public class ClientRequests
    {
        public record CreateClientRequest(
            Guid ClientID,
            string FirstName,
            string LastName,
            string Email,
            string PhoneNumber,
            DateOnly DateOfBirth,
            string Address,
            string Note,
            LoyaltyLevel LoyaltyLevel,
            Guid StaffID);
        public record UpdateClientRequest(
            Guid ClientID,
            string FirstName,
            string LastName,
            string Email,
            string PhoneNumber,
            DateOnly DateOfBirth,
            string Address,
            string Note);
        public record DeleteClientRequest(
            Guid ClientID);

        public record UpdateClientStaffRequest(
            Guid ClientID,
            Guid StaffID);
        public record UpdateClientNoteRequest(
            Guid ClientID,
            string Note);
    }
}
