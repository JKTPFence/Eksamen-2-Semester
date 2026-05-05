using System;
using System.Collections.Generic;
using System.Text;

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
            DateTime DateOfBirth,
            string Address,
            string Note);
        public record UpdateClientRequest(
            Guid ClientID,
            string FirstName,
            string LastName,
            string Email,
            string PhoneNumber,
            DateTime DateOfBirth,
            string Address,
            string Note);
        public record DeleteClientRequest(
            Guid ClientID);
    }
}
