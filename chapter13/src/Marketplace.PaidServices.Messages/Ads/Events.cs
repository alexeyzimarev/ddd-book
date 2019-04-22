using System;

namespace Marketplace.PaidServices.Messages.Ads
{
    public static class Events
    {
        public class V1
        {
            public class ServiceAddedForClassifiedAd
            {
                public Guid ClassfiiedAdId { get; set; }
                public string Description { get; set; }
                public TimeSpan Duration { get; set; }
                public string Attributes { get; set; }
            }
        }
    }
}