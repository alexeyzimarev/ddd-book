using System;
using System.Linq;

namespace Marketplace.PaidServices.Domain.PaidService
{
    public class PaidService
    {
        public PaidServiceType Type { get; private set; }
        public TimeSpan Duration { get; private set; }

        public static PaidService[] AvailableServices = {
            new PaidService{Type = PaidServiceType.ShowOnTop, Duration = TimeSpan.FromDays(1)}
        };

        public static TimeSpan DurationFor(PaidServiceType paidServiceType)
        {
            var service = AvailableServices.FirstOrDefault(x => x.Type == paidServiceType);
            if (service == null)
                throw new Exceptions.UnknownService(paidServiceType);

            return service.Duration;
        }
    }

    public enum PaidServiceType
    {
        ShowOnTop
    }
}