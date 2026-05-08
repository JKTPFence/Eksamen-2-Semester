using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Entities
{
    public class SessionType
    {
        public Guid SessionTypeId { get; private set; }
        public string SessionTypeName { get; private set; }
        public decimal SessionTypePrice { get; private set; }
        public int SessionTypeMaxAmount { get; private set; }
        public TimeOnly SessionTypeTimeSpan { get; private set; }

        public SessionType(string sessionTypeName, decimal sessionTypePrice, int sessionTypeMaxAmount, TimeOnly sessionTypeTimeSpan)
        {
            SessionTypeId = Guid.NewGuid();
            SessionTypeName = sessionTypeName;
            SessionTypePrice = sessionTypePrice;
            SessionTypeMaxAmount = sessionTypeMaxAmount;
            SessionTypeTimeSpan = sessionTypeTimeSpan;
        }
    }
}
