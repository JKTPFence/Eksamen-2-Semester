
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Entities
{
    public class SessionType : Aggregateroot
    {
        public string SessionTypeName { get; private set; }
        public Price SessionTypePrice { get; private set; } = new(0);
        public int SessionTypeMaxAmount { get; private set; }
        public TimeOnly SessionTypeTimeSpan { get; private set; }
        public List<int> AllowedAuthorisationNumbers { get; private set; }

        private SessionType() //Ef core
        {

        }

        public SessionType(string sessionTypeName, Price sessionTypePrice, int sessionTypeMaxAmount, TimeOnly sessionTypeTimeSpan, List<int> allowedAuthorisationNumbers)
        {
            Id = Guid.NewGuid();
            SessionTypeName = sessionTypeName;
            SessionTypePrice = sessionTypePrice;
            SessionTypeMaxAmount = sessionTypeMaxAmount;
            AllowedAuthorisationNumbers = allowedAuthorisationNumbers;
            SessionTypeTimeSpan = sessionTypeTimeSpan;
        }
    }
}
