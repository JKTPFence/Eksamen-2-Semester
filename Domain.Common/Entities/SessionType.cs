
namespace FysioEnterprise.Domain.Entities
{
    public class SessionType : Aggregateroot
    {
        public string SessionTypeName { get; private set; }
        public decimal SessionTypePrice { get; private set; }
        public int SessionTypeMaxAmount { get; private set; }
        public TimeOnly SessionTypeTimeSpan { get; private set; }

        public SessionType(string sessionTypeName, decimal sessionTypePrice, int sessionTypeMaxAmount, TimeOnly sessionTypeTimeSpan)
        {
            Id = Guid.NewGuid();
            SessionTypeName = sessionTypeName;
            SessionTypePrice = sessionTypePrice;
            SessionTypeMaxAmount = sessionTypeMaxAmount;
            SessionTypeTimeSpan = sessionTypeTimeSpan;
        }
    }
}
