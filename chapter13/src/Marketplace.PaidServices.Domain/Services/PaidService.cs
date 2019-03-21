using System;

namespace Marketplace.PaidServices.Domain.Services
{
    public abstract class PaidService
    {
        public static PaidService[] AvailableServices =
        {
            new ShowOnTop(), 
            new LargeCard(), 
            new AccentedCard()
        };

        public double Price { get; private set; }
        public TimeSpan Duration { get; private set; } = TimeSpan.Zero;
        public string Description { get; private set; }

        public class ShowOnTop : PaidService
        {
            public ShowOnTop()
            {
                Price = 10;
                Duration = TimeSpan.FromDays(7);
                Description = "Show on top of the list for seven days";
            }
        }

        public class LargeCard : PaidService
        {
            public LargeCard()
            {
                Price = 5;
                Description = "Show as a large card";
            }
        }

        public class AccentedCard : PaidService
        {
            public AccentedCard()
            {
                Price = 3;
                Description = "Show accented";
            }
        }
    }
}