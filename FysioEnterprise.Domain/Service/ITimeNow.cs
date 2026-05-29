namespace FysioEnterprise.Domain.Service
{
        public interface ITimeNow
        {
            public DateTime Now();
        }

        public class CurrentDateTime : ITimeNow
    {
            DateTime ITimeNow.Now()
            {
                return DateTime.Now;
            }
        }
}
