using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.ValueObjects
{
    public class SessionType
    {
        public string SessionTypeName { get; private set; }
        public int SessionTypePrice { get; private set; }
        public int SessionTypeMaxAmount { get; private set; }
        public TimeOnly SessionTypeTimeSpan { get; private set; }

        public SessionType(string sessionTypeName, int sessionTypePrice, int sessionTypeMaxAmount, TimeOnly sessionTypeTimeSpan)
        {
            SessionTypeName = sessionTypeName;
            SessionTypePrice = sessionTypePrice;
            SessionTypeMaxAmount = sessionTypeMaxAmount;
            SessionTypeTimeSpan = sessionTypeTimeSpan;
        }
    }
}
